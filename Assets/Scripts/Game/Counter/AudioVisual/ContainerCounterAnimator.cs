using System;
using Game.Counter.Logic;
using UnityEngine;

namespace Game.Counter.AudioVisual {
    /// <summary>
    /// Handles triggering the open/close animation for a <see cref="ContainerCounter"/> when it is opened.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class ContainerCounterAnimator : MonoBehaviour {
        private static readonly int OpenClose = Animator.StringToHash("OpenClose");


        [SerializeField, Tooltip("The container counter")]
        private ContainerCounter counter;


        private Animator _animator;


        private void Awake() {
            _animator = GetComponent<Animator>();
        }

        private void Start() {
            counter.OnContainerOpened += OnContainerOpenedAction;
        }


        /// <remarks>
        /// Invoked when the <see cref="ContainerCounter.OnContainerOpened"/> event is triggered.
        /// </remarks>
        private void OnContainerOpenedAction(object sender, EventArgs e) {
            _animator.SetBool(OpenClose, true);
        }
    }
}
