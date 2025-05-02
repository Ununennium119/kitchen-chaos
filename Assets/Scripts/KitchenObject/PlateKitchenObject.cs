using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace KitchenObject {
    public class PlateKitchenObject : KitchenObject {
        /// <summary>
        /// This event is invoked whenever a kitchen object is added to the plate.
        /// </summary>
        public event EventHandler<OnKitchenObjectAddedArgs> OnKitchenObjectAdded;
        public class OnKitchenObjectAddedArgs : EventArgs {
            public KitchenObjectSO[] KitchenObjectSOArray;
        }


        [SerializeField, Tooltip("Scriptable object of the kitchen object which can be added to the plate")]
        private KitchenObjectSO[] validKitchenObjects;

        [SerializeField, Tooltip("Scriptable object of the kitchen object list")]
        private KitchenObjectListSO kitchenObjectListSO;


        private List<KitchenObjectSO> _kitchenObjectSOList;


        /// <returns>List of scriptable object of the kitchen objects this plate contains</returns>
        public List<KitchenObjectSO> GetKitchenObjectSOList() {
            return _kitchenObjectSOList;
        }

        /// <summary>
        /// Tries to add the kitchen object to the plate.
        /// </summary>
        /// <param name="kitchenObjectSO">Scriptable object of the kitchen object to add</param>
        /// <returns>true if kitchen object is added</returns>
        public bool TryAddKitchenObject(KitchenObjectSO kitchenObjectSO) {
            if (!validKitchenObjects.Contains(kitchenObjectSO)) return false;
            if (_kitchenObjectSOList.Contains(kitchenObjectSO)) return false;

            var kitchenObjectSOIndex = kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
            AddKitchenObjectSOServerRpc(kitchenObjectSOIndex);
            return true;
        }


        private void Start() {
            _kitchenObjectSOList = new List<KitchenObjectSO>();
        }


        [ServerRpc(RequireOwnership = false)]
        private void AddKitchenObjectSOServerRpc(int kitchenObjectSOIndex) {
            AddKitchenObjectSOClientRpc(kitchenObjectSOIndex);
        }

        [ClientRpc]
        private void AddKitchenObjectSOClientRpc(int kitchenObjectSOIndex) {
            var kitchenObjectSO = kitchenObjectListSO.kitchenObjectSOList[kitchenObjectSOIndex];
            _kitchenObjectSOList.Add(kitchenObjectSO);
            OnKitchenObjectAdded?.Invoke(
                this,
                new OnKitchenObjectAddedArgs { KitchenObjectSOArray = _kitchenObjectSOList.ToArray() }
            );
        }
    }
}
