using System;
using Manager;
using UnityEngine;

namespace UI {
    public class WaitingForOtherPlayersUI : MonoBehaviour {
        
        private GameManager _gameManager;


        private void Start() {
            _gameManager = GameManager.Instance;

            _gameManager.OnStateChanged += OnGameStateChangedAction;
            _gameManager.OnLocalPlayerReadyChanged += OnLocalPlayerReadyChangedAction;
            
            Hide();
        }


        private void Show() {
            gameObject.SetActive(true);
        }

        private void Hide() {
            gameObject.SetActive(false);
        }
        

        private void OnGameStateChangedAction(object sender, GameManager.OnStateChangedArgs e) {
            if (e.State == GameManager.State.Countdown) {
                Hide();
            }
        }

        private void OnLocalPlayerReadyChangedAction(object sender, GameManager.OnLocalPlayerReadyChangedArgs e) {
            if (e.IsLocalPlayerReady) {
                Show();
            }
        }
    }
}
