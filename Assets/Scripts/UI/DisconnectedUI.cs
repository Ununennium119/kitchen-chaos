using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
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
        }

        private void Hide() {
            gameObject.SetActive(false);
        }


        private void OnClientDisconnectCallback(ulong clientId) {
            if (clientId == NetworkManager.LocalClientId || clientId == NetworkManager.ServerClientId) {
                Show();
            }
        }
    }
}
