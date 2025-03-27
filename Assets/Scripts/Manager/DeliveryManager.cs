using System;
using System.Collections;
using System.Collections.Generic;
using KitchenObject;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Manager {
    /// <summary>This class is responsible for managing orders and delivering plates.</summary>
    /// <remarks>This class is singleton</remarks>
    public class DeliveryManager : MonoBehaviour {
        public static DeliveryManager Instance { get; private set; }


        /// <summary>
        /// This event is invoked whenever a new order is spawned (created).
        /// </summary>
        public event EventHandler OnOrderSpawned;

        /// <summary>
        /// This event is invoked whenever a new order is despawned.
        /// </summary>
        public event EventHandler OnOrderDeSpawned;

        /// <summary>
        /// This event is invoked whenever a plate is delivered successfully.
        /// </summary>
        public event EventHandler OnDeliverySuccess;

        /// <summary>
        /// This event is invoked whenever player tries to deliver a non-matching plate.
        /// </summary>
        public event EventHandler OnDeliveryFail;


        [SerializeField, Tooltip("The scriptable object of order recipe list (used to create order randomly)")]
        private OrderRecipeListSO orderRecipeListSO;
        [SerializeField, Tooltip("The duration between spawning orders")]
        private float orderSpawnCooldown = 10f;
        [SerializeField, Tooltip("Maximum number of order which can be at the same time")]
        private int maxOrdersCount = 4;


        private GameManager _gameManager;
        private List<OrderRecipeSO> _waitingOrderRecipeSOList;
        private int _deliveredOrdersCount;
        private bool _isDeliveryActive;


        /// <summary>
        /// Tries to match the plate with an order and deliver it.
        /// </summary>
        /// <param name="plateKitchenObject">The plate kitchen object</param>
        /// <returns>true if plate is delivered successfully</returns>
        public bool DeliverPlate(PlateKitchenObject plateKitchenObject) {
            OrderRecipeSO deliveredWaitingOrderRecipeSO = null;
            var plateKitchenObjectSOList = plateKitchenObject.GetKitchenObjectSOList();
            foreach (var waitingOrderRecipeSO in _waitingOrderRecipeSOList) {
                var waitingOrderKitchenObjectSOList = waitingOrderRecipeSO.kitchenObjectSOList;
                if (waitingOrderKitchenObjectSOList.Count != plateKitchenObjectSOList.Count) continue;

                var doesWaitingOrderMatchesPlate = waitingOrderKitchenObjectSOList.TrueForAll(
                    waitingOrderKitchenObjectSO => plateKitchenObjectSOList.Contains(waitingOrderKitchenObjectSO)
                );
                if (!doesWaitingOrderMatchesPlate) continue;

                deliveredWaitingOrderRecipeSO = waitingOrderRecipeSO;
                break;
            }
            if (deliveredWaitingOrderRecipeSO == null) {
                OnDeliveryFail?.Invoke(this, EventArgs.Empty);
                return false;
            }

            _waitingOrderRecipeSOList.Remove(deliveredWaitingOrderRecipeSO);
            OnOrderDeSpawned?.Invoke(this, EventArgs.Empty);
            OnDeliverySuccess?.Invoke(this, EventArgs.Empty);
            _deliveredOrdersCount += 1;
            return true;
        }

        /// <returns>List of scriptable objects of the waiting order recipes.</returns>
        public List<OrderRecipeSO> GetWaitingOrderRecipeSOList() {
            return _waitingOrderRecipeSOList;
        }

        /// <returns>Number of delivered orders</returns>
        public int GetDeliveredOrdersCount() {
            return _deliveredOrdersCount;
        }


        private void Awake() {
            Debug.Log("Setting up DeliveryManager...");
            if (Instance != null) {
                Debug.LogError("There is more than one DeliveryManager in the scene!");
            }
            Instance = this;

            _waitingOrderRecipeSOList = new List<OrderRecipeSO>();
            _deliveredOrdersCount = 0;
        }

        private void Start() {
            _gameManager = GameManager.Instance;

            _gameManager.OnStateChanged += OnGameStateChangedAction;

            StartCoroutine(OrderSpawnCoroutine());
        }


        private IEnumerator OrderSpawnCoroutine() {
            while (true) {
                yield return new WaitForSeconds(orderSpawnCooldown);

                if (!_isDeliveryActive) continue;
                if (_waitingOrderRecipeSOList.Count >= maxOrdersCount) continue;

                var orderRecipeSOIndex = Random.Range(0, orderRecipeListSO.orderRecipeSOList.Count);
                var newOrderRecipeSO = orderRecipeListSO.orderRecipeSOList[orderRecipeSOIndex];
                _waitingOrderRecipeSOList.Add(newOrderRecipeSO);
                OnOrderSpawned?.Invoke(this, EventArgs.Empty);
            }
            // ReSharper disable once IteratorNeverReturns
        }


        private void OnGameStateChangedAction(object sender, GameManager.OnStateChangedArgs e) {
            _isDeliveryActive = e.State == GameManager.State.Playing;
        }
    }
}
