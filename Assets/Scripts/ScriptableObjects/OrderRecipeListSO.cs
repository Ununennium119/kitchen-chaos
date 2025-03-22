using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu]
    public class OrderRecipeListSO : ScriptableObject {
        public List<OrderRecipeSO> orderRecipeSOList;
    }
}
