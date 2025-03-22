namespace Counter {
    public class ClearCounter : BaseCounter {
        public override void Interact(Player.Player player) {
            var playerKitchenObject = player.GetKitchenObject();
            var counterKitchenObject = GetKitchenObject();

            // Handle plate logic
            // If player has a plate and counter is not empty try to add counter kitchen object to the plate
            if (playerKitchenObject?.TryGetPlateKitchenObject(out var playerPlateKitchenObject) == true) {
                if (counterKitchenObject != null) {
                    if (playerPlateKitchenObject.TryAddKitchenObject(counterKitchenObject.GetKitchenObjectSO())) {
                        counterKitchenObject.DestroySelf();
                    }
                    return;
                }
            }
            // If counter has a plate and player has a kitchen object try to add player kitchen object to the plate
            if (counterKitchenObject?.TryGetPlateKitchenObject(out var counterPlateKitchenObject) == true) {
                if (playerKitchenObject != null) {
                    if (counterPlateKitchenObject.TryAddKitchenObject(playerKitchenObject.GetKitchenObjectSO())) {
                        playerKitchenObject.DestroySelf();
                    }
                    return;
                }
            }


            // Otherwise swap player and counter kitchen objects
            playerKitchenObject?.ClearParent();
            counterKitchenObject?.ClearParent();
            if (playerKitchenObject != null) {
                playerKitchenObject.SetParent(this);
            }
            if (counterKitchenObject != null) {
                counterKitchenObject.SetParent(player);
            }
        }

        public override void InteractAlternate() {
            // Do Nothing
        }
    }
}
