using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logic;
using LobbyMenu.Logic;
using Unity.Netcode;
using Logger = Common.Logic.Logger;

namespace CharacterSelectMenu.Logic {
    /// <summary>
    /// Manages the ready status of players in the character select menu.
    /// </summary>
    /// <remarks>This class is singleton.</remarks>
    public class CharacterSelectReadyManager : NetworkBehaviour {
        public static CharacterSelectReadyManager Instance { get; private set; }


        /// <summary>
        /// This event is triggered whenever a player ready status changes.
        /// </summary>
        public event EventHandler OnReadyChanged;


        private LobbyManager _lobbyManager;

        /// <summary>
        /// Tracks the ready status of each player by their client ID.
        /// </summary>
        private readonly Dictionary<ulong, bool> _playerReadyDictionary = new();


        /// <summary>
        /// Marks the local player as ready. Sends a server RPC request.
        /// </summary>
        public void SetPlayerReady() {
            SetPlayerReadyServerRpc();
        }

        /// <param name="clientId">The client ID to check.</param>
        /// <returns>True if the player is ready, otherwise false.</returns>
        public bool IsPlayerReady(ulong clientId) {
            return _playerReadyDictionary.GetValueOrDefault(clientId, false);
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
            _lobbyManager = LobbyManager.Instance;
        }


        /// <summary>
        /// Server RPC that marks a player as ready based on the sender's client ID.
        /// </summary>
        /// <param name="serverRpcParams">RPC params</param>
        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
            var clientId = serverRpcParams.Receive.SenderClientId;
            _playerReadyDictionary[clientId] = true;
            SetPlayerReadyClientRpc(clientId);

            var playerReadyList = NetworkManager.Singleton.ConnectedClientsIds.Select(
                playerId => _playerReadyDictionary.TryGetValue(playerId, out var isReady) && isReady
            );
            if (playerReadyList.All(isPlayerReady => isPlayerReady)) {
                _lobbyManager.DeleteLobby();
                SceneLoader.LoadNetwork(SceneLoader.Scene.GameScene);
            }
        }

        /// <summary>
        /// Client RPC that updates the local ready status for the specified client ID.
        /// </summary>
        /// <param name="clientId">The ID of the client that became ready.</param>
        [ClientRpc]
        private void SetPlayerReadyClientRpc(ulong clientId) {
            _playerReadyDictionary[clientId] = true;
            OnReadyChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
