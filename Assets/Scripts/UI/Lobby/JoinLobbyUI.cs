using Multiplayer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Lobby {
    public class JoinLobbyUI : MonoBehaviour {
        [SerializeField, Tooltip("The lobby code input")]
        private TMP_InputField lobbyCodeInput;
        [SerializeField, Tooltip("The join button")]
        private Button joinButton;
        [SerializeField, Tooltip("The close button")]
        private Button closeButton;

        
        private LobbyManager _lobbyManager;
        

        public void Show() {
            gameObject.SetActive(true);
        }


        private void Awake() {
            joinButton.onClick.AddListener(() => {
                _lobbyManager.JoinLobbyByCode(lobbyCodeInput.text);
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
