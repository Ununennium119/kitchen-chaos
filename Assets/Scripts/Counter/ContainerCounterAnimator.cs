using System;
using UnityEngine;

namespace Counter {
    public class ContainerCounterAnimator : MonoBehaviour {
        private static readonly int OpenClose = Animator.StringToHash("OpenClose");


        [SerializeField] private ContainerCounter counter;

        private Animator _animator;


        private void Awake() {
            _animator = GetComponent<Animator>();
        }

        private void Start() {
            counter.OnContainerOpened += OnContainerOpenedAction;
        }


        private void OnContainerOpenedAction(object sender, EventArgs e) {
            _animator.SetBool(OpenClose, true);
        }
    }
}
