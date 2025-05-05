using System;
using System.Collections.Generic;
using System.Linq;
using Game.ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace Game.KitchenObject {
    /// <summary>
    /// Represents a plate that can hold other valid kitchen objects.
    /// </summary>
    public class PlateKitchenObject : KitchenObject {
        /// <summary>
        /// This event is triggered whenever a kitchen object is added to the plate.
        /// </summary>
        public event EventHandler<OnKitchenObjectAddedArgs> OnKitchenObjectAdded;
        public class OnKitchenObjectAddedArgs : EventArgs {
            public KitchenObjectSO[] KitchenObjectSOArray;
        }


        [SerializeField, Tooltip("Scriptable object of the kitchen object which can be added to the plate")]
        private KitchenObjectSO[] validKitchenObjects;

        [SerializeField, Tooltip("Scriptable object of the kitchen object list")]
        private KitchenObjectListSO kitchenObjectListSO;


        private readonly List<KitchenObjectSO> _kitchenObjectSOList = new();


        /// <returns>List of scriptable object of the kitchen objects this plate contains</returns>
        public List<KitchenObjectSO> GetKitchenObjectSOList() {
            return _kitchenObjectSOList;
        }


        // --- SERVER LOGIC ---

        /// <summary>
        /// Tries to add the kitchen object to the plate.
        /// </summary>
        /// <param name="kitchenObjectSO">Scriptable object of the kitchen object to add</param>
        /// <returns>true if kitchen object is added</returns>
        /// <remarks>
        /// Should only be called in the server.
        /// </remarks>
        public bool TryAddKitchenObject(KitchenObjectSO kitchenObjectSO) {
            if (!validKitchenObjects.Contains(kitchenObjectSO)) return false;
            if (_kitchenObjectSOList.Contains(kitchenObjectSO)) return false;

            // Update clients
            var kitchenObjectSOIndex = kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
            AddKitchenObjectSOClientRpc(kitchenObjectSOIndex);

            return true;
        }


        // --- CLIENT LOGIC ---

        /// <summary>
        /// Client RPC that adds a kitchen object to the plate for the client.
        /// </summary>
        /// <param name="kitchenObjectSOIndex">The index of the kitchen object in the master list.</param>
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
