namespace Counter {
    public class TrashCounter : BaseCounter {
        public override void Interact(Player.Player player) {
            if (player.HasKitchenObject()) {
                player.GetKitchenObject().DestroySelf();
            }
        }

        public override void InteractAlternate() {
            // Do Nothing
        }
    }
}
