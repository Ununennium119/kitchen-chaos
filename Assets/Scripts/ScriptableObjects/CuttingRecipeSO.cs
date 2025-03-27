using UnityEngine;

namespace ScriptableObjects {
    /// <summary>
    /// Contains data of a cutting recipe.
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Object/Cutting Recipe")]
    public class CuttingRecipeSO : ScriptableObject {
        /// <summary>
        /// Input of the cutting
        /// </summary>
        public KitchenObjectSO input;
        /// <summary>
        /// Output of the cutting
        /// </summary>
        public KitchenObjectSO output;
        /// <summary>
        /// Number of the cuts needed to cut input to the output
        /// </summary>
        public int totalCuts;
    }
}
