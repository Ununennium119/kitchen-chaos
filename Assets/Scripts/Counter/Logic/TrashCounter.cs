using System;
using Player;

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
            OnTrash?.Invoke(this, EventArgs.Empty);
        }

        public override void InteractAlternate() {
            // Do Nothing
        }
    }
}
