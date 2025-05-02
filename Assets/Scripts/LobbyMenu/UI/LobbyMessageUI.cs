using System;
using LobbyMenu.Logic;
using Multiplayer;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LobbyMenu.UI {
    public class LobbyMessageUI : MonoBehaviour {
        private MultiplayerManager _multiplayerManager;
        private LobbyManager _lobbyManager;


        [SerializeField, Tooltip("The message text")]
        private TextMeshProUGUI messageText;
        [SerializeField, Tooltip("The close button")]
        private Button closeButton;


        private void Awake() {
            closeButton.onClick.AddListener(() => {
                EventSystem.current.SetSelectedGameObject(null);
                Hide();
            });
        }

        private void Start() {
            _multiplayerManager = MultiplayerManager.Instance;
            _lobbyManager = LobbyManager.Instance;

            _multiplayerManager.OnFailedToJoin += OnFailedToJoinAction;

            _lobbyManager.OnCreateLobbyStarted += OnCreateLobbyStartedAction;
            _lobbyManager.OnCreateLobbyFailed += OnCreateLobbyFailedAction;
            _lobbyManager.OnJoinLobbyStarted += OnJoinLobbyStartedAction;
            _lobbyManager.OnJoinLobbyFailed += OnJoinLobbyFailedAction;
            _lobbyManager.OnQuickJoinNotFound += OnQuickJoinNotFoundAction;

            Hide();
        }

        private void OnDestroy() {
            _multiplayerManager.OnFailedToJoin -= OnFailedToJoinAction;

            _lobbyManager.OnCreateLobbyStarted -= OnCreateLobbyStartedAction;
            _lobbyManager.OnCreateLobbyFailed -= OnCreateLobbyFailedAction;
            _lobbyManager.OnJoinLobbyStarted -= OnJoinLobbyStartedAction;
            _lobbyManager.OnJoinLobbyFailed -= OnJoinLobbyFailedAction;
            _lobbyManager.OnQuickJoinNotFound -= OnQuickJoinNotFoundAction;
        }


        private void Show() {
            gameObject.SetActive(true);
            closeButton.Select();
        }

        private void Hide() {
            gameObject.SetActive(false);
        }

        private void ShowMessage(string message) {
            messageText.text = message;
            Show();
        }


        private void OnFailedToJoinAction(object sender, EventArgs e) {
            var reason = NetworkManager.Singleton.DisconnectReason;
            if (reason == "") {
                reason = "Failed to connect!";
            }
            ShowMessage(reason);
        }

        private void OnCreateLobbyStartedAction(object sender, EventArgs e) {
            ShowMessage("Creating lobby...");
        }

        private void OnCreateLobbyFailedAction(object sender, EventArgs e) {
            ShowMessage("Failed to create lobby!");
        }

        private void OnJoinLobbyStartedAction(object sender, EventArgs e) {
            ShowMessage("Joining lobby...");
        }

        private void OnJoinLobbyFailedAction(object sender, EventArgs e) {
            ShowMessage("Failed to join the lobby!");
        }

        private void OnQuickJoinNotFoundAction(object sender, EventArgs e) {
            ShowMessage("Could not find a lobby to join!");
        }
    }
}
