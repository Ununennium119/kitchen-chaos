using System;
using Game.Manager;
using Game.Player;
using Unity.Netcode;

namespace Game.Counter.Logic {
    /// <summary>
    /// Represents a counter where the player can deliver a plate after it has been prepared.
    /// </summary>
    public class DeliveryCounter : BaseCounter {
        /// <summary>
        /// This event is triggered whenever player delivers a dish successfully.
        /// </summary>
        public event EventHandler OnDeliverySuccess;


        /// <remarks>
        /// This field is only set in the server.
        /// </remarks>
        private DeliveryManager _deliveryManager;

        
        // --- SERVER LOGIC ---

        private void Start() {
            if (IsServer) {
                _deliveryManager = DeliveryManager.Instance;
            }
        }


        /// <summary>
        /// Handles the player's interaction with the delivery counter.
        /// If the player has a plate, it attempts to deliver the plate to the DeliveryManager.
        /// </summary>
        /// <param name="playerController">The player who is interacting with the counter.</param>
        /// <remarks>
        /// Should only be called from server.
        /// </remarks>
        public override void Interact(PlayerController playerController) {
            var playerKitchenObject = playerController.GetKitchenObject();
            // Do nothing if player does not have plate
            if (playerKitchenObject?.TryGetPlateKitchenObject(out var plateKitchenObject) != true) return;
            // Do nothing if the plate cannot be delivered
            if (!_deliveryManager.DeliverPlate(plateKitchenObject)) return;

            // Plate is delivered
            plateKitchenObject.DestroySelf();

            // Update clients
            DeliverySuccessClientRpc();
        }

        /// <summary>
        /// Defines alternate interaction behavior. Currently, does nothing.
        /// </summary>
        public override void InteractAlternate() {
            // Do Nothing
        }

        
        // --- CLIENT LOGIC ---

        /// <summary>
        /// Client RPC that triggers <see cref="OnDeliverySuccess"/> event for the client.
        /// </summary>
        [ClientRpc]
        private void DeliverySuccessClientRpc() {
            OnDeliverySuccess?.Invoke(this, EventArgs.Empty);
        }
    }
}
