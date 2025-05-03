using LobbyMenu.Logic;
using UnityEngine;

namespace LobbyMenu.UI {
    /// <summary>
    /// The UI for displaying a list of available lobbies in the lobby menu.
    /// </summary>
    public class LobbyListUI : MonoBehaviour {
        [SerializeField, Tooltip("The game object which contains lobbies")]
        private Transform lobbyContainer;
        [SerializeField, Tooltip("The lobby template")]
        private Transform lobbyTemplate;


        private LobbyManager _lobbyManager;


        private void Awake() {
            lobbyTemplate.gameObject.SetActive(false);
        }

        private void Start() {
            _lobbyManager = LobbyManager.Instance;

            _lobbyManager.OnLobbyListRefreshed += OnLobbyListRefreshedAction;
        }

        private void OnDestroy() {
            _lobbyManager.OnLobbyListRefreshed -= OnLobbyListRefreshedAction;
        }


        /// <summary>
        /// Updates the UI with the latest list of lobbies.
        /// </summary>
        /// <remarks>
        /// Invoked when the <see cref="LobbyManager.OnLobbyListRefreshed"/> event is triggered.
        /// </remarks>
        private void OnLobbyListRefreshedAction(object sender, LobbyManager.OnLobbyListRefreshedEventArgs e) {
            foreach (Transform child in lobbyContainer) {
                if (child == lobbyTemplate) continue;

                Destroy(child.gameObject);
            }

            foreach (var lobby in e.LobbyList) {
                var lobbyItem = Instantiate(lobbyTemplate, lobbyContainer);
                lobbyItem.GetComponent<SingleLobbyUI>().SetLobby(lobby);
                lobbyItem.gameObject.SetActive(true);
            }
        }
    }
}
