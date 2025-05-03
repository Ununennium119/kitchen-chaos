using System;
using Game.Counter.Logic;
using UnityEngine;

namespace Game.Counter.AudioVisual {
    /// <summary>
    /// Plays a cutting animation when the associated <see cref="CuttingCounter"/> triggers a cut action.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class CuttingCounterAnimator : MonoBehaviour {
        private static readonly int Cut = Animator.StringToHash("Cut");


        [SerializeField, Tooltip("The cutting counter")]
        private CuttingCounter counter;


        private Animator _animator;


        private void Awake() {
            _animator = GetComponent<Animator>();
        }

        private void Start() {
            counter.OnCut += OnCutAction;
        }


        /// <remarks>
        /// Invoked when the <see cref="CuttingCounter.OnCut"/> event is triggered.
        /// </remarks>
        private void OnCutAction(object sender, EventArgs e) {
            _animator.SetBool(Cut, true);
        }
    }
}
