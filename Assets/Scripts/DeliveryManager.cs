using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using KitchenObject;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryManager : MonoBehaviour {
    public static DeliveryManager Instance { get; private set; }


    public event EventHandler OnOrderSpawned; 
    public event EventHandler OnOrderDeSpawned; 
    public event EventHandler OnDeliverySuccess; 
    public event EventHandler OnDeliveryFail; 


    [SerializeField] private OrderRecipeListSO orderRecipeListSO;
    [SerializeField] private float maxOrderRecipeSpawnTimer;
    [SerializeField] private int maxOrderRecipesCount;


    private GameManager _gameManager;
    private List<OrderRecipeSO> _waitingOrderRecipeSOList;
    private float _orderRecipeSpawnTimer;
    private int _deliveredRecipesCount;
    private bool _isDeliveryActive;


    public bool DeliverPlate(PlateKitchenObject plateKitchenObject) {
        OrderRecipeSO deliveredWaitingOrderRecipeSO = null;
        var plateKitchenObjectSOList = plateKitchenObject.GetKitchenObjectSOList();
        foreach (var waitingOrderRecipeSO in _waitingOrderRecipeSOList) {
            var waitingOrderKitchenObjectSOList = waitingOrderRecipeSO.kitchenObjectSOList;
            if (waitingOrderKitchenObjectSOList.Count != plateKitchenObjectSOList.Count) continue;

            var isWaitingOrderFound = waitingOrderKitchenObjectSOList.TrueForAll(
                waitingOrderKitchenObjectSO => plateKitchenObjectSOList.Contains(waitingOrderKitchenObjectSO)
            );
            if (!isWaitingOrderFound) continue;

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
        _deliveredRecipesCount += 1;
        return true;
    }

    public List<OrderRecipeSO> GetWaitingOrderRecipeSOList() {
        return _waitingOrderRecipeSOList;
    }

    public int GetDeliveredRecipesCount() {
        return _deliveredRecipesCount;
    }


    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There is more than one DeliveryManager in the scene!");
        }
        Instance = this;

        _waitingOrderRecipeSOList = new List<OrderRecipeSO>();
        _orderRecipeSpawnTimer = maxOrderRecipeSpawnTimer;
        _deliveredRecipesCount = 0;
    }

    private void Start() {
        _gameManager = GameManager.Instance;

        _gameManager.OnStateChanged += OnStateChangedAction;
    }

    private void Update() {
        if (!_isDeliveryActive) return;
        if (_waitingOrderRecipeSOList.Count >= maxOrderRecipesCount) return;

        _orderRecipeSpawnTimer -= Time.deltaTime;
        if (!(_orderRecipeSpawnTimer <= 0)) return;

        var orderRecipeSOIndex = Random.Range(0, orderRecipeListSO.orderRecipeSOList.Count);
        var newOrderRecipeSO = orderRecipeListSO.orderRecipeSOList[orderRecipeSOIndex];
        _waitingOrderRecipeSOList.Add(newOrderRecipeSO);
        _orderRecipeSpawnTimer = maxOrderRecipeSpawnTimer;
        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        // The order spawned event is triggered in a specific condition so it doesn't affect performance
        OnOrderSpawned?.Invoke(this, EventArgs.Empty);
    }


    private void OnStateChangedAction(object sender, GameManager.OnStateChangedArgs e) {
        _isDeliveryActive = e.State == GameManager.State.Playing;
    }
}
