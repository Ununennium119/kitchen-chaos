using Manager;
using Multiplayer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Lobby {
    public class LobbyUI : MonoBehaviour {
        [SerializeField, Tooltip("The main menu button")]
        private Button mainMenuButton;
        [SerializeField, Tooltip("The create lobby button")]
        private Button createLobbyButton;
        [SerializeField, Tooltip("The join lobby button")]
        private Button joinLobbyButton;
        [SerializeField, Tooltip("The quick join lobby button")]
        private Button quickJoinLobbyButton;

        [SerializeField, Tooltip("The create lobby Ui")]
        private CreateLobbyUI createLobbyUI;
        [SerializeField, Tooltip("The join lobby Ui")]
        private JoinLobbyUI joinLobbyUI;

        [SerializeField, Tooltip("The player name input")]
        private TMP_InputField nameInput;
        [SerializeField, Tooltip("The update name button")]
        private Button updateNameButton;


        private LobbyManager _lobbyManager;

        private void Awake() {
            mainMenuButton.onClick.AddListener(() => { SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene); });
            createLobbyButton.onClick.AddListener(createLobbyUI.Show);
            joinLobbyButton.onClick.AddListener(joinLobbyUI.Show);
            quickJoinLobbyButton.onClick.AddListener(() => _lobbyManager.QuickJoinLobby());
            updateNameButton.onClick.AddListener(() => { PlayerPrefsManager.SetPlayerName(nameInput.text); });

            nameInput.text = PlayerPrefsManager.GetPlayerName();
        }

        private void Start() {
            _lobbyManager = LobbyManager.Instance;
        }
    }
}
