using System;
using ScriptableObjects.KitchenObjects;
using UnityEngine;

namespace Counter {
    public class ContainerCounter : BaseCounter {
        public event EventHandler OnContainerOpened;
        
        
        [SerializeField] private KitchenObjectScriptable kitchenObjectScriptable;

        public override void Interact(Player player) {
            if (player.HasKitchenObject()) return;

            var kitchenObjectTransform = Instantiate(kitchenObjectScriptable.prefab);
            kitchenObjectTransform.GetComponent<KitchenObject>().SetParent(player);
            OnContainerOpened?.Invoke(this, EventArgs.Empty);
        }
    }
}
