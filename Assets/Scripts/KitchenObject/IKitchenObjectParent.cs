using UnityEngine;

namespace KitchenObject {
    public interface IKitchenObjectParent {
        public Transform GetKitchenObjectFollowTransform();

        public global::KitchenObject.KitchenObject GetKitchenObject();

        public void SetKitchenObject(global::KitchenObject.KitchenObject kitchenObject);

        public void ClearKitchenObject();

        public bool HasKitchenObject();
    }
}
