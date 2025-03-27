using System.Linq;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD {
    public class GameClockUI : MonoBehaviour {
        [SerializeField, Tooltip("The image of the clock used to show remaining time")]
        private Image clockImage;


        private GameManager _gameManager;


        private void Start() {
            _gameManager = GameManager.Instance;

            _gameManager.OnStateChanged += OnStateChangedAction;
        }

        private void Update() {
            var remainingGameTime = _gameManager.GetRemainingGameTimeNormalized();
            clockImage.fillAmount = remainingGameTime;
        }


        private void OnStateChangedAction(object sender, GameManager.OnStateChangedArgs e) {
            var isActive = new[] { GameManager.State.Playing, GameManager.State.Countdown }.Contains(e.State);
            gameObject.SetActive(isActive);
        }
    }
}
