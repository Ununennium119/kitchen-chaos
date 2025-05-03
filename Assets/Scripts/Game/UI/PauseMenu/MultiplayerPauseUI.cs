using Game.Manager;
using UnityEngine;

namespace Game.UI.PauseMenu {
    /// <summary>
    /// Handles the visibility of the multiplayer pause UI.
    /// </summary>
    public class MultiplayerPauseUI : MonoBehaviour {
        
        private GameManager _gameManager;


        private void Start() {
            _gameManager = GameManager.Instance;

            _gameManager.OnPauseToggled += OnPauseToggledAction;
            
            Hide();
        }


        private void Show() {
            gameObject.SetActive(true);
        }

        private void Hide() {
            gameObject.SetActive(false);
        }


        /// <remarks>
        /// Invoked when the <see cref="GameManager.OnPauseToggled"/> event is triggered.
        /// </remarks>
        private void OnPauseToggledAction(object sender, GameManager.OnPauseToggledArgs e) {
            if (e.IsGamePaused) {
                Show();
            } else {
                Hide();
            }
        }
    }
}
