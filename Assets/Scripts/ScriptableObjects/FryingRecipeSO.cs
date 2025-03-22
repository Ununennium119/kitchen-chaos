using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu]
    public class FryingRecipeSO : ScriptableObject {
        public KitchenObjectSO rawKitchenObjectSO;
        public float fryingTime;
        public KitchenObjectSO friedKitchenObjectSO;
        public float burningTime;
        public KitchenObjectSO burnedKitchenObjectSO;
    }
}
