using Game.KitchenObject;
using UnityEngine;

namespace Game.UI.WorldSpace.Plate {
    /// <summary>
    /// Manages the icons representing kitchen objects placed on a plate.
    /// </summary>
    public class PlateIconsUI : MonoBehaviour {
        [SerializeField, Tooltip("The plate kitchen object")]
        private PlateKitchenObject plateKitchenObject;
        [SerializeField, Tooltip("The icon template of the kitchen objects")]
        private Transform iconTemplate;


        private void Start() {
            plateKitchenObject.OnKitchenObjectAdded += OnKitchenObjectAddedAction;

            iconTemplate.gameObject.SetActive(false);
            ClearIcons();
        }


        /// <summary>
        /// Updates the UI by instantiating new icons for the kitchen objects.
        /// </summary>
        /// <remarks>
        /// Invoked when the <see cref="PlateKitchenObject.OnKitchenObjectAdded"/> event is triggered.
        /// </remarks>
        private void OnKitchenObjectAddedAction(object sender, PlateKitchenObject.OnKitchenObjectAddedArgs e) {
            ClearIcons();
            foreach (var kitchenObjectSO in e.KitchenObjectSOArray) {
                var iconTransform = Instantiate(iconTemplate, transform);
                iconTransform.gameObject.SetActive(true);
                iconTransform.GetComponent<PlateSingleIconUI>().SetKitchenObjectSO(kitchenObjectSO);
            }
        }

        /// <summary>
        /// Clears all the icons from the UI.
        /// </summary>
        private void ClearIcons() {
            foreach (Transform child in transform) {
                if (child == iconTemplate) continue;
                Destroy(child.gameObject);
            }
        }
    }
}
