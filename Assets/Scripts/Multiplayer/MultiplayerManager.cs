using System;
using System.Threading;
using KitchenObject;
using Manager;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace Multiplayer {
    /// <summary>This class is responsible for handling multiplayer logic like spawning and syncing.</summary>
    /// <remarks>This class is singleton.</remarks>
    public class MultiplayerManager : NetworkBehaviour {
        private const int MAX_PLAYER_COUNT = 4;


        public static MultiplayerManager Instance { get; private set; }


        public event EventHandler OnTryingToJoin;
        public event EventHandler OnFailedToJoin;


        [SerializeField, Tooltip("List of kitchen object scriptable objects")]
        private KitchenObjectListSO kitchenObjectListSO;


        private GameManager _gameManager;


        /// <summary>
        /// Spawns and adds a kitchen object to the parent by calling a server RPC.
        /// </summary>
        /// <param name="kitchenObjectSO">Scriptable object of the kitchen object</param>
        /// <param name="parent">The parent to add kitchen object to</param>
        public void SpawnKitchenObject(
            KitchenObjectSO kitchenObjectSO,
            IKitchenObjectParent parent
        ) {
            var index = GetKitchenObjectSOIndex(kitchenObjectSO);
            SpawnKitchenObjectServerRpc(index, parent.GetNetworkObject());
        }

        public void StartServer() {
            NetworkManager.Singleton.ConnectionApprovalCallback = ConnectionApprovalCallback;
            NetworkManager.Singleton.StartServer();
        }

        public void StartClient() {
            OnTryingToJoin?.Invoke(this, EventArgs.Empty);
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallbackAction;
            NetworkManager.Singleton.StartClient();
        }

        public void StartHost() {
            NetworkManager.Singleton.ConnectionApprovalCallback = ConnectionApprovalCallback;
            NetworkManager.Singleton.StartHost();
        }

        /// <summary>
        /// Removes the kitchen object from its parent and destroys itself by calling a server RPC.
        /// </summary>
        /// <param name="kitchenObject">The kitchen object to destroy</param>
        public void DestroyKitchenObject(KitchenObject.KitchenObject kitchenObject) {
            DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
        }


        private void Awake() {
            Debug.Log("Setting up MultiplayerManager");
            if (Instance != null) {
                Debug.LogError("There is more than one instance of MultiplayerManager!");
            }
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        private void Start() {
            _gameManager = GameManager.Instance;
        }


        private int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO) {
            return kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
        }

        private KitchenObjectSO GetKitchenObjectSO(int kitchenObjectSOIndex) {
            return kitchenObjectListSO.kitchenObjectSOList[kitchenObjectSOIndex];
        }


        [ServerRpc(RequireOwnership = false)]
        private void SpawnKitchenObjectServerRpc(int index, NetworkObjectReference parentNetworkObjectReference) {
            parentNetworkObjectReference.TryGet(out var parentNetworkObject);
            var parent = parentNetworkObject.GetComponent<IKitchenObjectParent>();

            var kitchenObjectSO = GetKitchenObjectSO(index);
            var kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
            var kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
            kitchenObjectNetworkObject.Spawn();

            kitchenObjectTransform.GetComponent<KitchenObject.KitchenObject>().SetParent(parent);
        }

        [ServerRpc(RequireOwnership = false)]
        private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference) {
            kitchenObjectNetworkObjectReference.TryGet(out var kitchenObjectNetworkObject);
            kitchenObjectNetworkObject.Despawn();
        }


        private void ConnectionApprovalCallback(
            NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response
        ) {
            if (!SceneLoader.IsSceneActive(SceneLoader.Scene.CharacterSelectScene)) {
                response.Approved = false;
                response.Reason = "Game started already!";
                return;
            }
            if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_COUNT) {
                response.Approved = false;
                response.Reason = "Game is full!";
                return;
            }
            response.Approved = true;
        }


        private void OnClientDisconnectCallbackAction(ulong clientId) {
            OnFailedToJoin?.Invoke(this, EventArgs.Empty);
        }
    }
}
