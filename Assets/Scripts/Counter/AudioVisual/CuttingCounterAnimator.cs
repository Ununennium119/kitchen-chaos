using System;
using Counter.Logic;
using UnityEngine;

namespace Counter.AudioVisual {
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


        private void OnCutAction(object sender, EventArgs e) {
            _animator.SetBool(Cut, true);
        }
    }
}
