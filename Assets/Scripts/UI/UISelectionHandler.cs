using UnityEngine;
using UnityEngine.EventSystems;

namespace UI {
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
