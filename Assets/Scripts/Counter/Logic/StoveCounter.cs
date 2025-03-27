using System;
using System.Linq;
using ScriptableObjects;
using UI.WorldSpace.Progress;
using UnityEngine;

namespace Counter.Logic {
    public class StoveCounter : BaseCounter, IHasProgress {
        public enum State {
            Idle,
            Frying,
            Fried,
            Burned,
        }


        /// <summary>
        /// This event is invoked whenever state of the stove changes.
        /// </summary>
        public event EventHandler<OnStateChangedArgs> OnStateChanged;
        public class OnStateChangedArgs : EventArgs {
            public State State;
        }

        /// <summary>
        /// This event is invoked whenever progress of the frying changes.
        /// </summary>
        public event EventHandler<IHasProgress.OnProgressChangedArgs> OnProgressChanged;


        [SerializeField, Tooltip("Scriptable objects of the frying recipes.")]
        private FryingRecipeSO[] fryingRecipeSOArray;


        private FryingRecipeSO _currentFryingRecipeSO;
        private State _currentState;
        private float _fryingTime;
        private float _burningTime;


        public override void Interact(Player.PlayerController playerController) {
            var playerKitchenObject = playerController.GetKitchenObject();
            var counterKitchenObject = GetKitchenObject();

            // If player has a plate and counter is not empty try to add counter kitchen object to the plate
            if (playerKitchenObject?.TryGetPlateKitchenObject(out var playerPlateKitchenObject) == true) {
                if (counterKitchenObject != null) {
                    if (!playerPlateKitchenObject.TryAddKitchenObject(counterKitchenObject.GetKitchenObjectSO())) {
                        return;
                    }
                    counterKitchenObject.DestroySelf();
                    ChangeState(State.Idle);
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
            _currentFryingRecipeSO = recipeSO;
            if (recipeSO == null) {
                ChangeState(State.Idle);
                return;
            }

            // Start the stove
            var stoveKitchenObjectSO = GetKitchenObject().GetKitchenObjectSO();
            if (stoveKitchenObjectSO == recipeSO.rawKitchenObjectSO) {
                // Start frying
                ChangeState(State.Frying);
            } else if (stoveKitchenObjectSO == recipeSO.friedKitchenObjectSO) {
                // Start burning
                ChangeState(State.Fried);
            }
        }

        public override void InteractAlternate() {
            // Do Nothing
        }


        private void Update() {
            if (_currentFryingRecipeSO is null) return;

            switch (_currentState) {
                case State.Idle:
                    // Do Nothing
                    break;
                case State.Frying:
                    IncreaseFryingTime(amount: Time.deltaTime, totalFryingTime: _currentFryingRecipeSO.fryingTime);
                    if (_fryingTime >= _currentFryingRecipeSO.fryingTime) {
                        ChangeState(State.Fried);
                        GetKitchenObject().DestroySelf();
                        KitchenObject.KitchenObject.SpawnKitchenObject(
                            _currentFryingRecipeSO.friedKitchenObjectSO,
                            this
                        );
                    }
                    break;
                case State.Fried:
                    IncreaseBurningTime(amount: Time.deltaTime, totalBurningTime: _currentFryingRecipeSO.burningTime);
                    if (_burningTime >= _currentFryingRecipeSO.burningTime) {
                        ChangeState(State.Burned);
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


        private FryingRecipeSO GetRecipe(KitchenObjectSO kitchenObjectSO) {
            if (kitchenObjectSO == null) return null;
            return fryingRecipeSOArray.FirstOrDefault(
                cuttingRecipe => cuttingRecipe.rawKitchenObjectSO == kitchenObjectSO ||
                                 cuttingRecipe.friedKitchenObjectSO == kitchenObjectSO
            );
        }


        private void IncreaseFryingTime(float amount, float totalFryingTime) {
            _fryingTime += amount;
            InvokeOnProgressChanged(_fryingTime / totalFryingTime);
        }

        private void IncreaseBurningTime(float amount, float totalBurningTime) {
            _burningTime += amount;
            InvokeOnProgressChanged(_burningTime / totalBurningTime);
        }

        private void ResetFryingAndBurningTimes() {
            _fryingTime = 0;
            _burningTime = 0;
            InvokeOnProgressChanged(0f);
        }

        private void ChangeState(State newState) {
            ResetFryingAndBurningTimes();
            _currentState = newState;
            OnStateChanged?.Invoke(
                this,
                new OnStateChangedArgs { State = newState }
            );
        }

        private void InvokeOnProgressChanged(float progressNormalized) {
            OnProgressChanged?.Invoke(
                this,
                new IHasProgress.OnProgressChangedArgs { ProgressNormalized = progressNormalized }
            );
        }
    }
}
