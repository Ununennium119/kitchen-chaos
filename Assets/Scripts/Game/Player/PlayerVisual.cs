using UnityEngine;

namespace Game.Player {
    /// <summary>
    /// Manages the visual appearance of a player character.
    /// </summary>
    public class PlayerVisual : MonoBehaviour {
        [SerializeField, Tooltip("The head mesh renderer")]
        private MeshRenderer headMeshRenderer;
        [SerializeField, Tooltip("The head body renderer")]
        private MeshRenderer bodyMeshRenderer;
        
        
        private Material _headMaterial;
        private Material _bodyMaterial;


        /// <summary>
        /// Sets the color of the player's visual components.
        /// </summary>
        /// <param name="color">The color to apply to the player's head and body.</param>
        public void SetColor(Color color) {
            _headMaterial.color = color;
            _bodyMaterial.color = color;
        }
        

        /// <summary>
        /// Duplicates the materials used on the head and body to ensure each player instance
        /// has its own material and does not affect others.
        /// </summary>
        private void Awake() {
            _headMaterial = new Material(headMeshRenderer.material);
            _bodyMaterial = new Material(bodyMeshRenderer.material);
            headMeshRenderer.material = _headMaterial;
            bodyMeshRenderer.material = _bodyMaterial;
        }
    }
}
