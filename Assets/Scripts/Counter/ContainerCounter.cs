using ScriptableObjects.KitchenObjects;
using UnityEngine;

namespace Counter {
    public class ContainerCounter : BaseCounter {
        [SerializeField] private KitchenObjectScriptable kitchenObjectScriptable;

        public override void Interact(Player player) {
            if (player.HasKitchenObject()) return;

            var kitchenObjectTransform = Instantiate(kitchenObjectScriptable.prefab);
            kitchenObjectTransform.GetComponent<KitchenObject>().SetParent(player);
        }
    }
}
