using System;

namespace Counter {
    public class TrashCounter : BaseCounter {
        public static event EventHandler OnTrash;


        public static void ResetStaticObjects() {
            OnTrash = null;
        }


        public override void Interact(Player.Player player) {
            if (!player.HasKitchenObject()) return;

            player.GetKitchenObject().DestroySelf();
            OnTrash?.Invoke(this, null);
        }

        public override void InteractAlternate() {
            // Do Nothing
        }
    }
}
