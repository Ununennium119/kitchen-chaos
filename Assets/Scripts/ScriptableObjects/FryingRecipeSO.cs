using UnityEngine;

namespace ScriptableObjects {
    /// <summary>
    /// Contains data of a frying recipe.
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Object/Frying Recipe")]
    public class FryingRecipeSO : ScriptableObject {
        /// <summary>
        /// Input of the frying (raw object)
        /// </summary>
        public KitchenObjectSO rawKitchenObjectSO;
        /// <summary>
        /// The time needed to fry from raw to fried
        /// </summary>
        public float fryingTime;
        /// <summary>
        /// Output of the frying (fried object)
        /// </summary>
        public KitchenObjectSO friedKitchenObjectSO;
        /// <summary>
        /// The time needed to fry from fried to burned
        /// </summary>
        public float burningTime;
        /// <summary>
        /// Output of the burning (burned object)
        /// </summary>
        public KitchenObjectSO burnedKitchenObjectSO;
    }
}
