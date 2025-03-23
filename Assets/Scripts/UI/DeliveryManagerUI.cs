using System.Linq;
using UnityEngine;

namespace UI {
    public class DeliveryManagerUI : MonoBehaviour {
        [SerializeField] private Transform orderContainer;
        [SerializeField] private Transform orderTemplate;
        

        private readonly GameManager.State[] _activeStates = { GameManager.State.Playing, GameManager.State.Countdown };
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
            _gameManager.OnStateChanged += OnStateChangedAction;
        }


        private void OnOrderSpawnedAction(object sender, System.EventArgs e) {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            // The order spawned event is triggered in a specific condition so it doesn't affect performance
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
                // The order spawned event is triggered in a specific condition so it doesn't affect performance
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                var orderUI = orderTransform.GetComponent<OrderUI>();
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                orderUI.SetRecipeSO(waitingOrder);
            }
        }


        private void OnStateChangedAction(object sender, GameManager.OnStateChangedArgs e) {
            var isActive = _activeStates.Contains(e.State);
            gameObject.SetActive(isActive);
        }
    }
}
