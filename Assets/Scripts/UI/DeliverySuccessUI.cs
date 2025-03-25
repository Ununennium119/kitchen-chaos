using System;
using Counter;
using UnityEngine;

namespace UI {
    public class DeliverySuccessUI : MonoBehaviour {
        private static readonly int ShowHide = Animator.StringToHash("ShowHide");


        [SerializeField] private DeliveryCounter deliveryCounter;


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
