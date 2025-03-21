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


        [SerializeField] private CuttingRecipeScriptableObject[] cuttingRecipes;


        private int _currentNumberOfCuts = 0;


        public override void Interact(Player player) {
            // If player's kitchen object cannot be cut, do nothing
            var playerKitchenObject = player.GetKitchenObject();
            if (playerKitchenObject != null && !HasRecipe(playerKitchenObject.GetKitchenObjectScriptable())) {
                return;
            }

            // Swap player and counter kitchen objects
            var counterKitchenObject = GetKitchenObject();
            player.ClearKitchenObject();
            ClearKitchenObject();
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
            var recipe = GetRecipe(GetKitchenObject()?.GetKitchenObjectScriptable());
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
            KitchenObject.SpawnKitchenObject(recipe.output, this);
        }


        private CuttingRecipeScriptableObject GetRecipe(KitchenObjectScriptableObject kitchenObject) {
            return cuttingRecipes.FirstOrDefault(cuttingRecipe => cuttingRecipe.input == kitchenObject);
        }

        private KitchenObjectScriptableObject GetOutput(KitchenObjectScriptableObject kitchenObject) {
            return GetRecipe(kitchenObject)?.output;
        }

        private bool HasRecipe(KitchenObjectScriptableObject kitchenObject) {
            return GetOutput(kitchenObject) != null;
        }
    }
}
