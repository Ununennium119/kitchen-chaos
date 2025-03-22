namespace Counter {
    public class DeliveryCounter : BaseCounter {
        public override void Interact(Player.Player player) {
            var playerKitchenObject = player.GetKitchenObject();
            if (playerKitchenObject?.TryGetPlateKitchenObject(out var plateKitchenObject) == true) {
                plateKitchenObject.DestroySelf();
            }
        }

        public override void InteractAlternate() {
            // Do Nothing
        }
    }
}
