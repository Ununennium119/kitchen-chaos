using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu]
    public class OrderRecipeSO : ScriptableObject {
        public string recipeName;
        public List<KitchenObjectSO> kitchenObjectSOList;
    }
}
