using Player;
using ScriptableObjects;
using UnityEngine;

namespace Counter.Logic {
    public class PlateCounter : BaseCounter {
        [SerializeField, Tooltip("Scriptable object of the plate")]
        private KitchenObjectSO plateKitchenObjectSO;

        public override void Interact(PlayerController playerController) {
            if (!playerController.HasKitchenObject()) {
                KitchenObject.KitchenObject.SpawnKitchenObject(
                    plateKitchenObjectSO,
                    playerController
                );
            }
        }

        public override void InteractAlternate() {
            // Do Nothing
        }
    }
}
