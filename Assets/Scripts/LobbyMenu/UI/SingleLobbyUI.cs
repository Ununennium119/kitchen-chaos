using LobbyMenu.Logic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace LobbyMenu.UI {
    /// <summary>
    /// Represents a single lobby item in the lobby list UI.
    /// </summary>
    public class SingleLobbyUI : MonoBehaviour {
        [SerializeField, Tooltip("The lobby name text")]
        private TextMeshProUGUI lobbyName;
        [SerializeField, Tooltip("The join button")]
        private Button joinButton;


        private LobbyManager _lobbyManager;
        private string _lobbyId;


        /// <summary>
        /// Sets the lobby information, including the lobby name and ID.
        /// </summary>
        /// <param name="lobby">The <see cref="Lobby"/> object containing the lobby information.</param>
        public void SetLobby(Lobby lobby) {
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
