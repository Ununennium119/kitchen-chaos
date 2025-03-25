using System;
using TMPro;
using UnityEngine;

namespace UI {
    public class TutorialUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI keyboardMoveUpText;
        [SerializeField] private TextMeshProUGUI keyboardMoveLeftText;
        [SerializeField] private TextMeshProUGUI keyboardMoveDownText;
        [SerializeField] private TextMeshProUGUI keyboardMoveRightText;
        [SerializeField] private TextMeshProUGUI keyboardInteractText;
        [SerializeField] private TextMeshProUGUI keyboardAlternativeInteractText;
        [SerializeField] private TextMeshProUGUI keyboardPauseText;
        [SerializeField] private TextMeshProUGUI gamepadInteractText;
        [SerializeField] private TextMeshProUGUI gamepadAlternativeInteractText;
        [SerializeField] private TextMeshProUGUI gamepadPauseText;


        private GameManager _gameManager;
        private GameInput _gameInput;


        private void Start() {
            _gameManager = GameManager.Instance;
            _gameInput = GameInput.Instance;

            _gameManager.OnStateChanged += OnStateChangedAction;
            _gameInput.OnRebind += OnRebindAction;

            UpdateInputTexts();
        }


        private void Show() {
            gameObject.SetActive(true);
        }

        private void Hide() {
            gameObject.SetActive(false);
        }

        private void UpdateInputTexts() {
            keyboardMoveUpText.text = _gameInput.GetPlayerBindingDisplayString(GameInput.Binding.MoveUp);
            keyboardMoveLeftText.text = _gameInput.GetPlayerBindingDisplayString(GameInput.Binding.MoveLeft);
            keyboardMoveDownText.text = _gameInput.GetPlayerBindingDisplayString(GameInput.Binding.MoveDown);
            keyboardMoveRightText.text = _gameInput.GetPlayerBindingDisplayString(GameInput.Binding.MoveUp);
            keyboardInteractText.text = _gameInput.GetPlayerBindingDisplayString(GameInput.Binding.Interact);
            keyboardAlternativeInteractText.text =
                _gameInput.GetPlayerBindingDisplayString(GameInput.Binding.AlternativeInteract);
            keyboardPauseText.text = _gameInput.GetPlayerBindingDisplayString(GameInput.Binding.Pause);
            gamepadInteractText.text = _gameInput.GetPlayerBindingDisplayString(GameInput.Binding.GamepadInteract);
            gamepadAlternativeInteractText.text =
                _gameInput.GetPlayerBindingDisplayString(GameInput.Binding.GamepadAlternativeInteract);
            gamepadPauseText.text = _gameInput.GetPlayerBindingDisplayString(GameInput.Binding.GamepadPause);
        }


        private void OnStateChangedAction(object sender, GameManager.OnStateChangedArgs e) {
            if (e.State == GameManager.State.WaitingToStart) {
                Show();
            } else {
                Hide();
            }
        }

        private void OnRebindAction(object sender, EventArgs e) {
            UpdateInputTexts();
        }
    }
}
