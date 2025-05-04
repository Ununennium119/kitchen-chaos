using LobbyMenu.Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LobbyMenu.UI {
    /// <summary>
    /// The UI for joining an existing multiplayer lobby.
    /// </summary>
    public class JoinLobbyUI : MonoBehaviour {
        [SerializeField, Tooltip("The lobby code input")]
        private TMP_InputField lobbyCodeInput;
        [SerializeField, Tooltip("The join button")]
        private Button joinButton;
        [SerializeField, Tooltip("The close button")]
        private Button closeButton;


        private LobbyManager _lobbyManager;


        /// <summary>
        /// Shows the UI for joining a lobby.
        /// </summary>
        public void Show() {
            gameObject.SetActive(true);
            lobbyCodeInput.Select();
        }


        private void Awake() {
            joinButton.onClick.AddListener(() => { _lobbyManager.JoinLobbyByCode(lobbyCodeInput.text); });
            closeButton.onClick.AddListener(() => {
                EventSystem.current.SetSelectedGameObject(null);
                Hide();
            });
        }

        private void Start() {
            _lobbyManager = LobbyManager.Instance;

            Hide();
        }


        private void Hide() {
            gameObject.SetActive(false);
        }
    }
}
