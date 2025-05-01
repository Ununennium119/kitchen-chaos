using Multiplayer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Lobby {
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
        

        public void Show() {
            gameObject.SetActive(true);
        }


        private void Awake() {
            createButton.onClick.AddListener(() => {
                _lobbyManager.CreateLobby(lobbyNameInput.text, lobbyPrivateToggle.isOn);
            });
            closeButton.onClick.AddListener(Hide);
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
