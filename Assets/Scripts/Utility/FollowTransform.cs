using UnityEngine;

namespace Utility {
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
