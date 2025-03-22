using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu]
    public class KitchenObjectSO : ScriptableObject {
        public Transform prefab;
        public Sprite sprite;
        public string objectName;
    }
}
