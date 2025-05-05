using System;
using System.Collections.Generic;
using Common.Utility;
using Game.Player;
using Game.ScriptableObjects;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using Logger = Common.Utility.Logger;

namespace Common.Logic {
    /// <summary>This class is responsible for handling multiplayer logic like spawning and syncing.</summary>
    /// <remarks>This class is singleton.</remarks>
    public class MultiplayerManager : NetworkBehaviour {
        /// <summary>
        /// Maximum number of players allowed in the game.
        /// </summary>
        public const int MAX_PLAYER_COUNT = 4;


        public static MultiplayerManager Instance { get; private set; }


        /// <summary>
        /// This event is triggered whenever a player is trying to join the game.
        /// </summary>
        public event EventHandler OnTryingToJoin;

        /// <summary>
        /// This event is triggered whenever a player fails to join the game.
        /// </summary>
        public event EventHandler OnFailedToJoin;

        /// <summary>
        /// This event is triggered whenever the list of player data changes.
        /// </summary>
        public event EventHandler OnPlayerDataListChanged;


        [SerializeField, Tooltip("List of kitchen object scriptable objects")]
        private KitchenObjectListSO kitchenObjectListSO;

        [SerializeField, Tooltip("List of player colors")]
        private List<Color> playerColors = new();


        private readonly NetworkList<PlayerData> _playerDataList = new();


        /// <summary>
        /// Starts the client and attempts to join the server.
        /// </summary>
        public void StartClient() {
            OnTryingToJoin?.Invoke(this, EventArgs.Empty);
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallbackAction;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallbackAction;
            NetworkManager.Singleton.StartClient();
        }

        /// <summary>
        /// Starts as a host, acting both as a server and a client.
        /// </summary>
        public void StartHost() {
            NetworkManager.Singleton.ConnectionApprovalCallback = ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += HostOnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += HostOnClientDisconnectCallbackAction;
            NetworkManager.Singleton.StartHost();
        }


        /// <summary>
        /// Checks if player data exists at the given index.
        /// </summary>
        /// <param name="index">The index of the player data to check.</param>
        /// <returns><c>true</c> if player data exists at the index; otherwise, <c>false</c>.</returns>
        public bool HasPlayerData(int index) {
            return index >= 0 && _playerDataList.Count > index;
        }

        /// <summary>
        /// Retrieves the player data at the specified index.
        /// </summary>
        /// <param name="index">The index of the player data to retrieve.</param>
        /// <returns>The player data at the given index.</returns>
        public PlayerData GetPlayerData(int index) {
            return _playerDataList[index];
        }

        /// <summary>
        /// Retrieves the player data associated with a specific client ID.
        /// </summary>
        /// <param name="clientId">The client ID of the player.</param>
        /// <returns>The player data corresponding to the client ID, or <c>default</c> if not found.</returns>
        public PlayerData GetPlayerData(ulong clientId) {
            foreach (var playerData in _playerDataList) {
                if (playerData.ClientId == clientId) {
                    return playerData;
                }
            }
            return default;
        }

        /// <summary>
        /// Retrieves the index of the player data associated with a specific client ID.
        /// </summary>
        /// <param name="clientId">The client ID of the player.</param>
        /// <returns>The index of the player data in the list, or -1 if not found.</returns>
        public int GetPlayerDataIndex(ulong clientId) {
            for (var i = 0; i < _playerDataList.Count; i++) {
                if (_playerDataList[i].ClientId == clientId) {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Retrieves the local player data using the local client ID.
        /// </summary>
        /// <returns>The local player's data.</returns>
        public PlayerData GetLocalPlayerData() {
            return GetPlayerData(NetworkManager.Singleton.LocalClientId);
        }


        /// <summary>
        /// Retrieves the color associated with a specific player color index.
        /// </summary>
        /// <param name="colorIndex">The index of the color to retrieve.</param>
        /// <returns>The color corresponding to the given index.</returns>
        public Color GetPlayerColor(int colorIndex) {
            return playerColors[colorIndex];
        }

        /// <summary>
        /// Changes the color of the local player.
        /// This operation is executed on the server.
        /// </summary>
        /// <param name="colorIndex">The new color index to set for the player.</param>
        public void ChangePlayerColor(int colorIndex) {
            ChangePlayerColorServerRpc(colorIndex, new ServerRpcParams());
        }


        /// <summary>
        /// Kicks the player with the specified client ID from the game.
        /// </summary>
        /// <param name="clientId">The client ID of the player to kick.</param>
        public void KickPlayer(ulong clientId) {
            NetworkManager.Singleton.DisconnectClient(clientId);
            HostOnClientDisconnectCallbackAction(clientId);
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

            _playerDataList.OnListChanged += OnPlayerDataListChangedAction;

            DontDestroyOnLoad(gameObject);
        }


        /// <summary>
        /// Checks whether a specific player color is available.
        /// </summary>
        private bool IsColorAvailable(int colorIndex) {
            foreach (var playerData in _playerDataList) {
                if (playerData.ColorIndex == colorIndex) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns the first available color index.
        /// </summary>
        private int GetFirstAvailableColor() {
            for (var i = 0; i < playerColors.Count; i++) {
                if (IsColorAvailable(i)) {
                    return i;
                }
            }
            return -1;
        }


        /// <summary>
        /// Server RPC that changes the color of a player's avatar.
        /// </summary>
        /// <param name="colorIndex">The index of the new color to assign to the player.</param>
        /// <param name="serverRpcParams">The parameters for the RPC (unused in this case).</param>
        [ServerRpc(RequireOwnership = false)]
        private void ChangePlayerColorServerRpc(int colorIndex, ServerRpcParams serverRpcParams) {
            if (!IsColorAvailable(colorIndex)) return;

            var playerDataIndex = GetPlayerDataIndex(serverRpcParams.Receive.SenderClientId);
            var playerData = _playerDataList[playerDataIndex];
            playerData.ColorIndex = colorIndex;
            _playerDataList[playerDataIndex] = playerData;
        }

        /// <summary>
        /// Server RPC to change the player's name and ID.
        /// </summary>
        /// <param name="playerName">The new name of the player.</param>
        /// <param name="playerId">The new player ID.</param>
        /// <param name="serverRpcParams">The parameters for the RPC call, including the client ID of the sender.</param>
        [ServerRpc(RequireOwnership = false)]
        private void ChangePlayerNameAndIdServerRpc(
            string playerName,
            string playerId,
            ServerRpcParams serverRpcParams
        ) {
            var playerDataIndex = GetPlayerDataIndex(serverRpcParams.Receive.SenderClientId);
            var playerData = _playerDataList[playerDataIndex];
            playerData.Name = playerName;
            playerData.PlayerId = playerId;
            _playerDataList[playerDataIndex] = playerData;
        }


        /// <summary>
        /// Connection approval callback that checks if a connection should be accepted or rejected.
        /// </summary>
        /// <param name="request">The connection request details from the client.</param>
        /// <param name="response">The response to the connection request, which determines if the connection is approved.</param>
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

        /// <summary>
        /// Requests the server to update the player's name and ID.
        /// </summary>
        /// <param name="clientId">The client ID of the player that has connected.</param>
        /// <remarks>
        /// Invoked when the <see cref="NetworkManager.OnClientConnectedCallback"/> event is triggered for the client.
        /// </remarks>
        private void OnClientConnectedCallbackAction(ulong clientId) {
            ChangePlayerNameAndIdServerRpc(
                PlayerPrefsManager.GetPlayerName(),
                AuthenticationService.Instance.PlayerId,
                new ServerRpcParams()
            );
        }

        /// <summary>
        /// Notifies that the player has failed to join the game.
        /// </summary>
        /// <param name="clientId">The client ID of the player that has disconnected.</param>
        /// <remarks>
        /// Invoked when the <see cref="NetworkManager.OnClientDisconnectCallback"/> event is triggered for the client.
        /// </remarks>
        private void OnClientDisconnectCallbackAction(ulong clientId) {
            OnFailedToJoin?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Assigns a new player data entry for the connected client.
        /// If the client is the local player, it also requests the server to update the player's name and ID.
        /// </summary>
        /// <param name="clientId">The client ID of the player that has connected.</param>
        /// <remarks>
        /// Invoked when the <see cref="NetworkManager.OnClientConnectedCallback"/> event is triggered for the host.
        /// </remarks>
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

        /// <summary>
        /// Removes the player’s data from the player data list.
        /// </summary>
        /// <param name="clientId">The client ID of the player that has disconnected.</param>
        /// <remarks>
        /// Invoked when the <see cref="NetworkManager.OnClientDisconnectCallback"/> event is triggered for the host.
        /// </remarks>
        private void HostOnClientDisconnectCallbackAction(ulong clientId) {
            var playerDataIndex = GetPlayerDataIndex(clientId);
            if (playerDataIndex == -1) return;
            _playerDataList.RemoveAt(playerDataIndex);
        }


        /// <summary>
        /// Triggers the OnPlayerDataListChanged event to notify other parts of the game.
        /// </summary>
        /// <param name="changeEvent">The change event for the player data list.</param>
        /// <remarks>
        /// Invoked when the <see cref="_playerDataList"/> value is changed.
        /// </remarks>
        private void OnPlayerDataListChangedAction(NetworkListEvent<PlayerData> changeEvent) {
            OnPlayerDataListChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
