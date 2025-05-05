using Common.Logic;
using Game.ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace Game.KitchenObject {
    /// <summary>
    /// Represents a kitchen object.
    /// </summary>
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
        /// <remarks>
        /// Should only be called from server.
        /// </remarks>
        public static void SpawnKitchenObject(
            KitchenObjectSO kitchenObjectSO,
            IKitchenObjectParent parent
        ) {
            var kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
            var kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
            kitchenObjectNetworkObject.Spawn();

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

        /// <summary>
        /// Sets the parent of this kitchen object.
        /// </summary>
        /// <param name="newParent">The new parent</param>
        /// <remarks>
        /// Should only be called from server.
        /// </remarks>
        public void SetParent(IKitchenObjectParent newParent) {
            if (newParent.HasKitchenObject()) {
                Debug.Log($"Parent {nameof(newParent)} already has kitchenObject.");
                return;
            }

            newParent.SetKitchenObject(this);
            var followTransform = GetComponent<FollowTransform>();
            followTransform.SetTargetTransform(newParent.GetKitchenObjectFollowTransform());
            transform.localPosition = Vector3.zero;
            _parent = newParent;

            // Update clients
            SetParentClientRpc(newParent.GetNetworkObject());
        }

        /// <summary>
        /// Clears parent of this kitchen object.
        /// </summary>
        /// <remarks>
        /// Should only be called from server.
        /// </remarks>
        public void ClearParent() {
            _parent?.ClearKitchenObject();
            _parent = null;

            // Update clients
            ClearParentClientRpc();
        }

        /// <summary>
        /// Removes this kitchen object from its parent and destroys itself.
        /// </summary>
        /// <remarks>
        /// Should only be called from server.
        /// </remarks>
        public void DestroySelf() {
            _parent?.ClearKitchenObject();

            // Update clients
            ClearParentKitchenObjectClientRpc();

            NetworkObject.Despawn();
        }


        /// <summary>
        /// Client RPC that sets parent of the game object for the client.
        /// </summary>
        /// <param name="newParentNetworkObjectReference">Network object reference of the new parent</param>
        [ClientRpc]
        private void SetParentClientRpc(NetworkObjectReference newParentNetworkObjectReference) {
            if (IsServer) return;

            newParentNetworkObjectReference.TryGet(out var newParentNetworkObject);
            var newParent = newParentNetworkObject.GetComponent<IKitchenObjectParent>();

            newParent.SetKitchenObject(this);
            var followTransform = GetComponent<FollowTransform>();
            followTransform.SetTargetTransform(newParent.GetKitchenObjectFollowTransform());
            transform.localPosition = Vector3.zero;
            _parent = newParent;
        }

        /// <summary>
        /// Client RPC that clears parent of the game object for the client.
        /// </summary>
        [ClientRpc]
        private void ClearParentClientRpc() {
            if (IsServer) return;

            _parent?.ClearKitchenObject();
            _parent = null;
        }

        /// <summary>
        /// Client RPC that clears kitchen object of the parent for this client.
        /// </summary>
        [ClientRpc]
        private void ClearParentKitchenObjectClientRpc() {
            if (IsServer) return;

            _parent?.ClearKitchenObject();
        }
    }
}
