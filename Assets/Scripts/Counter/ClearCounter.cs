using ScriptableObjects.KitchenObjects;
using UnityEngine;

namespace Counter {
    public class ClearCounter : BaseCounter {
        [SerializeField] private KitchenObjectScriptable kitchenObjectScriptable;

        public override void Interact(Player player) {
            // Swap player and counter kitchen objects
            var playerKitchenObject = player.GetKitchenObject();
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
    }
}
