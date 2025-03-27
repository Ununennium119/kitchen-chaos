using UnityEngine;

namespace ScriptableObjects {
    /// <summary>
    /// Contains data of a kitchen object
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Object/Kitchen Object")]
    public class KitchenObjectSO : ScriptableObject {
        /// <summary>
        /// Prefab of the kitchen object
        /// </summary>
        public Transform prefab;
        /// <summary>
        /// Sprite of the kitchen object
        /// </summary>
        public Sprite sprite;
        /// <summary>
        /// Name of the kitchen object
        /// </summary>
        public string objectName;
    }
}
