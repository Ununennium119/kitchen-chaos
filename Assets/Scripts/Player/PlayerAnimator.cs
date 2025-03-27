using UnityEngine;

namespace Player {
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimator : MonoBehaviour {
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");


        private Animator _animator;
        private PlayerController _playerController;


        private void Awake() {
            _animator = GetComponent<Animator>();
            _animator.SetBool(IsWalking, _playerController.IsWalking());
        }

        private void Start() {
            _playerController = PlayerController.Instance;
        }

        private void Update() {
            _animator.SetBool(IsWalking, _playerController.IsWalking());
        }
    }
}
