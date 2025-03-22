using UnityEngine;

namespace KitchenObject {
    public interface IKitchenObjectParent {
        public Transform GetKitchenObjectFollowTransform();

        public KitchenObject GetKitchenObject();

        public void SetKitchenObject(KitchenObject kitchenObject);

        public void ClearKitchenObject();

        public bool HasKitchenObject();
    }
}
