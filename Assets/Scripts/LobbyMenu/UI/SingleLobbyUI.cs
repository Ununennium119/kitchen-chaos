using LobbyMenu.Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LobbyMenu.UI {
    public class SingleLobbyUI : MonoBehaviour {
        [SerializeField, Tooltip("The lobby name text")]
        private TextMeshProUGUI lobbyName;
        [SerializeField, Tooltip("The join button")]
        private Button joinButton;


        private LobbyManager _lobbyManager;
        private string _lobbyId;


        public void SetLobby(Unity.Services.Lobbies.Models.Lobby lobby) {
            lobbyName.text = lobby.Name;
            _lobbyId = lobby.Id;
        }


        private void Awake() {
            joinButton.onClick.AddListener(() => _lobbyManager.JoinLobbyById(_lobbyId));
        }

        private void Start() {
            _lobbyManager = LobbyManager.Instance;
        }
    }
}
