using Game.Player;
using Game.ScriptableObjects;
using UnityEngine;

namespace Game.Counter.Logic {
    /// <summary>
    /// Represents a counter that provides a plate to the player when interacted with.
    /// </summary>
    public class PlateCounter : BaseCounter {
        [SerializeField, Tooltip("Scriptable object of the plate")]
        private KitchenObjectSO plateKitchenObjectSO;


        /// <summary>
        /// Handles the player's interaction with the plate counter.
        /// If the player doesn't have a kitchen object, a plate is spawned for them.
        /// </summary>
        /// <param name="playerController">The player interacting with the counter.</param>
        /// <remarks>
        /// Should only be called from server.
        /// </remarks>
        public override void Interact(PlayerController playerController) {
            if (!playerController.HasKitchenObject()) {
                KitchenObject.KitchenObject.SpawnKitchenObject(
                    plateKitchenObjectSO,
                    playerController
                );
            }
        }

        /// <summary>
        /// Defines alternate interaction behavior. Currently, does nothing.
        /// </summary>
        public override void InteractAlternate() {
            // Do Nothing
        }
    }
}
