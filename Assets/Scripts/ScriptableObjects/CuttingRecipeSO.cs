using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu]
    public class CuttingRecipeSO : ScriptableObject {
        public KitchenObjectSO input;
        public KitchenObjectSO output;
        public int totalCuts;
    }
}
