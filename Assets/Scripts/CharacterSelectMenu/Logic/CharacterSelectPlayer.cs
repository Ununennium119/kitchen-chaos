using System;
using Common.Logic;
using Game.Player;
using LobbyMenu.Logic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterSelectMenu.Logic {
    /// <summary>
    /// Handles the visual representation and interaction logic for a player in the character selection menu.
    /// </summary>
    public class CharacterSelectPlayer : MonoBehaviour {
        [SerializeField, Tooltip("The index of the player visual")]
        private int index;

        [SerializeField, Tooltip("The game object that shows when the player is ready")]
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

            // Subscribe to events
            _multiplayerManager.OnPlayerDataListChanged += OnPlayerDataListChangedAction;
            _characterSelectReadyManager.OnReadyChanged += OnReadyChangedAction;

            UpdatePlayerVisual();

            // At start, no one is ready
            readyGameObject.SetActive(false);

            // Show kick button for the server
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


        /// <summary>
        /// Updates the UI elements based on the current player data.
        /// </summary>
        private void UpdatePlayerVisual() {
            // Set active
            var isActive = _multiplayerManager.HasPlayerData(index);
            gameObject.SetActive(isActive);
            if (!isActive) return;

            // Set player name and color
            var playerData = _multiplayerManager.GetPlayerData(index);
            nameText.text = playerData.Name.ToString();
            var color = _multiplayerManager.GetPlayerColor(playerData.ColorIndex);
            playerVisual.SetColor(color);
        }


        /// <remarks>
        /// Invoked when the <see cref="MultiplayerManager.OnPlayerDataListChanged"/> event is triggered.
        /// </remarks>
        private void OnPlayerDataListChangedAction(object sender, EventArgs e) {
            UpdatePlayerVisual();
        }

        /// <summary>
        /// Updates the ready status indicator based on readiness state.
        /// </summary>
        /// <remarks>
        /// Invoked when the <see cref="CharacterSelectReadyManager.OnReadyChanged"/> event is triggered.
        /// </remarks>
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
