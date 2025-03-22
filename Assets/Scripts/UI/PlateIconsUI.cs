using KitchenObject;
using UnityEngine;

namespace UI {
    public class PlateIconsUI : MonoBehaviour {
        [SerializeField] private PlateKitchenObject plateKitchenObject;
        [SerializeField] private Transform iconTemplate;


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
                Debug.Log(iconTransform.GetComponent<PlateSingleIconUI>());
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
