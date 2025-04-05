using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects {
    /// <summary>
    /// Contains list of kitchen object scriptable objects.
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Object/Kitchen Object List")]
    public class KitchenObjectListSO : ScriptableObject {
        /// <summary>
        /// List of kitchen object scriptable objects.
        /// </summary>
        public List<KitchenObjectSO> kitchenObjectSOList;
    }
}
