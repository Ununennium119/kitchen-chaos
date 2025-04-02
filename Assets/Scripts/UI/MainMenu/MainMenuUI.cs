using Counter;
using Counter.Logic;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu {
    public class MainMenuUI : MonoBehaviour {
        [SerializeField, Tooltip("The play button")]
        private Button playButton;
        [SerializeField, Tooltip("The quit button")]
        private Button quitButton;


        private void Awake() {
            playButton.onClick.AddListener(() => { SceneLoader.LoadScene(SceneLoader.Scene.GameScene); });
            quitButton.onClick.AddListener(Application.Quit);

            // Resetting (setting to null) all static objects used when loading main menu
            TrashCounter.ResetStaticObjects();
            CuttingCounter.ResetStaticObjects();
            PlayerController.ResetStaticObjects();

            // Resetting time scale
            Time.timeScale = 1f;

            playButton.Select();
        }
    }
}
