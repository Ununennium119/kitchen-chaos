using System;

namespace Counter {
    public class DeliveryCounter : BaseCounter {
        private DeliveryManager _deliveryManager;


        private void Start() {
            _deliveryManager = DeliveryManager.Instance;
        }


        public override void Interact(Player.Player player) {
            var playerKitchenObject = player.GetKitchenObject();
            if (playerKitchenObject?.TryGetPlateKitchenObject(out var plateKitchenObject) != true) return;

            if (_deliveryManager.DeliverPlate(plateKitchenObject)) {
                plateKitchenObject.DestroySelf();
            }
        }

        public override void InteractAlternate() {
            // Do Nothing
        }
    }
}
