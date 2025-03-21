using System;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Counter {
    public class ContainerCounter : BaseCounter {
        public event EventHandler OnContainerOpened;


        [SerializeField] private KitchenObjectScriptableObject kitchenObjectScriptableObject;

        public override void Interact(Player player) {
            if (player.HasKitchenObject()) return;

            KitchenObject.SpawnKitchenObject(kitchenObjectScriptableObject, player);

            OnContainerOpened?.Invoke(this, EventArgs.Empty);
        }

        public override void InteractAlternate() {
            // Do Nothing
        }
    }
}
