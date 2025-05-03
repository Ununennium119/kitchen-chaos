using LobbyMenu.Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LobbyMenu.UI {
    /// <summary>
    /// The UI for creating a new multiplayer lobby.
    /// </summary>
    public class CreateLobbyUI : MonoBehaviour {
        [SerializeField, Tooltip("The lobby name input")]
        private TMP_InputField lobbyNameInput;
        [SerializeField, Tooltip("The lobby private toggle")]
        private Toggle lobbyPrivateToggle;
        [SerializeField, Tooltip("The create button")]
        private Button createButton;
        [SerializeField, Tooltip("The close button")]
        private Button closeButton;

        
        private LobbyManager _lobbyManager;
        

        /// <summary>
        /// Shows the UI for creating a new lobby.
        /// </summary>
        public void Show() {
            gameObject.SetActive(true);
            lobbyNameInput.Select();
        }


        private void Awake() {
            createButton.onClick.AddListener(() => {
                _lobbyManager.CreateLobby(lobbyNameInput.text, lobbyPrivateToggle.isOn);
            });
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
