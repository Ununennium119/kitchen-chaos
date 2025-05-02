using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Logic;
using LobbyMenu.Logic;
using Unity.Netcode;
using UnityEngine;

namespace CharacterSelectMenu.Logic {
    public class CharacterSelectReadyManager : NetworkBehaviour {
        public static CharacterSelectReadyManager Instance { get; private set; }
        
        
        public event EventHandler OnReadyChanged;


        private LobbyManager _lobbyManager;
        private readonly Dictionary<ulong, bool> _playerReadyDictionary = new();


        public void SetPlayerReady() {
            SetPlayerReadyServerRpc();
        }

        public bool IsPlayerReady(ulong clientId) {
            return _playerReadyDictionary.GetValueOrDefault(clientId, false);
        }


        private void Awake() {
            Debug.Log("Setting up CharacterSelectReadyManager...");
            if (Instance != null) {
                Debug.LogError("There are more than one CharacterSelectReadyManager in the scene!");
            }
            Instance = this;
        }

        private void Start() {
            _lobbyManager = LobbyManager.Instance;
        }


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

        [ClientRpc]
        private void SetPlayerReadyClientRpc(ulong clientId) {
            _playerReadyDictionary[clientId] = true;
            OnReadyChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}