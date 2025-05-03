using Common.Logic;
using Common.Utility;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI {
    /// <summary>
    /// The UI displayed when the client is disconnected from the network.
    /// </summary>
    public class DisconnectedUI : NetworkBehaviour {
        [SerializeField, Tooltip("The main menu button")]
        private Button mainMenuButton;


        private void Awake() {
            mainMenuButton.onClick.AddListener(() => SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene));
        }

        private void Start() {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;

            Hide();
        }

        public override void OnDestroy() {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
        }


        private void Show() {
            gameObject.SetActive(true);
            mainMenuButton.Select();
        }

        private void Hide() {
            gameObject.SetActive(false);
        }

        /// <remarks>
        /// Invoked when the <see cref="NetworkManager.OnClientDisconnectCallback"/> event is triggered.
        /// </remarks>
        private void OnClientDisconnectCallback(ulong clientId) {
            if (clientId == NetworkManager.LocalClientId || clientId == NetworkManager.ServerClientId) {
                Show();
            }
        }
    }
}
