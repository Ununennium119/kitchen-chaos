using System;
using Game.Player;
using Game.ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace Game.Counter.Logic {
    /// <summary>
    /// Represents a counter that contains a kitchen object that the player can get.
    /// </summary>
    public class ContainerCounter : BaseCounter {
        /// <summary>
        /// This event is triggered whenever the container gets opened by the play (by interacting with it).
        /// </summary>
        public event EventHandler OnContainerOpened;


        [SerializeField, Tooltip("Scriptable object of the kitchen object which this container has")]
        private KitchenObjectSO kitchenObjectSO;


        /// <summary>
        /// Handles the interaction between the player and the counter.
        /// If the player has a plate, the kitchen object is added to it. 
        /// Otherwise, if the player doesn't have a kitchen object, a kitchen object will be added to player.
        /// </summary>
        /// <param name="playerController">The player interacting with the counter.</param>
        public override void Interact(PlayerController playerController) {
            var playerKitchenObject = playerController.GetKitchenObject();

            // If player has a plate try to add counter kitchen object to the plate
            if (playerKitchenObject?.TryGetPlateKitchenObject(out var playerPlateKitchenObject) == true) {
                if (!playerPlateKitchenObject.TryAddKitchenObject(kitchenObjectSO)) {
                    return;
                }
                return;
            }

            // If player already has a kitchen object do nothing
            if (playerController.HasKitchenObject()) return;

            // Add kitchen object to the player
            KitchenObject.KitchenObject.SpawnKitchenObject(kitchenObjectSO, playerController);
            InvokeOnContainerOpenedServerRpc();
        }

        /// <summary>
        /// Defines alternate interaction behavior. Currently, does nothing.
        /// </summary>
        public override void InteractAlternate() {
            // Do Nothing
        }


        /// <summary>
        /// Server RPC that triggers the <see cref="OnContainerOpened"/> event for all clients.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        private void InvokeOnContainerOpenedServerRpc() {
            InvokeOnContainerOpenedClientRpc();
        }

        /// <summary>
        /// Client RPC that triggers the <see cref="OnContainerOpened"/> event on the client.
        /// </summary>
        [ClientRpc]
        private void InvokeOnContainerOpenedClientRpc() {
            OnContainerOpened?.Invoke(this, EventArgs.Empty);
        }
    }
}
