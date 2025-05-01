using System;
using Multiplayer;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Lobby {
    public class ConnectionResponseMessageUI : MonoBehaviour {
        private MultiplayerManager _multiplayerManager;


        [SerializeField, Tooltip("The message text")]
        private TextMeshProUGUI messageText;
        [SerializeField, Tooltip("The close button")]
        private Button closeButton;


        private void Awake() {
            closeButton.onClick.AddListener(Hide);
        }

        private void Start() {
            _multiplayerManager = MultiplayerManager.Instance;

            _multiplayerManager.OnFailedToJoin += OnFailedToJoinAction;

            Hide();
        }

        private void OnDestroy() {
            _multiplayerManager.OnFailedToJoin -= OnFailedToJoinAction;
        }


        private void Show() {
            gameObject.SetActive(true);
        }

        private void Hide() {
            gameObject.SetActive(false);
        }

        private void OnFailedToJoinAction(object sender, EventArgs e) {
            var reason = NetworkManager.Singleton.DisconnectReason;
            if (reason == "") {
                reason = "Failed to connect!";
            }
            messageText.text = reason;
            
            Show();
        }
    }
}
