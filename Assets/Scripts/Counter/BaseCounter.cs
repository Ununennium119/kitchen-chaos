using System;
using KitchenObject;
using UnityEngine;

namespace Counter {
    public class BaseCounter : MonoBehaviour, IKitchenObjectParent {
        [SerializeField] private Transform counterTopPoint;

        private KitchenObject.KitchenObject _kitchenObject;


        public virtual void Interact(Player.Player player) {
            throw new NotImplementedException("BaseCounter.Interact is not implemented!");
        }
        
        public virtual void InteractAlternate() {
            throw new NotImplementedException("BaseCounter.Interact is not implemented!");
        }


        public Transform GetKitchenObjectFollowTransform() {
            return counterTopPoint;
        }

        public KitchenObject.KitchenObject GetKitchenObject() {
            return _kitchenObject;
        }

        public void SetKitchenObject(KitchenObject.KitchenObject kitchenObject) {
            _kitchenObject = kitchenObject;
        }

        public void ClearKitchenObject() {
            _kitchenObject = null;
        }

        public bool HasKitchenObject() {
            return _kitchenObject != null;
        }
    }
}
