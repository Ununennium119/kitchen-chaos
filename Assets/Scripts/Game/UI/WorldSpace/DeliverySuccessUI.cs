using System;
using Game.Counter.Logic;
using UnityEngine;

namespace Game.UI.WorldSpace {
    /// <summary>
    /// This class manages the UI that appears when a delivery is successful.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class DeliverySuccessUI : MonoBehaviour {
        private static readonly int ShowHide = Animator.StringToHash("ShowHide");


        [SerializeField, Tooltip("The related delivery counter")]
        private DeliveryCounter deliveryCounter;


        private Animator _animator;


        private void Awake() {
            _animator = GetComponent<Animator>();
        }

        private void Start() {
            deliveryCounter.OnDeliverySuccess += OnDeliverySuccessAction;
        }


        /// <remarks>
        /// Invoked when the <see cref="DeliveryCounter.OnDeliverySuccess"/> event is triggered.
        /// </remarks>
        private void OnDeliverySuccessAction(object sender, EventArgs e) {
            _animator.SetTrigger(ShowHide);
        }
    }
}
