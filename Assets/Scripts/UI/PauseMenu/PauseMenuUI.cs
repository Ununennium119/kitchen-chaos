using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PauseMenu {
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
            mainMenuButton.onClick.AddListener(() => { SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene); });
        }

        private void Start() {
            _gameManager = GameManager.Instance;
            _optionsMenuUI = OptionsMenuUI.Instance;

            _gameManager.OnPauseToggled += OnPauseToggledAction;

            gameObject.SetActive(false);
        }


        private void OnPauseToggledAction(object sender, GameManager.OnPauseToggledArgs e) {
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
