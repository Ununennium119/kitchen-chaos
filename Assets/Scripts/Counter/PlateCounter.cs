using ScriptableObjects;
using UnityEngine;

namespace Counter {
    public class PlateCounter : BaseCounter {
        [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

        public override void Interact(Player.Player player) {
            if (!player.HasKitchenObject()) {
                KitchenObject.KitchenObject.SpawnKitchenObject(
                    plateKitchenObjectSO,
                    player
                );
            }
        }

        public override void InteractAlternate() {
            // Do Nothing
        }
    }
}
