using System;
using System.Linq;
using ScriptableObjects;
using UI.WorldSpace.Progress;
using UnityEngine;

namespace Counter.Logic {
    public class CuttingCounter : BaseCounter, IHasProgress {
        /// <summary>
        /// This event is invoked whenever a cut is performed in any of the cutting counters.
        /// </summary>
        public static event EventHandler OnAnyCut;


        public static void ResetStaticObjects() {
            OnAnyCut = null;
        }


        /// <summary>
        /// This event is invoked whenever progress (number of cuts) is changed.
        /// </summary>
        public event EventHandler<IHasProgress.OnProgressChangedArgs> OnProgressChanged;

        /// <summary>
        /// This event is invoked whenever a cut is performed in any of the cutting counters.
        /// </summary>
        public event EventHandler OnCut;


        [SerializeField, Tooltip("Scriptable object of the cutting recipes")]
        private CuttingRecipeSO[] cuttingRecipeSOArray;


        private int _numberOfCuts;


        public override void Interact(Player.PlayerController playerController) {
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
            UpdateNumberOfCuts(0, 1);
        }

        public override void InteractAlternate() {
            // Do nothing if there is no recipe for the counter's kitchen object
            var kitchenObjectSO = GetKitchenObject()?.GetKitchenObjectSO();
            var recipeSO = GetRecipe(kitchenObjectSO);
            if (recipeSO == null) return;

            // Increment number of cuts
            UpdateNumberOfCuts(_numberOfCuts + 1, recipeSO.totalCuts);
            OnCut?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);

            if (_numberOfCuts < recipeSO.totalCuts) return;

            // Cutting is completed
            GetKitchenObject().DestroySelf();
            KitchenObject.KitchenObject.SpawnKitchenObject(recipeSO.output, this);
        }


        private void UpdateNumberOfCuts(int numberOfCuts, int totalNumberOfCuts) {
            _numberOfCuts = numberOfCuts;
            OnProgressChanged?.Invoke(
                this,
                new IHasProgress.OnProgressChangedArgs { ProgressNormalized = (float)_numberOfCuts / totalNumberOfCuts }
            );
        }


        private CuttingRecipeSO GetRecipe(KitchenObjectSO kitchenObjectSO) {
            return cuttingRecipeSOArray.FirstOrDefault(cuttingRecipe => cuttingRecipe.input == kitchenObjectSO);
        }

        private bool HasRecipe(KitchenObjectSO kitchenObjectSO) {
            return GetRecipe(kitchenObjectSO) != null;
        }
    }
}
