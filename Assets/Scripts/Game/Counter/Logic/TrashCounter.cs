using System;
using Game.Player;
using Unity.Netcode;

namespace Game.Counter.Logic {
    /// <summary>
    /// Represents a trash counter where players can drop kitchen objects.
    /// </summary>
    public class TrashCounter : BaseCounter {
        /// <summary>
        /// This event is triggered when a kitchen object is dropped in trash counter.
        /// </summary>
        public static event EventHandler OnTrash;


        /// <summary>
        /// Resets the static objects, specifically the OnTrash event.
        /// This method is used to clean up the event subscription.
        /// </summary>
        public static void ResetStaticObjects() {
            OnTrash = null;
        }


        /// <summary>
        /// Handles the player's interaction with the trash counter.
        /// If the player has a kitchen object, it will be destroyed.
        /// </summary>
        /// <param name="playerController">The player interacting with the trash counter.</param>
        public override void Interact(PlayerController playerController) {
            if (!playerController.HasKitchenObject()) return;

            playerController.GetKitchenObject().DestroySelf();
            TriggerOnTrashServerRpc();
        }

        /// <summary>
        /// Defines alternate interaction behavior. Currently, does nothing.
        /// </summary>
        public override void InteractAlternate() {
            // Do Nothing
        }


        /// <summary>
        /// Server RPC that triggers the <see cref="OnTrash" /> event for all clients.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        private void TriggerOnTrashServerRpc() {
            TriggerOnTrashClientRpc();
        }

        /// <summary>
        /// Client RPC that triggers the <see cref="OnTrash" /> event for the client.
        /// </summary>
        [ClientRpc]
        private void TriggerOnTrashClientRpc() {
            OnTrash?.Invoke(this, EventArgs.Empty);
        }
    }
}
