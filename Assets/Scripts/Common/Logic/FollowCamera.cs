using System;
using UnityEngine;

namespace Common.Logic {
    /// <summary>
    /// Makes the attached object follow the camera in various ways based on the selected mode.
    /// </summary>
    public class FollowCamera : MonoBehaviour {
        /// <summary>
        /// Specifies the different modes of how the object follows the camera.
        /// </summary>
        private enum Mode {
            /// <summary>
            /// The object rotates to always look at the camera.
            /// </summary>
            LookAt,
            
            /// <summary>
            /// The object rotates to look in the opposite direction of the camera.
            /// </summary>
            LookAtInverse,
            
            /// <summary>
            /// The object faces the same direction as the camera.
            /// </summary>
            Forward,
            
            /// <summary>
            /// The object faces the opposite direction of the camera.
            /// </summary>
            ForwardInverse
        }


        [SerializeField, Tooltip("Specifies how the object follows camera")]
        private Mode mode = Mode.LookAt;


        private void Update() {
            var mainCamera = Camera.main;
            if (mainCamera is null) return;

            switch (mode) {
                case Mode.LookAt:
                    transform.LookAt(mainCamera.transform);
                    break;
                case Mode.LookAtInverse:
                    var directionFromCamera = transform.position - mainCamera.transform.position;
                    transform.LookAt(transform.position + directionFromCamera);
                    break;
                case Mode.Forward:
                    transform.forward = mainCamera.transform.forward;
                    break;
                case Mode.ForwardInverse:
                    transform.forward = -mainCamera.transform.forward;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
