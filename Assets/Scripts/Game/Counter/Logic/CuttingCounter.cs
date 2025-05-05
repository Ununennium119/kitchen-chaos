using System;
using System.Linq;
using Game.Player;
using Game.ScriptableObjects;
using Game.UI.WorldSpace.Progress;
using Unity.Netcode;
using UnityEngine;

namespace Game.Counter.Logic {
    /// <summary>
    /// Represents a cutting counter where the player can interact to cut kitchen objects based on recipes.
    /// </summary>
    public class CuttingCounter : BaseCounter, IHasProgress {
        /// <summary>
        /// This event is triggered whenever a cut is performed in any of the cutting counters.
        /// </summary>
        public static event EventHandler OnAnyCut;


        /// <summary>
        /// Resets the static objects, specifically the OnTrash event.
        /// This method is used to clean up the event subscription.
        /// </summary>
        public static void ResetStaticObjects() {
            OnAnyCut = null;
        }


        /// <summary>
        /// This event is triggered whenever progress (number of cuts) is changed.
        /// </summary>
        public event EventHandler<IHasProgress.OnProgressChangedArgs> OnProgressChanged;

        /// <summary>
        /// This event is triggered whenever a cut is performed in any of the cutting counters.
        /// </summary>
        public event EventHandler OnCut;


        [SerializeField, Tooltip("Scriptable object of the cutting recipes")]
        private CuttingRecipeSO[] cuttingRecipeSOArray;

        [SerializeField, Tooltip("Scriptable object of list of kitchen objects")]
        private KitchenObjectListSO kitchenObjectListSO;


        /// <remarks>
        /// This field is only updated in the server.
        /// </remarks>
        private int _numberOfCuts;


        // --- SERVER LOGIC ---

        /// <summary>
        /// Handles player interaction with the cutting counter.
        /// If player doesn't have a kitchen object, or the kitchen object can be put on the cutting counter,
        /// the player kitchen object and the counter kitchen object will be swapped.
        /// </summary>
        /// <param name="playerController">The player interacting with the counter.</param>
        /// <remarks>
        /// Should only be called from server.
        /// </remarks>
        public override void Interact(PlayerController playerController) {
            var playerKitchenObject = playerController.GetKitchenObject();
            var counterKitchenObject = GetKitchenObject();

            // If player has a plate and counter is not empty try to move counter kitchen object to the plate
            if (playerKitchenObject?.TryGetPlateKitchenObject(out var playerPlateKitchenObject) == true) {
                if (counterKitchenObject != null) {
                    if (!playerPlateKitchenObject.TryAddKitchenObject(counterKitchenObject.GetKitchenObjectSO())) {
                        return;
                    }
                    counterKitchenObject.ClearParent();
                    counterKitchenObject.DestroySelf();
                    return;
                }
            }

            // If player's kitchen object cannot be cut, do nothing
            if (playerKitchenObject != null && !HasRecipe(playerKitchenObject.GetKitchenObjectSO())) {
                return;
            }

            // Swap player and counter kitchen objects
            playerKitchenObject?.ClearParent();
            counterKitchenObject?.ClearParent();
            playerKitchenObject?.SetParent(this);
            counterKitchenObject?.SetParent(playerController);

            // Reset number of cuts
            UpdateNumberOfCuts(0, 1, null);
        }

        /// <summary>
        /// Handles the alternate interaction for the cutting counter.
        /// A cut will be added to the kitchen object on the counter.
        /// </summary>
        /// <remarks>
        /// Should only be called from server.
        /// </remarks>
        public override void InteractAlternate() {
            // Do nothing if there is no recipe for the counter's kitchen object
            var kitchenObjectSO = GetKitchenObject()?.GetKitchenObjectSO();
            var recipeSO = GetRecipe(kitchenObjectSO);
            if (recipeSO == null) return;

            // Increment number of cuts
            UpdateNumberOfCuts(
                _numberOfCuts + 1,
                recipeSO.totalCuts,
                recipeSO.output
            );
            InvokeOnCutClientRpc();
        }

        /// <summary>
        /// Updates the number of cuts, progress, and finalizes cutting if necessary.
        /// </summary>
        /// <remarks>
        /// Should only be called from the server.
        /// </remarks>
        private void UpdateNumberOfCuts(
            int numberOfCuts,
            int totalNumberOfCuts,
            KitchenObjectSO outputKitchenObjectSO
        ) {
            _numberOfCuts = numberOfCuts;
            InvokeOnProgressChangedClientRpc(numberOfCuts, totalNumberOfCuts);

            if (outputKitchenObjectSO == null) return;
            if (numberOfCuts < totalNumberOfCuts) return;

            // Cutting is completed
            GetKitchenObject().DestroySelf();
            KitchenObject.KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
        }


        // --- CLIENT LOGIC ---

        /// <summary>
        /// Client RPC to invoke <see cref="OnProgressChanged"/> event.
        /// </summary>
        [ClientRpc]
        private void InvokeOnProgressChangedClientRpc(int numberOfCuts, int totalNumberOfCuts) {
            OnProgressChanged?.Invoke(
                this,
                new IHasProgress.OnProgressChangedArgs {
                    ProgressNormalized = (float)numberOfCuts / totalNumberOfCuts
                }
            );
        }

        /// <summary>
        /// Client RPC to invoke the <see cref="OnCut"/> event.
        /// </summary>
        [ClientRpc]
        private void InvokeOnCutClientRpc() {
            OnCut?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);
        }


        private CuttingRecipeSO GetRecipe(KitchenObjectSO kitchenObjectSO) {
            return cuttingRecipeSOArray.FirstOrDefault(cuttingRecipe => cuttingRecipe.input == kitchenObjectSO);
        }

        private bool HasRecipe(KitchenObjectSO kitchenObjectSO) {
            return GetRecipe(kitchenObjectSO) != null;
        }
    }
}
