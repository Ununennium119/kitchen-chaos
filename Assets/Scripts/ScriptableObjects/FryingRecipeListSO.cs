using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects {
    /// <summary>
    /// Contains list of frying recipes scriptable objects.
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Object/Frying Recipe List")]
    public class FryingRecipeListSO : ScriptableObject {
        /// <summary>
        /// List of frying recipes scriptable objects
        /// </summary>
        public List<FryingRecipeSO> fryingRecipeSOList;
    }
}
