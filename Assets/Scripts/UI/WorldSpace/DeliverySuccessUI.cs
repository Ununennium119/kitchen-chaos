using System;
using Counter;
using Counter.Logic;
using UnityEngine;

namespace UI.WorldSpace {
    [RequireComponent(typeof(Animator))]
    public class DeliverySuccessUI : MonoBehaviour {
        private static readonly int ShowHide = Animator.StringToHash("ShowHide");


        [SerializeField, Tooltip("The related delivery counter")] private DeliveryCounter deliveryCounter;


        private Animator _animator;


        private void Awake() {
            _animator = GetComponent<Animator>();
        }

        private void Start() {
            deliveryCounter.OnDeliverySuccess += OnDeliverySuccessAction;
        }


        private void OnDeliverySuccessAction(object sender, EventArgs e) {
            _animator.SetTrigger(ShowHide);
        }
    }
}
