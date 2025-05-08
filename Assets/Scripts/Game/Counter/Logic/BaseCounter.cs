using System;
using Game.KitchenObject;
using Game.Player;
using Unity.Netcode;
using UnityEngine;

namespace Game.Counter.Logic {
    /// <summary>
    /// Base class for all counters.
    /// </summary>
    public class BaseCounter : NetworkBehaviour, IKitchenObjectParent {
        [SerializeField, Tooltip("The position in which the kitchen object is placed in the scene")]
        private Transform counterTopPoint;


        /// <remarks>
        /// This field is only updated in the server.
        /// </remarks>
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


        /// <inheritdoc/>
        /// <remark>Implementation of <see cref="IKitchenObjectParent.GetKitchenObjectFollowTransform"/>.</remark>
        public Transform GetKitchenObjectFollowTransform() {
            return counterTopPoint;
        }

        /// <inheritdoc/>
        /// <remark>Implementation of <see cref="IKitchenObjectParent.GetKitchenObject"/>.</remark>
        public KitchenObject.KitchenObject GetKitchenObject() {
            return _kitchenObject;
        }

        /// <inheritdoc/>
        /// <remark>Implementation of <see cref="IKitchenObjectParent.SetKitchenObject"/>.</remark>
        public void SetKitchenObject(KitchenObject.KitchenObject kitchenObject) {
            _kitchenObject = kitchenObject;
        }

        /// <inheritdoc/>
        /// <remark>Implementation of <see cref="IKitchenObjectParent.ClearKitchenObject"/>.</remark>
        public void ClearKitchenObject() {
            _kitchenObject = null;
        }

        /// <inheritdoc/>
        /// <remark>Implementation of <see cref="IKitchenObjectParent.HasKitchenObject"/>.</remark>
        public bool HasKitchenObject() {
            return _kitchenObject is not null;
        }

        /// <inheritdoc/>
        /// <remark>Implementation of <see cref="IKitchenObjectParent.GetNetworkObject"/>.</remark>
        public NetworkObjectReference GetNetworkObject() {
            return NetworkObject;
        }
    }
}
