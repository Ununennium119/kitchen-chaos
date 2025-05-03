using System;
using Game.Manager;
using TMPro;
using UnityEngine;

namespace Game.UI.HUD {
    /// <summary>
    /// Controls the UI countdown animation and display shown before the game starts.
    /// </summary>
    public class GameStartCountdownUI : MonoBehaviour {
        private static readonly int PopUpTrigger = Animator.StringToHash("PopUp");


        [SerializeField, Tooltip("Array of count down colors (color at index (i) is used for number (i + 1)")]
        private Color[] colors;
        [SerializeField, Tooltip("The text of the countdown")]
        private TextMeshProUGUI countdownText;
        [SerializeField, Tooltip("The countdown animator")]
        private Animator countdownAnimator;


        private GameManager _gameManager;
        private int _previousNumber;


        /// <summary>
        /// Returns a positive modulus result, ensuring index wrapping for color selection.
        /// </summary>
        /// <param name="a">Dividend</param>
        /// <param name="b">Divisor</param>
        /// <returns>A positive integer representing (a % b)</returns>
        private static int PositiveMod(int a, int b) {
            var remainder = a % b;
            return remainder < 0 ? remainder + Math.Abs(b) : remainder;
        }


        private void Awake() {
            _previousNumber = 0;
        }

        private void Start() {
            _gameManager = GameManager.Instance;

            _gameManager.OnStateChanged += OnGameStateChangedAction;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Updates the countdown display and triggers animations when the number changes.
        /// </summary>
        private void Update() {
            var countdownCeil = GetCountdownCeil();
            if (countdownCeil != _previousNumber) {
                _previousNumber = countdownCeil;
                countdownAnimator.SetTrigger(PopUpTrigger);
            }
            countdownText.text = countdownCeil.ToString();
            var color = colors[PositiveMod(countdownCeil - 1, colors.Length)];
            countdownText.color = new Color(color.r, color.g, color.b, 1f);
        }


        /// <summary>
        /// Handles showing or hiding the countdown UI based on game state.
        /// </summary>
        /// <remarks>
        /// Invoked when the <see cref="GameManager.OnStateChanged"/> event is triggered.
        /// </remarks>
        private void OnGameStateChangedAction(object sender, GameManager.OnStateChangedArgs e) {
            var isActive = e.State == GameManager.State.Countdown;
            gameObject.SetActive(isActive);
        }

        /// <summary>
        /// Gets the current countdown time rounded up to the nearest whole number.
        /// </summary>
        /// <returns>Ceiling integer of the current countdown time</returns>
        private int GetCountdownCeil() {
            var countdown = _gameManager.GetCountdownTime();
            return Mathf.CeilToInt(countdown);
        }
    }
}
