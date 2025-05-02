using Multiplayer;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using Utility;

namespace KitchenObject {
    [RequireComponent(typeof(FollowTransform))]
    public class KitchenObject : NetworkBehaviour {
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
            MultiplayerManager.Instance.SpawnKitchenObject(kitchenObjectSO, parent);
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
            SetParentServerRpc(newParent.GetNetworkObject());
        }

        /// <summary>
        /// Clears parent of this kitchen object.
        /// </summary>
        public void ClearParent() {
            ClearParentServerRpc();
        }

        /// <summary>
        /// Removes this kitchen object from its parent and destroys itself.
        /// </summary>
        public void DestroySelf() {
            ClearParentKitchenObjectServerRpc();
            MultiplayerManager.Instance.DestroyKitchenObject(this);
        }


        [ServerRpc(RequireOwnership = false)]
        private void SetParentServerRpc(NetworkObjectReference newParentNetworkObjectReference) {
            SetParentClientRpc(newParentNetworkObjectReference);
        }

        [ClientRpc]
        private void SetParentClientRpc(NetworkObjectReference newParentNetworkObjectReference) {
            newParentNetworkObjectReference.TryGet(out var newParentNetworkObject);
            var newParent = newParentNetworkObject.GetComponent<IKitchenObjectParent>();
            if (newParent.HasKitchenObject()) {
                Debug.LogError("Trying to set kitchen object for a parent which already has one!");
            }
            newParent.SetKitchenObject(this);
            var followTransform = GetComponent<FollowTransform>();
            followTransform.SetTargetTransform(newParent.GetKitchenObjectFollowTransform());
            transform.localPosition = Vector3.zero;
            _parent = newParent;
        }

        [ServerRpc(RequireOwnership = false)]
        private void ClearParentServerRpc() {
            ClearParentClientRpc();
        }

        [ClientRpc]
        private void ClearParentClientRpc() {
            _parent?.ClearKitchenObject();
            _parent = null;
        }

        [ServerRpc(RequireOwnership = false)]
        private void ClearParentKitchenObjectServerRpc() {
            ClearParentKitchenObjectClientRpc();
        }

        [ClientRpc]
        private void ClearParentKitchenObjectClientRpc() {
            _parent?.ClearKitchenObject();
        }
    }
}
