using System;
using System.Linq;
using Game.Manager;
using UnityEngine;

namespace Game.UI.HUD {
    /// <summary>
    /// Manages the UI that displays the list of active recipe orders.
    /// </summary>
    public class OrderListUI : MonoBehaviour {
        [SerializeField, Tooltip("The object containing the orders")]
        private Transform orderContainer;
        [SerializeField, Tooltip("The order template")]
        private Transform orderTemplate;


        private GameManager _gameManager;
        private DeliveryManager _deliveryManager;


        private void Awake() {
            orderTemplate.gameObject.SetActive(false);
            ClearOrders();
        }

        private void Start() {
            _deliveryManager = DeliveryManager.Instance;
            _gameManager = GameManager.Instance;

            _deliveryManager.OnOrderSpawned += OnOrderSpawnedAction;
            _deliveryManager.OnOrderDeSpawned += OnOrderDeSpawnedAction;
            _gameManager.OnStateChanged += OnGameStateChangedAction;
        }


        /// <summary>
        /// Refreshes the list of orders.
        /// </summary>
        /// <remarks>
        /// Invoked when the <see cref="DeliveryManager.OnOrderSpawned"/> event is triggered.
        /// </remarks>
        private void OnOrderSpawnedAction(object sender, EventArgs e) {
            UpdateOrders();
        }

        /// <summary>
        /// Refreshes the list of orders.
        /// </summary>
        /// <remarks>
        /// Invoked when the <see cref="DeliveryManager.OnOrderDeSpawned"/> event is triggered.
        /// </remarks>
        private void OnOrderDeSpawnedAction(object sender, EventArgs e) {
            UpdateOrders();
        }


        /// <summary>
        /// Destroys all current order UI elements except for the template.
        /// </summary>
        private void ClearOrders() {
            foreach (Transform child in orderContainer) {
                if (child == orderTemplate) continue;
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Updates the UI list to match the current set of active orders.
        /// </summary>
        private void UpdateOrders() {
            ClearOrders();

            var waitingOrders = _deliveryManager.GetWaitingOrderRecipeSOList();
            foreach (var waitingOrder in waitingOrders) {
                var orderTransform = Instantiate(orderTemplate, orderContainer);
                orderTransform.gameObject.SetActive(true);
                var orderUI = orderTransform.GetComponent<OrderUI>();
                orderUI.SetRecipeSO(waitingOrder);
            }
        }


        /// <summary>
        /// Toggles the visibility of the order UI based on the current game state.
        /// </summary>
        /// <remarks>
        /// Invoked when the <see cref="GameManager.OnStateChanged"/> event is triggered.
        /// </remarks>
        private void OnGameStateChangedAction(object sender, GameManager.OnStateChangedArgs e) {
            var isActive = new[] { GameManager.State.Playing, GameManager.State.Countdown }.Contains(e.State);
            gameObject.SetActive(isActive);
        }
    }
}
