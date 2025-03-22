using ScriptableObjects;
using UnityEngine;

namespace KitchenObject {
    public class KitchenObject : MonoBehaviour {
        [SerializeField] private KitchenObjectSO kitchenObjectSO;


        private IKitchenObjectParent _parent;


        public static void SpawnKitchenObject(
            KitchenObjectSO kitchenObjectSO,
            IKitchenObjectParent parent
        ) {
            var kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
            kitchenObjectTransform.GetComponent<KitchenObject>().SetParent(parent);
        }


        public KitchenObjectSO GetKitchenObjectSO() {
            return kitchenObjectSO;
        }

        public IKitchenObjectParent GetParent() {
            return _parent;
        }

        public void SetParent(IKitchenObjectParent newParent) {
            if (newParent.HasKitchenObject()) {
                Debug.LogError("Trying to set kitchen object for a prent which already has one!");
            }
            newParent.SetKitchenObject(this);
            transform.parent = newParent.GetKitchenObjectFollowTransform();
            transform.localPosition = Vector3.zero;
            _parent = newParent;
        }

        public void ClearParent() {
            _parent?.ClearKitchenObject();
            _parent = null;
        }

        public void DestroySelf() {
            _parent?.ClearKitchenObject();
            Destroy(gameObject);
        }
    }
}
