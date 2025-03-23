using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class PauseMenuUI : MonoBehaviour {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button mainMenuButton;


        private GameManager _gameManager;


        private void Awake() {
            resumeButton.onClick.AddListener(() => { _gameManager.ToggleGamePause(); });
            mainMenuButton.onClick.AddListener(() => { SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene); });
        }

        private void Start() {
            _gameManager = GameManager.Instance;

            _gameManager.OnPauseToggled += OnPauseToggledAction;
            
            gameObject.SetActive(false);
        }


        private void OnPauseToggledAction(object sender, GameManager.OnPauseToggledArgs e) {
            gameObject.SetActive(e.IsGamePaused);
        }
    }
}
