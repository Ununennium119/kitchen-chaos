using System;
using Common.Logic;
using UnityEngine;

namespace LobbyMenu.UI {
    /// <summary>
    /// The UI for the "Connecting" screen during the process of joining a multiplayer lobby.
    /// </summary>
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


        /// <remarks>
        /// Invoked when the <see cref="MultiplayerManager.OnTryingToJoin"/> event is triggered.
        /// </remarks>
        private void OnTryingToJoinAction(object sender, EventArgs e) {
            Show();
        }

        /// <remarks>
        /// Invoked when the <see cref="MultiplayerManager.OnFailedToJoin"/> event is triggered.
        /// </remarks>
        private void OnFailedToJoinAction(object sender, EventArgs e) {
            Hide();
        }
    }
}
