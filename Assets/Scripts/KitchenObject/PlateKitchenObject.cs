using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using UnityEngine;

namespace KitchenObject {
    public class PlateKitchenObject : KitchenObject {
        public event EventHandler<OnKitchenObjectAddedArgs> OnKitchenObjectAdded;
        public class OnKitchenObjectAddedArgs : EventArgs {
            public KitchenObjectSO[] KitchenObjectSOArray;
        }


        [SerializeField] private KitchenObjectSO[] validKitchenObjects;


        private List<KitchenObjectSO> _kitchenObjectSOList;


        public List<KitchenObjectSO> GetKitchenObjectSOList() {
            return _kitchenObjectSOList;
        }


        private void Start() {
            _kitchenObjectSOList = new List<KitchenObjectSO>();
        }


        public bool TryAddKitchenObject(KitchenObjectSO kitchenObjectSO) {
            if (!validKitchenObjects.Contains(kitchenObjectSO)) {
                return false;
            }
            if (_kitchenObjectSOList.Contains(kitchenObjectSO)) {
                return false;
            }

            _kitchenObjectSOList.Add(kitchenObjectSO);
            OnKitchenObjectAdded?.Invoke(
                this,
                new OnKitchenObjectAddedArgs { KitchenObjectSOArray = _kitchenObjectSOList.ToArray() }
            );
            return true;
        }
    }
}
