using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.KitchenObject;
using Game.ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using Logger = Common.Utility.Logger;
using Random = UnityEngine.Random;

namespace Game.Manager {
    /// <summary>This class is responsible for managing orders and delivering plates.</summary>
    /// <remarks>This class is singleton</remarks>
    public class DeliveryManager : NetworkBehaviour {
        public static DeliveryManager Instance { get; private set; }


        /// <summary>
        /// This event is triggered whenever a new order is spawned (created).
        /// </summary>
        public event EventHandler OnOrderSpawned;

        /// <summary>
        /// This event is triggered whenever a new order is despawned.
        /// </summary>
        public event EventHandler OnOrderDeSpawned;

        /// <summary>
        /// This event is triggered whenever a plate is delivered successfully.
        /// </summary>
        public event EventHandler OnDeliverySuccess;

        /// <summary>
        /// This event is triggered whenever player tries to deliver a non-matching plate.
        /// </summary>
        public event EventHandler OnDeliveryFail;


        [SerializeField, Tooltip("The scriptable object of order recipe list (used to create order randomly)")]
        private OrderRecipeListSO orderRecipeListSO;
        [SerializeField, Tooltip("The duration between spawning orders")]
        private float orderSpawnCooldown = 10f;
        [SerializeField, Tooltip("Maximum number of order which can be at the same time")]
        private int maxOrdersCount = 4;


        private GameManager _gameManager;
        private readonly List<OrderRecipeSO> _waitingOrderRecipeSOList = new();
        private int _deliveredOrdersCount;
        private bool _isDeliveryActive;


        /// <summary>
        /// Tries to match the plate with an order and deliver it.
        /// </summary>
        /// <param name="plateKitchenObject">The plate kitchen object</param>
        /// <returns>true if plate is delivered successfully</returns>
        /// <remarks>
        /// Should only be called from server.
        /// </remarks>
        public bool DeliverPlate(PlateKitchenObject plateKitchenObject) {
            OrderRecipeSO deliveredWaitingOrderRecipeSO = null;
            var deliveredWaitingOrderIndex = -1;
            var plateKitchenObjectSOList = plateKitchenObject.GetKitchenObjectSOList();
            foreach (
                var (waitingOrderRecipeSO, index) in _waitingOrderRecipeSOList.Select(
                    (value, index) => (value, index)
                )
            ) {
                var waitingOrderKitchenObjectSOList = waitingOrderRecipeSO.kitchenObjectSOList;
                if (waitingOrderKitchenObjectSOList.Count != plateKitchenObjectSOList.Count) continue;

                var doesWaitingOrderMatchesPlate = waitingOrderKitchenObjectSOList.TrueForAll(
                    waitingOrderKitchenObjectSO => plateKitchenObjectSOList.Contains(waitingOrderKitchenObjectSO)
                );
                if (!doesWaitingOrderMatchesPlate) continue;

                deliveredWaitingOrderRecipeSO = waitingOrderRecipeSO;
                deliveredWaitingOrderIndex = index;
                break;
            }
            if (deliveredWaitingOrderRecipeSO == null) {
                // Update clients
                FailedDeliveryClientRpc();

                return false;
            }

            // Update clients
            SuccessfullyDeliveryClientRpc(deliveredWaitingOrderIndex);

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
            Logger.LogInitializingInstance(this);
            if (Instance != null) {
                Logger.LogMultipleInstancesError(this);
                Destroy(gameObject);
                return;
            }
            Instance = this;
            Logger.LogInstanceInitialized(this);
        }

        private void Start() {
            _gameManager = GameManager.Instance;

            _gameManager.OnStateChanged += OnGameStateChangedAction;
        }

        public override void OnNetworkSpawn() {
            if (IsServer) {
                StartCoroutine(OrderSpawnCoroutine());
            }
        }


        /// <summary>
        /// Coroutine that periodically spawns new orders if conditions allow for all clients.
        /// </summary>
        private IEnumerator OrderSpawnCoroutine() {
            while (true) {
                yield return new WaitForSeconds(orderSpawnCooldown);

                if (!_isDeliveryActive) continue;
                if (_waitingOrderRecipeSOList.Count >= maxOrdersCount) continue;

                var orderRecipeSOIndex = Random.Range(0, orderRecipeListSO.orderRecipeSOList.Count);
                SpawnOrderClientRpc(orderRecipeSOIndex);
            }
            // ReSharper disable once IteratorNeverReturns
        }


        /// <summary>
        /// Reacts to game state changes to enable or disable delivery flow.
        /// </summary>
        /// <remarks>
        /// Invoked when the <see cref="GameManager.OnStateChanged"/> event is triggered.
        /// </remarks>
        private void OnGameStateChangedAction(object sender, GameManager.OnStateChangedArgs e) {
            _isDeliveryActive = e.State == GameManager.State.Playing;
        }


        /// <summary>
        /// Client RPC that notifies handles a successful delivery for the client.
        /// </summary>
        /// <param name="orderIndex">Index of the successfully delivered order.</param>
        [ClientRpc]
        private void SuccessfullyDeliveryClientRpc(int orderIndex) {
            _deliveredOrdersCount += 1;
            _waitingOrderRecipeSOList.RemoveAt(orderIndex);
            OnOrderDeSpawned?.Invoke(this, EventArgs.Empty);
            OnDeliverySuccess?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Client RPC that notifies handles a failed delivery for the client.
        /// </summary>
        [ClientRpc]
        private void FailedDeliveryClientRpc() {
            OnDeliveryFail?.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        /// Client RPC that spawns a new order for the client.
        /// </summary>
        /// <param name="orderIndex">Index of the order recipe to spawn.</param>
        [ClientRpc]
        private void SpawnOrderClientRpc(int orderIndex) {
            var newOrderRecipeSO = orderRecipeListSO.orderRecipeSOList[orderIndex];
            _waitingOrderRecipeSOList.Add(newOrderRecipeSO);
            OnOrderSpawned?.Invoke(this, EventArgs.Empty);
        }
    }
}
