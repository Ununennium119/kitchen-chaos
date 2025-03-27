using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD {
    public class OrderUI : MonoBehaviour {
        [SerializeField, Tooltip("Order recipe name text")]
        private TextMeshProUGUI orderRecipeName;
        [SerializeField, Tooltip("The object containing icons")]
        private Transform iconContainer;
        [SerializeField, Tooltip("The icon template")]
        private Transform iconTemplate;


        private void Awake() {
            iconTemplate.gameObject.SetActive(false);
            ClearIcons();
        }


        public void SetRecipeSO(OrderRecipeSO recipeSO) {
            ClearIcons();
            orderRecipeName.text = recipeSO.recipeName;
            foreach (var kitchenObjectSO in recipeSO.kitchenObjectSOList) {
                var iconTransform = Instantiate(iconTemplate, iconContainer);
                iconTransform.gameObject.SetActive(true);
                iconTransform.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
            }
        }


        private void ClearIcons() {
            foreach (Transform child in iconContainer) {
                if (child == iconTemplate) continue;
                Destroy(child.gameObject);
            }
        }
    }
}
