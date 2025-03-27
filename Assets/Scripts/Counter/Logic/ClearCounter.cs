using KitchenObject;

namespace Counter.Logic {
    public class ClearCounter : BaseCounter {
        public override void Interact(Player.PlayerController playerController) {
            var playerKitchenObject = playerController.GetKitchenObject();
            var counterKitchenObject = GetKitchenObject();

            // Handle plate logic
            PlateKitchenObject playerPlateKitchenObject = null;
            PlateKitchenObject counterPlateKitchenObject = null;
            var playerHasPlate = playerKitchenObject?.TryGetPlateKitchenObject(out playerPlateKitchenObject);
            var counterHasPlate = counterKitchenObject?.TryGetPlateKitchenObject(out counterPlateKitchenObject);
            // If both counter and player have plates swap them
            if (!(playerHasPlate == true && counterHasPlate == true)) {
                // If player has a plate and counter is not empty try to move counter kitchen object to the plate.
                if (playerHasPlate == true) {
                    if (counterKitchenObject != null) {
                        if (!playerPlateKitchenObject.TryAddKitchenObject(counterKitchenObject.GetKitchenObjectSO())) {
                            return;
                        }
                        counterKitchenObject.DestroySelf();
                        return;
                    }
                }
                // If counter has a plate and player has a kitchen object try to move player kitchen object to the plate.
                if (counterHasPlate == true) {
                    if (playerKitchenObject != null) {
                        if (!counterPlateKitchenObject.TryAddKitchenObject(playerKitchenObject.GetKitchenObjectSO())) {
                            return;
                        }
                        playerKitchenObject.DestroySelf();
                        return;
                    }
                }
            }

            // Otherwise swap kitchen objects of the player and the counter 
            playerKitchenObject?.ClearParent();
            counterKitchenObject?.ClearParent();
            playerKitchenObject?.SetParent(this);
            counterKitchenObject?.SetParent(playerController);
        }

        public override void InteractAlternate() {
            // Do Nothing
        }
    }
}
