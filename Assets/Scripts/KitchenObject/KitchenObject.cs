using ScriptableObjects;
using UnityEngine;

namespace KitchenObject {
    public class KitchenObject : MonoBehaviour {
        [SerializeField, Tooltip("Scriptable object of the kitchen object")]
        private KitchenObjectSO kitchenObjectSO;


        private IKitchenObjectParent _parent;


        /// <summary>
        /// Spawns and adds a kitchen object to the parent.
        /// </summary>
        /// <param name="kitchenObjectSO">Scriptable object of the kitchen object</param>
        /// <param name="parent">The parent to add kitchen object to</param>
        public static void SpawnKitchenObject(
            KitchenObjectSO kitchenObjectSO,
            IKitchenObjectParent parent
        ) {
            var kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
            kitchenObjectTransform.GetComponent<KitchenObject>().SetParent(parent);
        }


        /// <summary>
        /// Tries to cast this kitchen object to plate.
        /// </summary>
        /// <param name="plate">This kitchen object as a plate if it is a plate; otherwise, null.</param>
        /// <returns>true if this kitchen object is a plate</returns>
        public bool TryGetPlateKitchenObject(out PlateKitchenObject plate) {
            if (this is PlateKitchenObject plateKitchenObject) {
                plate = plateKitchenObject;
                return true;
            }

            plate = null;
            return false;
        }

        /// <returns>Scriptable object of this kitchen object</returns>
        public KitchenObjectSO GetKitchenObjectSO() {
            return kitchenObjectSO;
        }

        /// <returns>Parent of this kitchen object</returns>
        public IKitchenObjectParent GetParent() {
            return _parent;
        }

        /// <summary>
        /// Sets the parent of this kitchen object.
        /// </summary>
        /// <param name="newParent">The new parent</param>
        public void SetParent(IKitchenObjectParent newParent) {
            if (newParent.HasKitchenObject()) {
                Debug.LogError("Trying to set kitchen object for a prent which already has one!");
            }
            newParent.SetKitchenObject(this);
            transform.parent = newParent.GetKitchenObjectFollowTransform();
            transform.localPosition = Vector3.zero;
            _parent = newParent;
        }

        /// <summary>
        /// Clears parent of this kitchen object.
        /// </summary>
        public void ClearParent() {
            _parent?.ClearKitchenObject();
            _parent = null;
        }

        /// <summary>
        /// Removes this kitchen object from its parent and destroys itself.
        /// </summary>
        public void DestroySelf() {
            _parent?.ClearKitchenObject();
            Destroy(gameObject);
        }
    }
}
