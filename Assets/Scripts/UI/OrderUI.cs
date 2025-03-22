using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class OrderUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI orderRecipeName;
        [SerializeField] private Transform iconContainer;
        [SerializeField] private Transform iconTemplate;


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
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                // The  order spawned is triggered in a specific condition so it doesn't affect performance
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
