using UnityEngine;

namespace ScriptableObjects.KitchenObjects {
    [CreateAssetMenu]
    public class KitchenObjectScriptable : ScriptableObject {
        public Transform prefab;
        public Sprite sprite;
        public string objectName;
    }
}
