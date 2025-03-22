using System;
using System.Linq;
using ScriptableObjects;
using UI;
using UnityEngine;

namespace Counter {
    public class StoveCounter : BaseCounter, IHasProgress {
        public enum State {
            Idle,
            Frying,
            Fried,
            Burned,
        }


        public event EventHandler<OnStateChangedArgs> OnStateChanged;
        public class OnStateChangedArgs : EventArgs {
            public State State;
        }

        public event EventHandler<IHasProgress.OnProgressChangedArgs> OnProgressChanged;


        [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;


        private FryingRecipeSO _currentFryingRecipeSO;
        private State _currentState;
        private float _fryingTimer;
        private float _burningTimer;


        public override void Interact(Player.Player player) {
            // If player's kitchen object cannot be fried, do nothing
            var playerKitchenObject = player.GetKitchenObject();
            var recipeSO = GetRecipe(playerKitchenObject?.GetKitchenObjectSO());
            if (playerKitchenObject != null && recipeSO == null) {
                return;
            }

            // Swap player and counter kitchen objects
            var counterKitchenObject = GetKitchenObject();
            playerKitchenObject?.ClearParent();
            counterKitchenObject?.ClearParent();
            if (playerKitchenObject != null) {
                playerKitchenObject.SetParent(this);
            }
            if (counterKitchenObject != null) {
                counterKitchenObject.SetParent(player);
            }
            _currentFryingRecipeSO = recipeSO;

            // Start the stove
            if (recipeSO == null) {
                ChangeState(State.Idle);
                InvokeOnProgressChanged(0f);
                return;
            }
            var stoveKitchenObjectSO = GetKitchenObject().GetKitchenObjectSO();
            if (stoveKitchenObjectSO == recipeSO.rawKitchenObjectSO) {
                // Start frying
                _fryingTimer = 0;
                ChangeState(State.Frying);
            } else if (stoveKitchenObjectSO == recipeSO.friedKitchenObjectSO) {
                // Start burning
                _burningTimer = 0;
                ChangeState(State.Fried);
            }
        }


        private void Update() {
            if (_currentFryingRecipeSO is null) return;

            switch (_currentState) {
                case State.Idle:
                    // Do Nothing
                    break;
                case State.Frying:
                    _fryingTimer += Time.deltaTime;
                    if (_fryingTimer >= _currentFryingRecipeSO.fryingTime) {
                        _burningTimer = 0;
                        ChangeState(State.Fried);
                        InvokeOnProgressChanged(1f);
                        GetKitchenObject().DestroySelf();
                        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                        // This is only called in a specific state so it's not effecting performance
                        KitchenObject.KitchenObject.SpawnKitchenObject(
                            _currentFryingRecipeSO.friedKitchenObjectSO,
                            this
                        );
                    } else {
                        InvokeOnProgressChanged(_fryingTimer / _currentFryingRecipeSO.fryingTime);
                    }
                    break;
                case State.Fried:
                    _burningTimer += Time.deltaTime;
                    if (_burningTimer >= _currentFryingRecipeSO.burningTime) {
                        ChangeState(State.Burned);
                        InvokeOnProgressChanged(1f);
                        GetKitchenObject().DestroySelf();
                        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                        // This is only called in a specific state so it's not effecting performance
                        KitchenObject.KitchenObject.SpawnKitchenObject(
                            _currentFryingRecipeSO.burnedKitchenObjectSO,
                            this
                        );
                    } else {
                        InvokeOnProgressChanged(_burningTimer / _currentFryingRecipeSO.burningTime);
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


        private void ChangeState(State newState) {
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
