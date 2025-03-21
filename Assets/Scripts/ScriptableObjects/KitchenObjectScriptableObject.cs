using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu]
    public class KitchenObjectScriptableObject : ScriptableObject {
        public Transform prefab;
        public Sprite sprite;
        public string objectName;
    }
}
