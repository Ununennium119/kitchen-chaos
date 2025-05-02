using System;
using Multiplayer;
using UnityEngine;

namespace UI.Lobby {
    public class ConnectingUI : MonoBehaviour {
        private MultiplayerManager _multiplayerManager;


        private void Start() {
            _multiplayerManager = MultiplayerManager.Instance;

            _multiplayerManager.OnTryingToJoin += OnTryingToJoinAction;
            _multiplayerManager.OnFailedToJoin += OnFailedToJoinAction;

            Hide();
        }

        private void OnDestroy() {
            _multiplayerManager.OnTryingToJoin -= OnTryingToJoinAction;
            _multiplayerManager.OnFailedToJoin -= OnFailedToJoinAction;
        }


        private void Show() {
            gameObject.SetActive(true);
        }

        private void Hide() {
            gameObject.SetActive(false);
        }


        private void OnTryingToJoinAction(object sender, EventArgs e) {
            Show();
        }

        private void OnFailedToJoinAction(object sender, EventArgs e) {
            Hide();
        }
    }
}
