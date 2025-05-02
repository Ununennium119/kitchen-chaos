using UnityEngine;
using UnityEngine.EventSystems;

namespace Common.UI {
    public class UISelectionHandler : MonoBehaviour {
        [SerializeField] private GameObject fallbackSelectable;

        private void Update() {
            if (!EventSystem.current.currentSelectedGameObject) {
                if (fallbackSelectable && fallbackSelectable.activeInHierarchy) {
                    EventSystem.current.SetSelectedGameObject(fallbackSelectable);
                }
            }
        }
    }
}
