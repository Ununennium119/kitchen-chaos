using Manager;
using Multiplayer;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterSelect {
    public class CharacterSelectUI : NetworkBehaviour {
        [SerializeField, Tooltip("The main menu button")]
        private Button mainMenuButton;
        [SerializeField, Tooltip("The ready button")]
        private Button readyButton;
        [SerializeField, Tooltip("The lobby name text")]
        private TextMeshProUGUI lobbyNameText;
        [SerializeField, Tooltip("The lobby code text")]
        private TextMeshProUGUI lobbyCodeText;


        private CharacterSelectReadyManager _characterSelectReadyManager;
        private LobbyManager _lobbyManager;


        private void Start() {
            _characterSelectReadyManager = CharacterSelectReadyManager.Instance;
            _lobbyManager = LobbyManager.Instance;
            
            mainMenuButton.onClick.AddListener(() => {
                _lobbyManager.LeaveLobby();
                NetworkManager.Singleton.Shutdown();
                SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene);
            });
            readyButton.onClick.AddListener(() => { _characterSelectReadyManager.SetPlayerReady(); });

            var lobby = _lobbyManager.GetJoinedLobby();
            lobbyNameText.text = lobby.Name;
            lobbyCodeText.text = lobby.LobbyCode;
        }
    }
}
