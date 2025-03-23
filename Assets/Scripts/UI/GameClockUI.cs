using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class GameClockUI : MonoBehaviour {
        [SerializeField] private Image clockImage;


        private readonly GameManager.State[] _activeStates = { GameManager.State.Playing, GameManager.State.Countdown };
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
            var isActive = _activeStates.Contains(e.State);
            gameObject.SetActive(isActive);
        }
    }
}
