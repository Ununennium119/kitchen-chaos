using Game.Manager;
using UnityEngine;

namespace Game.UI {
    /// <summary>
    /// Manages the UI that informs the player to wait for other players to be ready before the game starts.
    /// </summary>
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
        

        /// <summary>
        /// Hides the UI if the state is Countdown.
        /// </summary>
        /// <remarks>
        /// Invoked when the <see cref="GameManager.OnStateChanged"/> event is triggered.
        /// </remarks>
        private void OnGameStateChangedAction(object sender, GameManager.OnStateChangedArgs e) {
            if (e.State == GameManager.State.Countdown) {
                Hide();
            }
        }

        /// <summary>
        /// Shows the UI if the local player is not ready.
        /// </summary>
        /// <remarks>
        /// Invoked when the <see cref="GameManager.OnLocalPlayerReadyChanged"/> event is triggered.
        /// </remarks>
        private void OnLocalPlayerReadyChangedAction(object sender, GameManager.OnLocalPlayerReadyChangedArgs e) {
            if (e.IsLocalPlayerReady) {
                Show();
            }
        }
    }
}
