using System;
using System.Linq;
using ScriptableObjects;
using UI;
using UnityEngine;
using KitchenObject;

namespace Counter {
    public class CuttingCounter : BaseCounter, IHasProgress {
        public event EventHandler<IHasProgress.OnProgressChangedArgs> OnProgressChanged;

        public event EventHandler OnCut;


        [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;


        private int _currentNumberOfCuts;


        public override void Interact(Player.Player player) {
            // If player's kitchen object cannot be cut, do nothing
            var playerKitchenObject = player.GetKitchenObject();
            if (playerKitchenObject != null && !HasRecipe(playerKitchenObject.GetKitchenObjectSO())) {
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

            // Reset number of cuts
            _currentNumberOfCuts = 0;
            OnProgressChanged?.Invoke(
                this,
                new IHasProgress.OnProgressChangedArgs { ProgressNormalized = _currentNumberOfCuts }
            );
        }

        public override void InteractAlternate() {
            var recipeSO = GetRecipe(GetKitchenObject()?.GetKitchenObjectSO());
            if (recipeSO == null) return;

            // Increment number of cuts
            _currentNumberOfCuts += 1;
            OnProgressChanged?.Invoke(
                this,
                new IHasProgress.OnProgressChangedArgs { ProgressNormalized = (float)_currentNumberOfCuts / recipeSO.totalCuts }
            );
            OnCut?.Invoke(this, EventArgs.Empty);
            if (_currentNumberOfCuts < recipeSO.totalCuts) return;

            // Cutting is completed
            GetKitchenObject().DestroySelf();
            KitchenObject.KitchenObject.SpawnKitchenObject(recipeSO.output, this);
        }


        private CuttingRecipeSO GetRecipe(KitchenObjectSO kitchenObjectSO) {
            return cuttingRecipeSOArray.FirstOrDefault(cuttingRecipe => cuttingRecipe.input == kitchenObjectSO);
        }

        private bool HasRecipe(KitchenObjectSO kitchenObjectSO) {
            return GetRecipe(kitchenObjectSO) != null;
        }
    }
}
