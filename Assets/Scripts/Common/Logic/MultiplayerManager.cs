using System;
using System.Collections.Generic;
using Game.KitchenObject;
using Game.Player;
using Game.ScriptableObjects;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

namespace Common.Logic {
    /// <summary>This class is responsible for handling multiplayer logic like spawning and syncing.</summary>
    /// <remarks>This class is singleton.</remarks>
    public class MultiplayerManager : NetworkBehaviour {
        public const int MAX_PLAYER_COUNT = 4;


        public static MultiplayerManager Instance { get; private set; }


        public event EventHandler OnTryingToJoin;
        public event EventHandler OnFailedToJoin;
        public event EventHandler OnPlayerDataListChanged;


        [SerializeField, Tooltip("List of kitchen object scriptable objects")]
        private KitchenObjectListSO kitchenObjectListSO;

        [SerializeField, Tooltip("List of player colors")]
        private List<Color> playerColors = new();


        private readonly NetworkList<PlayerData> _playerDataList = new();


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
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallbackAction;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallbackAction;
            NetworkManager.Singleton.StartClient();
        }

        public void StartHost() {
            NetworkManager.Singleton.ConnectionApprovalCallback = ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += HostOnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += HostOnClientDisconnectCallbackAction;
            NetworkManager.Singleton.StartHost();
        }

        /// <summary>
        /// Removes the kitchen object from its parent and destroys itself by calling a server RPC.
        /// </summary>
        /// <param name="kitchenObject">The kitchen object to destroy</param>
        public void DestroyKitchenObject(KitchenObject kitchenObject) {
            DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
        }


        public bool HasPlayerData(int index) {
            return index >= 0 && _playerDataList.Count > index;
        }

        public PlayerData GetPlayerData(int index) {
            return _playerDataList[index];
        }

        public PlayerData GetPlayerData(ulong clientId) {
            foreach (var playerData in _playerDataList) {
                if (playerData.ClientId == clientId) {
                    return playerData;
                }
            }
            return default;
        }

        public int GetPlayerDataIndex(ulong clientId) {
            for (var i = 0; i < _playerDataList.Count; i++) {
                if (_playerDataList[i].ClientId == clientId) {
                    return i;
                }
            }
            return -1;
        }

        public PlayerData GetLocalPlayerData() {
            return GetPlayerData(NetworkManager.Singleton.LocalClientId);
        }


        public Color GetPlayerColor(int colorIndex) {
            return playerColors[colorIndex];
        }

        public void ChangePlayerColor(int colorIndex) {
            ChangePlayerColorServerRpc(colorIndex, new ServerRpcParams());
        }


        public void KickPlayer(ulong clientId) {
            NetworkManager.Singleton.DisconnectClient(clientId);
            HostOnClientDisconnectCallbackAction(clientId);
        }


        private void Awake() {
            Debug.Log("Setting up MultiplayerManager");
            if (Instance != null) {
                Debug.LogError("There is more than one instance of MultiplayerManager!");
            }
            Instance = this;

            _playerDataList.OnListChanged += OnPlayerDataListChangedAction;

            DontDestroyOnLoad(gameObject);
        }


        private int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO) {
            return kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
        }

        private KitchenObjectSO GetKitchenObjectSO(int kitchenObjectSOIndex) {
            return kitchenObjectListSO.kitchenObjectSOList[kitchenObjectSOIndex];
        }


        private bool IsColorAvailable(int colorIndex) {
            foreach (var playerData in _playerDataList) {
                if (playerData.ColorIndex == colorIndex) {
                    return false;
                }
            }
            return true;
        }

        private int GetFirstAvailableColor() {
            for (var i = 0; i < playerColors.Count; i++) {
                if (IsColorAvailable(i)) {
                    return i;
                }
            }
            return -1;
        }


        [ServerRpc(RequireOwnership = false)]
        private void SpawnKitchenObjectServerRpc(int index, NetworkObjectReference parentNetworkObjectReference) {
            parentNetworkObjectReference.TryGet(out var parentNetworkObject);
            var parent = parentNetworkObject.GetComponent<IKitchenObjectParent>();

            var kitchenObjectSO = GetKitchenObjectSO(index);
            var kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
            var kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
            kitchenObjectNetworkObject.Spawn();

            kitchenObjectTransform.GetComponent<KitchenObject>().SetParent(parent);
        }

        [ServerRpc(RequireOwnership = false)]
        private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference) {
            kitchenObjectNetworkObjectReference.TryGet(out var kitchenObjectNetworkObject);
            kitchenObjectNetworkObject.Despawn();
        }


        [ServerRpc(RequireOwnership = false)]
        private void ChangePlayerColorServerRpc(int colorIndex, ServerRpcParams serverRpcParams) {
            if (!IsColorAvailable(colorIndex)) return;

            var playerDataIndex = GetPlayerDataIndex(serverRpcParams.Receive.SenderClientId);
            var playerData = _playerDataList[playerDataIndex];
            playerData.ColorIndex = colorIndex;
            _playerDataList[playerDataIndex] = playerData;
        }

        [ServerRpc(RequireOwnership = false)]
        private void ChangePlayerNameAndIdServerRpc(string playerName, string playerId, ServerRpcParams serverRpcParams) {
            var playerDataIndex = GetPlayerDataIndex(serverRpcParams.Receive.SenderClientId);
            var playerData = _playerDataList[playerDataIndex];
            playerData.Name = playerName;
            playerData.PlayerId = playerId;
            _playerDataList[playerDataIndex] = playerData;
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

        private void OnClientConnectedCallbackAction(ulong clientId) {
            ChangePlayerNameAndIdServerRpc(
                PlayerPrefsManager.GetPlayerName(),
                AuthenticationService.Instance.PlayerId,
                new ServerRpcParams()
            );
        }

        private void OnClientDisconnectCallbackAction(ulong clientId) {
            OnFailedToJoin?.Invoke(this, EventArgs.Empty);
        }

        private void HostOnClientConnectedCallback(ulong clientId) {
            _playerDataList.Add(
                new PlayerData {
                    ClientId = clientId,
                    ColorIndex = GetFirstAvailableColor()
                }
            );
            if (clientId == NetworkManager.Singleton.LocalClientId) {
                ChangePlayerNameAndIdServerRpc(
                    PlayerPrefsManager.GetPlayerName(),
                    AuthenticationService.Instance.PlayerId,
                    new ServerRpcParams()
                );
            }
        }

        private void HostOnClientDisconnectCallbackAction(ulong clientId) {
            var playerDataIndex = GetPlayerDataIndex(clientId);
            if (playerDataIndex == -1) return;
            _playerDataList.RemoveAt(playerDataIndex);
        }


        private void OnPlayerDataListChangedAction(NetworkListEvent<PlayerData> changeEvent) {
            OnPlayerDataListChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
