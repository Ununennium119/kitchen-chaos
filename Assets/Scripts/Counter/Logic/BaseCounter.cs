using System;
using KitchenObject;
using Player;
using Unity.Netcode;
using UnityEngine;

namespace Counter.Logic {
    public class BaseCounter : NetworkBehaviour, IKitchenObjectParent {
        [SerializeField, Tooltip("The position in which the kitchen object is placed in the scene")]
        private Transform counterTopPoint;


        private KitchenObject.KitchenObject _kitchenObject;


        /// <summary>
        /// Defines the interaction behavior of the counter when the player interacts with it.
        /// This method should be overridden in derived classes to implement specific functionality.
        /// </summary>
        /// <param name="playerController">The player object.</param>
        public virtual void Interact(PlayerController playerController) {
            throw new NotImplementedException($"{GetType().Name}BaseCounter.Interact is not implemented!");
        }

        /// <summary>
        /// Defines the interaction behavior of the counter when the player interacts alternatively with it.
        /// This method should be overridden in derived classes to implement specific functionality.
        /// </summary>
        public virtual void InteractAlternate() {
            throw new NotImplementedException($"{GetType().Name}r.Interact is not implemented!");
        }


        /// <inheritdoc cref="IKitchenObjectParent.GetKitchenObjectFollowTransform"/>
        /// <remark>Implementation of <see cref="IKitchenObjectParent.GetKitchenObjectFollowTransform"/>.</remark>
        public Transform GetKitchenObjectFollowTransform() {
            return counterTopPoint;
        }

        /// <inheritdoc cref="IKitchenObjectParent.GetKitchenObject"/>
        /// <remark>Implementation of <see cref="IKitchenObjectParent.GetKitchenObject"/>.</remark>
        public KitchenObject.KitchenObject GetKitchenObject() {
            return _kitchenObject;
        }

        /// <inheritdoc cref="IKitchenObjectParent.SetKitchenObject"/>
        /// <remark>Implementation of <see cref="IKitchenObjectParent.SetKitchenObject"/>.</remark>
        public void SetKitchenObject(KitchenObject.KitchenObject kitchenObject) {
            _kitchenObject = kitchenObject;
        }

        /// <inheritdoc cref="IKitchenObjectParent.ClearKitchenObject"/>
        /// <remark>Implementation of <see cref="IKitchenObjectParent.ClearKitchenObject"/>.</remark>
        public void ClearKitchenObject() {
            _kitchenObject = null;
        }

        /// <inheritdoc cref="IKitchenObjectParent.HasKitchenObject"/>
        /// <remark>Implementation of <see cref="IKitchenObjectParent.HasKitchenObject"/>.</remark>
        public bool HasKitchenObject() {
            return _kitchenObject is not null;
        }

        /// <inheritdoc cref="IKitchenObjectParent.GetNetworkObject"/>
        /// <remark>Implementation of <see cref="IKitchenObjectParent.GetNetworkObject"/>.</remark>
        public NetworkObjectReference GetNetworkObject() {
            return NetworkObject;
        }
    }
}
