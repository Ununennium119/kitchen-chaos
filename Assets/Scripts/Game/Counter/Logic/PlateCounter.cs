using Game.Player;
using Game.ScriptableObjects;
using UnityEngine;

namespace Game.Counter.Logic {
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
