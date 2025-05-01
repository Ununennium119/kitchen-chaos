using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Manager {
    public class CharacterSelectReadyManager : NetworkBehaviour {
        public static CharacterSelectReadyManager Instance { get; private set; }
        
        
        private readonly Dictionary<ulong, bool> _playerReadyDictionary = new();


        public void SetPlayerReady() {
            SetPlayerReadyServerRpc();
        }


        private void Awake() {
            Debug.Log("Setting up CharacterSelectReadyManager...");
            if (Instance != null) {
                Debug.LogError("There are more than one CharacterSelectReadyManager in the scene!");
            }
            Instance = this;
        }
        

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
            _playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

            var playerReadyList = NetworkManager.Singleton.ConnectedClientsIds.Select(
                playerId => _playerReadyDictionary.TryGetValue(playerId, out var isReady) && isReady
            );
            if (playerReadyList.All(isPlayerReady => isPlayerReady)) {
                SceneLoader.LoadNetwork(SceneLoader.Scene.GameScene);
            }
        }
    }
}
