using System.Linq;
using Game.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.HUD {
    /// <summary>
    /// Manages the game clock UI element that displays remaining game time.
    /// </summary>
    public class GameClockUI : MonoBehaviour {
        [SerializeField, Tooltip("The image of the clock used to show remaining time")]
        private Image clockImage;


        private GameManager _gameManager;


        private void Start() {
            _gameManager = GameManager.Instance;

            _gameManager.OnStateChanged += OnStateChangedAction;
        }

        /// <summary>
        /// Updates the clock fill amount each frame based on the remaining game time.
        /// </summary>
        private void Update() {
            var remainingGameTime = _gameManager.GetRemainingGameTimeNormalized();
            clockImage.fillAmount = remainingGameTime;
        }


        /// <summary>
        /// Toggles the visibility of the clock UI based on the current game state.
        /// </summary>
        /// <remarks>
        /// Invoked when the <see cref="GameManager.OnStateChanged"/> event is triggered.
        /// </remarks>
        private void OnStateChangedAction(object sender, GameManager.OnStateChangedArgs e) {
            var isActive = new[] { GameManager.State.Playing, GameManager.State.Countdown }.Contains(e.State);
            gameObject.SetActive(isActive);
        }
    }
}
