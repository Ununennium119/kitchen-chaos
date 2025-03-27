using System.Linq;
using Manager;
using UnityEngine;

namespace UI.HUD {
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


        private void OnOrderSpawnedAction(object sender, System.EventArgs e) {
            UpdateOrders();
        }

        private void OnOrderDeSpawnedAction(object sender, System.EventArgs e) {
            UpdateOrders();
        }


        private void ClearOrders() {
            foreach (Transform child in orderContainer) {
                if (child == orderTemplate) continue;
                Destroy(child.gameObject);
            }
        }

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


        private void OnGameStateChangedAction(object sender, GameManager.OnStateChangedArgs e) {
            var isActive = new[] { GameManager.State.Playing, GameManager.State.Countdown }.Contains(e.State);
            gameObject.SetActive(isActive);
        }
    }
}
