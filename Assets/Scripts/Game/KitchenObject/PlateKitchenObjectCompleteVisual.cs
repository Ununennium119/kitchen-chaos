using System;
using System.Linq;
using Game.ScriptableObjects;
using UnityEngine;

namespace Game.KitchenObject {
    /// <summary>
    /// Handles the visual representation of ingredients added to a plate.
    /// </summary>
    public class PlateKitchenObjectCompleteVisual : MonoBehaviour {
        /// <summary>
        /// Represents a pair mapping a kitchen object scriptable object to its corresponding visual GameObject.
        /// </summary>
        [Serializable]
        private struct KitchenObjectSOVisualPair {
            public KitchenObjectSO kitchenObjectSO;
            public GameObject visual;
        }


        [SerializeField, Tooltip("Reference to the PlateKitchenObject.")]
        private PlateKitchenObject plateKitchenObject;

        [SerializeField, Tooltip("Array of visual mappings for each kitchen object.")]
        private KitchenObjectSOVisualPair[] kitchenObjectSOVisualPairs;


        private void Awake() {
            foreach (var kitchenObjectSOVisualPair in kitchenObjectSOVisualPairs) {
                kitchenObjectSOVisualPair.visual.SetActive(false);
            }
        }

        private void Start() {
            plateKitchenObject.OnKitchenObjectAdded += OnKitchenObjectAddedAction;
        }

        /// <summary>
        /// Activates the visual GameObject associated with the added kitchen object.
        /// </summary>
        /// <remarks>
        /// Invoked when the <see cref="PlateKitchenObject.OnKitchenObjectAdded"/> event is triggered.
        /// </remarks>
        private void OnKitchenObjectAddedAction(object sender, PlateKitchenObject.OnKitchenObjectAddedArgs e) {
            foreach (var kitchenObjectSOGameObjectPair in kitchenObjectSOVisualPairs) {
                if (e.KitchenObjectSOArray.Contains(kitchenObjectSOGameObjectPair.kitchenObjectSO)) {
                    kitchenObjectSOGameObjectPair.visual.SetActive(true);
                }
            }
        }
    }
}
