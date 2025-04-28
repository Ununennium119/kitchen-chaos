using Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Lobby {
    public class LobbyTestUI : MonoBehaviour {
        [SerializeField, Tooltip("The create game button")]
        private Button createGameButton;

        [SerializeField, Tooltip("The join game button")]
        private Button joinGameButton;


        private void Start() {
            createGameButton.onClick.AddListener(() => {
                MultiplayerManager.Instance.StartHost();
                SceneLoader.LoadNetwork(SceneLoader.Scene.CharacterSelectScene);
            });
            joinGameButton.onClick.AddListener(() => { MultiplayerManager.Instance.StartClient(); });
        }
    }
}
