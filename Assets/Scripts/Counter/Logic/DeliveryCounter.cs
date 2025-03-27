using System;
using Manager;

namespace Counter.Logic {
    public class DeliveryCounter : BaseCounter {
        /// <summary>
        /// This event is invoked whenever player delivers a dish successfully.
        /// </summary>
        public event EventHandler OnDeliverySuccess;


        private DeliveryManager _deliveryManager;


        private void Start() {
            _deliveryManager = DeliveryManager.Instance;
        }


        public override void Interact(Player.PlayerController playerController) {
            var playerKitchenObject = playerController.GetKitchenObject();
            // Do nothing if player does not have plate
            if (playerKitchenObject?.TryGetPlateKitchenObject(out var plateKitchenObject) != true) return;
            // Do nothing if the plate cannot be delivered
            if (!_deliveryManager.DeliverPlate(plateKitchenObject)) return;

            // Plate is delivered
            plateKitchenObject.DestroySelf();
            OnDeliverySuccess?.Invoke(this, EventArgs.Empty);
        }

        public override void InteractAlternate() {
            // Do Nothing
        }
    }
}
