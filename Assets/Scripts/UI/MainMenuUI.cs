using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class MainMenuUI : MonoBehaviour {
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;


        private void Awake() {
            playButton.onClick.AddListener(() => { SceneLoader.LoadScene(SceneLoader.Scene.GameScene); });
            quitButton.onClick.AddListener(Application.Quit);
        }
    }
}
