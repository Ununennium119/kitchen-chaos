using UnityEngine;

namespace KitchenObject {
    public interface IKitchenObjectParent {
        /// <returns>The position in which kitchen object is placed in the scene</returns>
        public Transform GetKitchenObjectFollowTransform();

        /// <returns>Parent's kitchen object</returns>
        public KitchenObject GetKitchenObject();

        /// <summary>
        /// Sets kitchen object for the parent.
        /// </summary>
        /// <param name="kitchenObject">Kitchen object to set</param>
        public void SetKitchenObject(KitchenObject kitchenObject);

        /// <summary>
        /// Clears parent's kitchen object.
        /// </summary>
        public void ClearKitchenObject();

        /// <returns>true if the parent has kitchen object</returns>
        public bool HasKitchenObject();
    }
}
