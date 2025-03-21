using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu]
    public class CuttingRecipeScriptableObject : ScriptableObject {
        public KitchenObjectScriptableObject input;
        public KitchenObjectScriptableObject output;
    }
}
