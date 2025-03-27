using System;
using System.Linq;
using ScriptableObjects;
using UnityEngine;

namespace KitchenObject {
    public class PlateKitchenObjectCompleteVisual : MonoBehaviour {
        [Serializable]
        private struct KitchenObjectSOVisualPair {
            public KitchenObjectSO kitchenObjectSO;
            public GameObject visual;
        }


        [SerializeField] private PlateKitchenObject plateKitchenObject;
        [SerializeField] private KitchenObjectSOVisualPair[] kitchenObjectSOVisualPairs;


        private void Awake() {
            foreach (var kitchenObjectSOVisualPair in kitchenObjectSOVisualPairs) {
                kitchenObjectSOVisualPair.visual.SetActive(false);
            }
        }

        private void Start() {
            plateKitchenObject.OnKitchenObjectAdded += OnKitchenObjectAddedAction;
        }


        private void OnKitchenObjectAddedAction(object sender, PlateKitchenObject.OnKitchenObjectAddedArgs e) {
            foreach (var kitchenObjectSOGameObjectPair in kitchenObjectSOVisualPairs) {
                if (e.KitchenObjectSOArray.Contains(kitchenObjectSOGameObjectPair.kitchenObjectSO)) {
                    kitchenObjectSOGameObjectPair.visual.SetActive(true);
                }
            }
        }
    }
}
