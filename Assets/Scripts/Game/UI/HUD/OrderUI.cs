using Game.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.HUD {
    /// <summary>
    /// UI component responsible for displaying a single recipe order.
    /// </summary>
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


        /// <summary>
        /// Configures the UI with data from the provided <see cref="OrderRecipeSO"/>.
        /// </summary>
        /// <param name="recipeSO">The recipe scriptable object containing name and ingredient list.</param>
        public void SetRecipeSO(OrderRecipeSO recipeSO) {
            ClearIcons();
            orderRecipeName.text = recipeSO.recipeName;
            foreach (var kitchenObjectSO in recipeSO.kitchenObjectSOList) {
                var iconTransform = Instantiate(iconTemplate, iconContainer);
                iconTransform.gameObject.SetActive(true);
                iconTransform.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
            }
        }


        /// <summary>
        /// Removes all dynamically added ingredient icons.
        /// </summary>
        private void ClearIcons() {
            foreach (Transform child in iconContainer) {
                if (child == iconTemplate) continue;
                Destroy(child.gameObject);
            }
        }
    }
}
