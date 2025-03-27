using KitchenObject;
using UnityEngine;

namespace UI.WorldSpace.Plate {
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


        private void OnKitchenObjectAddedAction(object sender, PlateKitchenObject.OnKitchenObjectAddedArgs e) {
            ClearIcons();
            foreach (var kitchenObjectSO in e.KitchenObjectSOArray) {
                var iconTransform = Instantiate(iconTemplate, transform);
                iconTransform.gameObject.SetActive(true);
                iconTransform.GetComponent<PlateSingleIconUI>().SetKitchenObjectSO(kitchenObjectSO);
            }
        }

        private void ClearIcons() {
            foreach (Transform child in transform) {
                if (child == iconTemplate) continue;
                Destroy(child.gameObject);
            }
        }
    }
}
