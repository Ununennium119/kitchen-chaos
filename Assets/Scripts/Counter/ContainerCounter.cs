using System;
using ScriptableObjects;
using UnityEngine;

namespace Counter {
    public class ContainerCounter : BaseCounter {
        public event EventHandler OnContainerOpened;


        [SerializeField] private KitchenObjectSO kitchenObjectSO;

        public override void Interact(Player.Player player) {
            if (player.HasKitchenObject()) return;

            KitchenObject.KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);

            OnContainerOpened?.Invoke(this, EventArgs.Empty);
        }

        public override void InteractAlternate() {
            // Do Nothing
        }
    }
}
