using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects {
    /// <summary>
    /// Contains list of order recipes scriptable objects.
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Object/Order Recipe List")]
    public class OrderRecipeListSO : ScriptableObject {
        /// <summary>
        /// List of order recipes scriptable objects
        /// </summary>
        public List<OrderRecipeSO> orderRecipeSOList;
    }
}
