using System;
using UnityEngine;

namespace Player {
    public class PlayerVisual : MonoBehaviour {
        [SerializeField, Tooltip("The head mesh renderer")]
        private MeshRenderer headMeshRenderer;
        [SerializeField, Tooltip("The head body renderer")]
        private MeshRenderer bodyMeshRenderer;
        
        
        private Material _headMaterial;
        private Material _bodyMaterial;


        public void SetColor(Color color) {
            _headMaterial.color = color;
            _bodyMaterial.color = color;
        }
        

        private void Awake() {
            _headMaterial = new Material(headMeshRenderer.material);
            _bodyMaterial = new Material(bodyMeshRenderer.material);
            headMeshRenderer.material = _headMaterial;
            bodyMeshRenderer.material = _bodyMaterial;
        }
    }
}
