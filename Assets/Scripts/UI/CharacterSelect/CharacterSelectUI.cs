using Manager;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterSelect {
    public class CharacterSelectUI : NetworkBehaviour {
        [SerializeField, Tooltip("The main menu button")]
        private Button mainMenuButton;
        [SerializeField, Tooltip("The ready button")]
        private Button readyButton;


        private CharacterSelectReadyManager _characterSelectReadyManager;


        private void Start() {
            _characterSelectReadyManager = CharacterSelectReadyManager.Instance;

            mainMenuButton.onClick.AddListener(() => {
                NetworkManager.Singleton.Shutdown();
                SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene);
            });
            readyButton.onClick.AddListener(() => { _characterSelectReadyManager.SetPlayerReady(); });
        }
    }
}
