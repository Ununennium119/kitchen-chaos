using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects {
    /// <summary>
    /// Contains data of a order recipe.
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Object/Order Recipe")]
    public class OrderRecipeSO : ScriptableObject {
        /// <summary>
        /// Name of the recipe.
        /// </summary>
        public string recipeName;
        /// <summary>
        /// List of the scriptable objects of the kitchen objects in the order.
        /// </summary>
        public List<KitchenObjectSO> kitchenObjectSOList;
    }
}
