using System;
using Manager;
using TMPro;
using UnityEngine;

namespace UI.HUD {
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


        private void OnGameStateChangedAction(object sender, GameManager.OnStateChangedArgs e) {
            var isActive = e.State == GameManager.State.Countdown;
            gameObject.SetActive(isActive);
        }

        private int GetCountdownCeil() {
            var countdown = _gameManager.GetCountdownTime();
            return Mathf.CeilToInt(countdown);
        }
    }
}
