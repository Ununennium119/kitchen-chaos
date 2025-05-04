using Common.Utility;
using Game.Manager;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.PauseMenu {
    /// <summary>
    /// Handles the UI logic for the pause menu, including the resume, options, and main menu buttons.
    /// </summary>
    public class PauseMenuUI : MonoBehaviour {
        [SerializeField, Tooltip("The resume button")]
        private Button resumeButton;
        [SerializeField, Tooltip("The options button")]
        private Button optionsButton;
        [SerializeField, Tooltip("The main menu button")]
        private Button mainMenuButton;


        private GameManager _gameManager;
        private OptionsMenuUI _optionsMenuUI;


        private void Awake() {
            resumeButton.onClick.AddListener(() => { _gameManager.ToggleGamePause(); });
            optionsButton.onClick.AddListener(() => {
                _optionsMenuUI.Show(Show);
                gameObject.SetActive(false);
            });
            mainMenuButton.onClick.AddListener(() => {
                NetworkManager.Singleton.Shutdown();
                SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene);
            });
        }

        private void Start() {
            _gameManager = GameManager.Instance;
            _optionsMenuUI = OptionsMenuUI.Instance;

            _gameManager.OnLocalPauseToggled += OnLocalPauseToggledAction;

            gameObject.SetActive(false);
        }


        /// <remarks>
        /// Invoked when the <see cref="GameManager.OnLocalPauseToggled"/> event is triggered.
        /// </remarks>
        private void OnLocalPauseToggledAction(object sender, GameManager.OnLocalPauseToggledArgs e) {
            if (e.IsGamePaused) {
                Show();
            } else {
                Hide();
            }
        }

        private void Show() {
            gameObject.SetActive(true);
            resumeButton.Select();
        }

        private void Hide() {
            gameObject.SetActive(false);
        }
    }
}
