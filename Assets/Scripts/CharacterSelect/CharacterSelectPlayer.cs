using System;
using LobbyMenu;
using LobbyMenu.Logic;
using Manager;
using Multiplayer;
using Player;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterSelect {
    public class CharacterSelectPlayer : MonoBehaviour {
        [SerializeField, Tooltip("The index of the player visual")]
        private int index;

        [SerializeField, Tooltip("The ready game object")]
        private GameObject readyGameObject;

        [SerializeField, Tooltip("The player name text")]
        private TextMeshPro nameText;

        [SerializeField, Tooltip("The player visual")]
        private PlayerVisual playerVisual;

        [SerializeField, Tooltip("The kick button")]
        private Button kickButton;


        private MultiplayerManager _multiplayerManager;
        private LobbyManager _lobbyManager;
        private CharacterSelectReadyManager _characterSelectReadyManager;


        private void Awake() {
            kickButton.onClick.AddListener(() => {
                var playerData = _multiplayerManager.GetPlayerData(index);
                _lobbyManager.KickPlayer(playerData.PlayerId.ToString());
                _multiplayerManager.KickPlayer(playerData.ClientId);
            });
        }
    
        private void Start() {
            _multiplayerManager = MultiplayerManager.Instance;
            _lobbyManager = LobbyManager.Instance;
            _characterSelectReadyManager = CharacterSelectReadyManager.Instance;

            _multiplayerManager.OnPlayerDataListChanged += OnPlayerDataListChangedAction;
            _characterSelectReadyManager.OnReadyChanged += OnReadyChangedAction;

            UpdatePlayerVisual();
            readyGameObject.SetActive(false);

            var isKickButtonActive = NetworkManager.Singleton.IsServer;
            if (_multiplayerManager.HasPlayerData(index)) {
                var playerData = _multiplayerManager.GetPlayerData(index);
                isKickButtonActive &= playerData.ClientId != NetworkManager.Singleton.LocalClientId;
            }
            kickButton.gameObject.SetActive(isKickButtonActive);
        }

        private void OnDestroy() {
            _multiplayerManager.OnPlayerDataListChanged -= OnPlayerDataListChangedAction;
        }


        private void UpdatePlayerVisual() {
            var isActive = _multiplayerManager.HasPlayerData(index);
            gameObject.SetActive(isActive);
            if (!isActive) return;

            var playerData = _multiplayerManager.GetPlayerData(index);
            nameText.text = playerData.Name.ToString();
            var color = _multiplayerManager.GetPlayerColor(playerData.ColorIndex);
            playerVisual.SetColor(color);
        }


        private void OnPlayerDataListChangedAction(object sender, EventArgs e) {
            UpdatePlayerVisual();
        }

        private void OnReadyChangedAction(object sender, EventArgs e) {
            var isActive = false;
            if (_multiplayerManager.HasPlayerData(index)) {
                var playerData = _multiplayerManager.GetPlayerData(index);
                isActive = _characterSelectReadyManager.IsPlayerReady(playerData.ClientId);
            }
            readyGameObject.SetActive(isActive);
        }
    }
}
