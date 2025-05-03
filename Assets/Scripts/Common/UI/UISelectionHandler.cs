using UnityEngine;
using UnityEngine.EventSystems;

namespace Common.UI {
    /// <summary>
    /// Handles the UI selection behavior by ensuring that a fallback selectable game object is selected when no other object is selected.
    /// </summary>
    public class UISelectionHandler : MonoBehaviour {
        [SerializeField, Tooltip("The object to select when no other object is selected")]
        private GameObject fallbackSelectable;


        private void Update() {
            if (!EventSystem.current.currentSelectedGameObject) {
                if (fallbackSelectable && fallbackSelectable.activeInHierarchy) {
                    EventSystem.current.SetSelectedGameObject(fallbackSelectable);
                }
            }
        }
    }
}
