using System;
using Counter.Logic;
using UnityEngine;

namespace Counter.AudioVisual {
    [RequireComponent(typeof(Animator))]
    public class ContainerCounterAnimator : MonoBehaviour {
        private static readonly int OpenClose = Animator.StringToHash("OpenClose");


        [SerializeField, Tooltip("The container counter")] private ContainerCounter counter;


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
