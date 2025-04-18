using System;
using Manager;
using UnityEngine;

namespace UI.PauseMenu {
    public class MultiplayerPauseUI : MonoBehaviour {
        
        private GameManager _gameManager;


        private void Start() {
            _gameManager = GameManager.Instance;

            _gameManager.OnPauseToggled += OnPauseToggledAction;
            
            Hide();
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
        }

        private void Hide() {
            gameObject.SetActive(false);
        }
    }
}
