using System;
using TMPro;
using UnityEngine;

namespace UI {
    public class StartGameCountdownUI : MonoBehaviour {
        [SerializeField] private Color[] colors;
        [SerializeField] private TextMeshProUGUI countdownText;


        private GameManager _gameManager;


        private static int PositiveMod(int a, int b) {
            var remainder = a % b;
            return remainder < 0 ? remainder + Math.Abs(b) : remainder;
        }


        private void Start() {
            _gameManager = GameManager.Instance;

            _gameManager.OnStateChanged += OnStateChangedAction;
            gameObject.SetActive(false);
        }

        private void Update() {
            var countdownCeil = GetCountdownCeil();
            countdownText.text = countdownCeil.ToString();
            var color = colors[PositiveMod(countdownCeil - 1, colors.Length)];
            countdownText.color = new Color(color.r, color.g, color.b, 1f);
        }


        private void OnStateChangedAction(object sender, GameManager.OnStateChangedArgs e) {
            var isActive = e.State == GameManager.State.Countdown;
            gameObject.SetActive(isActive);
        }

        private int GetCountdownCeil() {
            var countdown = _gameManager.GetCountdownTimer();
            return Mathf.CeilToInt(countdown);
        }
    }
}
