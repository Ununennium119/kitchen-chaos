using System;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace Counter.Logic {
    public class ContainerCounter : BaseCounter {
        /// <summary>
        /// This event is invoked whenever the container gets opened by the play (by interacting with it).
        /// </summary>
        public event EventHandler OnContainerOpened;


        [SerializeField, Tooltip("Scriptable object of the kitchen object which this container has")]
        private KitchenObjectSO kitchenObjectSO;

        public override void Interact(Player.PlayerController playerController) {
            var playerKitchenObject = playerController.GetKitchenObject();

            // If player has a plate try to add counter kitchen object to the plate
            if (playerKitchenObject?.TryGetPlateKitchenObject(out var playerPlateKitchenObject) == true) {
                if (!playerPlateKitchenObject.TryAddKitchenObject(kitchenObjectSO)) {
                    return;
                }
                return;
            }

            if (playerController.HasKitchenObject()) return;

            KitchenObject.KitchenObject.SpawnKitchenObject(kitchenObjectSO, playerController);
            InvokeOnContainerOpenedServerRpc();
        }

        public override void InteractAlternate() {
            // Do Nothing
        }


        [ServerRpc(RequireOwnership = false)]
        private void InvokeOnContainerOpenedServerRpc() {
            InvokeOnContainerOpenedClientRpc();
        }

        [ClientRpc]
        private void InvokeOnContainerOpenedClientRpc() {
            OnContainerOpened?.Invoke(this, EventArgs.Empty);
        }
    }
}
