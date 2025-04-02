using Unity.Netcode;
using UnityEngine;

namespace Player {
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimator : NetworkBehaviour {
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");


        [SerializeField] private PlayerController playerController;


        private Animator _animator;


        private void Awake() {
            _animator = GetComponent<Animator>();
            _animator.SetBool(IsWalking, playerController.IsWalking());
        }

        private void Update() {
            if (!IsOwner) return;

            _animator.SetBool(IsWalking, playerController.IsWalking());
        }
    }
}
