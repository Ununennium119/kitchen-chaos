using UnityEngine;

namespace Common.Logic {
    /// <summary>
    /// Makes the game object to follow another transform's position and rotation.
    /// </summary>
    public class FollowTransform : MonoBehaviour {
        private Transform _target;


        public void SetTargetTransform(Transform target) {
            _target = target;
        }


        private void LateUpdate() {
            if (_target is null) return;

            transform.position = _target.position;
            transform.rotation = _target.rotation;
        }
    }
}
