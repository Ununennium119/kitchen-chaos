using Game.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.WorldSpace.Plate {
    /// <summary>
    /// Represents a single icon in the plate UI that displays a kitchen object.
    /// </summary>
    public class PlateSingleIconUI : MonoBehaviour {
        [SerializeField, Tooltip("The plate item icon")] private Image icon;


        /// <summary>
        /// Sets the kitchen object sprite on the icon image.
        /// </summary>
        /// <param name="kitchenObjectSO">The kitchen object ScriptableObject containing the sprite for the icon.</param>
        public void SetKitchenObjectSO(KitchenObjectSO kitchenObjectSO) {
            icon.sprite = kitchenObjectSO.sprite;
        }
    }
}
