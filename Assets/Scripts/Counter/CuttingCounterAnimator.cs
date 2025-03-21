using System;
using UnityEngine;

namespace Counter {
    public class CuttingCounterAnimator : MonoBehaviour {
        private static readonly int Cut = Animator.StringToHash("Cut");


        [SerializeField] private CuttingCounter counter;

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
