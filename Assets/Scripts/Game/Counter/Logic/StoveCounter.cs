using System;
using System.Linq;
using Game.Player;
using Game.ScriptableObjects;
using Game.UI.WorldSpace.Progress;
using Unity.Netcode;
using UnityEngine;

namespace Game.Counter.Logic {
    /// <summary>
    /// Represents a stove counter where players can fry and burn items based on frying recipes.
    /// </summary>
    public class StoveCounter : BaseCounter, IHasProgress {
        /// <summary>
        /// Enumeration representing the different states of the stove.
        /// </summary>
        public enum State {
            Idle,
            Frying,
            Fried,
            Burned,
        }


        /// <summary>
        /// This event is triggered whenever state of the stove changes.
        /// </summary>
        public event EventHandler<OnStateChangedArgs> OnStateChanged;
        public class OnStateChangedArgs : EventArgs {
            public State State;
        }

        /// <summary>
        /// This event is triggered whenever progress of the frying changes.
        /// </summary>
        public event EventHandler<IHasProgress.OnProgressChangedArgs> OnProgressChanged;


        [SerializeField, Tooltip("Scriptable object of list of the frying recipes.")]
        private FryingRecipeListSO fryingRecipeListSO;


        private FryingRecipeSO _currentFryingRecipeSO;
        private State _currentState;
        private readonly NetworkVariable<float> _fryingTime = new();
        private readonly NetworkVariable<float> _burningTime = new();


        /// <summary>
        /// Handles the player's interaction with the stove counter.
        /// If possible, swaps kitchen objects between player and counter.
        /// </summary>
        /// <param name="playerController">The player interacting with the stove.</param>
        public override void Interact(PlayerController playerController) {
            var playerKitchenObject = playerController.GetKitchenObject();
            var counterKitchenObject = GetKitchenObject();

            // If player has a plate and counter is not empty try to add counter kitchen object to the plate
            if (playerKitchenObject?.TryGetPlateKitchenObject(out var playerPlateKitchenObject) == true) {
                if (counterKitchenObject != null) {
                    if (!playerPlateKitchenObject.TryAddKitchenObject(counterKitchenObject.GetKitchenObjectSO())) {
                        return;
                    }
                    counterKitchenObject.DestroySelf();
                    ChangeStateServerRpc(State.Idle);
                    return;
                }
            }

            // If player's kitchen object cannot be fried, do nothing
            var recipeSO = GetRecipe(playerKitchenObject?.GetKitchenObjectSO());
            if (playerKitchenObject != null && recipeSO == null) {
                return;
            }

            // Swap player and counter kitchen objects
            playerKitchenObject?.ClearParent();
            counterKitchenObject?.ClearParent();
            playerKitchenObject?.SetParent(this);
            counterKitchenObject?.SetParent(playerController);

            // Update stove's current recipe
            UpdateRecipeSOServerRpc(GetRecipeIndex(recipeSO));
            _currentFryingRecipeSO = recipeSO;
            if (recipeSO == null) {
                ChangeStateServerRpc(State.Idle);
                return;
            }

            // Start the stove
            var stoveKitchenObjectSO = playerKitchenObject?.GetKitchenObjectSO();
            if (stoveKitchenObjectSO == recipeSO.rawKitchenObjectSO) {
                // Start frying
                ChangeStateServerRpc(State.Frying);
            } else if (stoveKitchenObjectSO == recipeSO.friedKitchenObjectSO) {
                // Start burning
                ChangeStateServerRpc(State.Fried);
            }
        }

        /// <summary>
        /// Defines alternate interaction behavior. Currently, does nothing.
        /// </summary>
        public override void InteractAlternate() {
            // Do Nothing
        }


        private void Update() {
            if (!IsServer) return;

            if (_currentFryingRecipeSO is null) return;

            switch (_currentState) {
                case State.Idle:
                    // Do Nothing
                    break;
                case State.Frying:
                    _fryingTime.Value += Time.deltaTime;
                    if (_fryingTime.Value >= _currentFryingRecipeSO.fryingTime) {
                        // Transition to Fried state once frying is complete
                        ChangeStateServerRpc(State.Fried);
                        GetKitchenObject().DestroySelf();
                        KitchenObject.KitchenObject.SpawnKitchenObject(
                            _currentFryingRecipeSO.friedKitchenObjectSO,
                            this
                        );
                    }
                    break;
                case State.Fried:
                    _burningTime.Value += Time.deltaTime;
                    if (_burningTime.Value >= _currentFryingRecipeSO.burningTime) {
                        // Transition to Burned state once burning is complete
                        ChangeStateServerRpc(State.Burned);
                        GetKitchenObject().DestroySelf();
                        KitchenObject.KitchenObject.SpawnKitchenObject(
                            _currentFryingRecipeSO.burnedKitchenObjectSO,
                            this
                        );
                    }
                    break;
                case State.Burned:
                    // Do Nothing
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void OnNetworkSpawn() {
            _fryingTime.OnValueChanged += TriggerOnProgressChanged;
            _burningTime.OnValueChanged += TriggerOnProgressChanged;
        }


        private FryingRecipeSO GetRecipe(KitchenObjectSO kitchenObjectSO) {
            if (kitchenObjectSO == null) return null;
            return fryingRecipeListSO.fryingRecipeSOList.FirstOrDefault(
                cuttingRecipe => cuttingRecipe.rawKitchenObjectSO == kitchenObjectSO ||
                                 cuttingRecipe.friedKitchenObjectSO == kitchenObjectSO
            );
        }

        private int GetRecipeIndex(FryingRecipeSO fryingRecipeSO) {
            if (fryingRecipeSO == null) return -1;
            return fryingRecipeListSO.fryingRecipeSOList.IndexOf(fryingRecipeSO);
        }


        private void ResetFryingAndBurningTimes() {
            _fryingTime.Value = 0;
            _burningTime.Value = 0;
        }

        /// <summary>
        /// Triggers <see cref="OnProgressChanged"/> event.
        /// </summary>
        /// <remarks>
        /// Invoked when the frying or burning progress changes.
        /// </remarks>
        private void TriggerOnProgressChanged(float previousValue, float newValue) {
            var totalTime = _currentState switch {
                State.Idle or State.Burned => 1,
                State.Frying => _currentFryingRecipeSO != null ? _currentFryingRecipeSO.fryingTime : 1,
                State.Fried => _currentFryingRecipeSO != null ? _currentFryingRecipeSO.burningTime : 1,
                _ => throw new ArgumentOutOfRangeException()
            };
            OnProgressChanged?.Invoke(
                this,
                new IHasProgress.OnProgressChangedArgs { ProgressNormalized = newValue / totalTime }
            );
        }


        /// <summary>
        /// Server RPC that changes the state of the stove.
        /// </summary>
        /// <param name="newState">The new state to change to.</param>
        [ServerRpc(RequireOwnership = false)]
        private void ChangeStateServerRpc(State newState) {
            ResetFryingAndBurningTimes();
            ChangeStateClientRpc(newState);
        }

        /// <summary>
        /// Client RPC that changes the state of the stove and triggers the state change event.
        /// </summary>
        /// <param name="newState">The new state to change to.</param>
        [ClientRpc]
        private void ChangeStateClientRpc(State newState) {
            _currentState = newState;
            OnStateChanged?.Invoke(
                this,
                new OnStateChangedArgs { State = newState }
            );
        }

        /// <summary>
        /// Server RPC that updates the frying recipe.
        /// </summary>
        /// <param name="index">The index of the frying recipe in the list.</param>
        [ServerRpc(RequireOwnership = false)]
        private void UpdateRecipeSOServerRpc(int index) {
            UpdateRecipeSOClientRpc(index);
        }

        /// <summary>
        /// Client RPC that updates the frying recipe.
        /// </summary>
        /// <param name="index">The index of the frying recipe in the list.</param>
        [ClientRpc]
        private void UpdateRecipeSOClientRpc(int index) {
            try {
                _currentFryingRecipeSO = fryingRecipeListSO.fryingRecipeSOList[index];
            } catch (ArgumentOutOfRangeException) {
                _currentFryingRecipeSO = null;
            }
        }
    }
}
