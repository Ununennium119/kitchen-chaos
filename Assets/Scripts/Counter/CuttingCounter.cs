using System.Linq;
using ScriptableObjects;
using UnityEngine;

namespace Counter {
    public class CuttingCounter : BaseCounter {
        [SerializeField] private CuttingRecipeScriptableObject[] cuttingRecipes;


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
        }

        public override void InteractAlternate() {
            var output = GetOutput(GetKitchenObject()?.GetKitchenObjectScriptable());
            if (output == null) return;

            GetKitchenObject().DestroySelf();
            KitchenObject.SpawnKitchenObject(output, this);
        }


        private KitchenObjectScriptableObject GetOutput(KitchenObjectScriptableObject kitchenObject) {
            return cuttingRecipes.FirstOrDefault(cuttingRecipe => cuttingRecipe.input == kitchenObject)?.output;
        }

        private bool HasRecipe(KitchenObjectScriptableObject kitchenObject) {
            return GetOutput(kitchenObject) != null;
        }
    }
}
