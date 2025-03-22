using System;
using System.Linq;
using ScriptableObjects;
using UnityEngine;

namespace Counter {
    public class CuttingCounter : BaseCounter {
        public event EventHandler<OnProgressChangedArgs> OnProgressChanged;

        public class OnProgressChangedArgs : EventArgs {
            public float ProgressNormalized;
        }

        public event EventHandler OnCut;


        [SerializeField] private CuttingRecipeSO[] cuttingRecipes;


        private int _currentNumberOfCuts = 0;


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
                new OnProgressChangedArgs { ProgressNormalized = _currentNumberOfCuts }
            );
        }

        public override void InteractAlternate() {
            var recipe = GetRecipe(GetKitchenObject()?.GetKitchenObjectSO());
            if (recipe == null) return;

            // Increment number of cuts
            _currentNumberOfCuts += 1;
            OnProgressChanged?.Invoke(
                this,
                new OnProgressChangedArgs { ProgressNormalized = (float)_currentNumberOfCuts / recipe.totalCuts }
            );
            OnCut?.Invoke(this, EventArgs.Empty);
            if (_currentNumberOfCuts < recipe.totalCuts) return;

            // Cutting is completed
            GetKitchenObject().DestroySelf();
            KitchenObject.KitchenObject.SpawnKitchenObject(recipe.output, this);
        }


        private CuttingRecipeSO GetRecipe(KitchenObjectSO kitchenObject) {
            return cuttingRecipes.FirstOrDefault(cuttingRecipe => cuttingRecipe.input == kitchenObject);
        }

        private KitchenObjectSO GetOutput(KitchenObjectSO kitchenObject) {
            return GetRecipe(kitchenObject)?.output;
        }

        private bool HasRecipe(KitchenObjectSO kitchenObject) {
            return GetOutput(kitchenObject) != null;
        }
    }
}
