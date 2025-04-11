using System;
using Player;
using Unity.Netcode;

namespace Counter.Logic {
    public class TrashCounter : BaseCounter {
        /// <summary>
        /// This event is invoked when a kitchen object is dropped in trash counter.
        /// </summary>
        public static event EventHandler OnTrash;


        public static void ResetStaticObjects() {
            OnTrash = null;
        }


        public override void Interact(PlayerController playerController) {
            if (!playerController.HasKitchenObject()) return;

            playerController.GetKitchenObject().DestroySelf();
            InvokeOnTrashServerRpc();
        }

        public override void InteractAlternate() {
            // Do Nothing
        }


        [ServerRpc(RequireOwnership = false)]
        private void InvokeOnTrashServerRpc() {
            InvokeOnTrashClientRpc();
        }

        [ClientRpc]
        private void InvokeOnTrashClientRpc() {
            OnTrash?.Invoke(this, EventArgs.Empty);
        }
    }
}
