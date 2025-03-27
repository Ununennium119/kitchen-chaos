using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WorldSpace.Plate {
    public class PlateSingleIconUI : MonoBehaviour {
        [SerializeField] private Image icon;


        public void SetKitchenObjectSO(KitchenObjectSO kitchenObjectSO) {
            icon.sprite = kitchenObjectSO.sprite;
        }
    }
}
