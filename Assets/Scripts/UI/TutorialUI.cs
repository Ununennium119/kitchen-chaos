using System;
using Manager;
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
        private InputManager _inputManager;


        private void Start() {
            _gameManager = GameManager.Instance;
            _inputManager = InputManager.Instance;

            _gameManager.OnStateChanged += OnGameStateChangedAction;
            _gameManager.OnLocalPlayerReadyChanged += OnLocalPlayerReadyChangedAction;
            _inputManager.OnRebind += OnRebindAction;

            UpdateInputTexts();
        }


        private void Show() {
            gameObject.SetActive(true);
        }

        private void Hide() {
            gameObject.SetActive(false);
        }

        private void UpdateInputTexts() {
            keyboardMoveUpText.text = _inputManager.GetPlayerBindingDisplayString(InputManager.Binding.MoveUp);
            keyboardMoveLeftText.text = _inputManager.GetPlayerBindingDisplayString(InputManager.Binding.MoveLeft);
            keyboardMoveDownText.text = _inputManager.GetPlayerBindingDisplayString(InputManager.Binding.MoveDown);
            keyboardMoveRightText.text = _inputManager.GetPlayerBindingDisplayString(InputManager.Binding.MoveUp);
            keyboardInteractText.text = _inputManager.GetPlayerBindingDisplayString(InputManager.Binding.Interact);
            keyboardAlternativeInteractText.text =
                _inputManager.GetPlayerBindingDisplayString(InputManager.Binding.AlternativeInteract);
            keyboardPauseText.text = _inputManager.GetPlayerBindingDisplayString(InputManager.Binding.Pause);
            gamepadInteractText.text =
                _inputManager.GetPlayerBindingDisplayString(InputManager.Binding.GamepadInteract);
            gamepadAlternativeInteractText.text =
                _inputManager.GetPlayerBindingDisplayString(InputManager.Binding.GamepadAlternativeInteract);
            gamepadPauseText.text = _inputManager.GetPlayerBindingDisplayString(InputManager.Binding.GamepadPause);
        }


        private void OnGameStateChangedAction(object sender, GameManager.OnStateChangedArgs e) {
            if (e.State == GameManager.State.WaitingToStart) {
                Show();
            } else {
                Hide();
            }
        }

        private void OnLocalPlayerReadyChangedAction(object sender, GameManager.OnLocalPlayerReadyChangedArgs e) {
            if (e.IsLocalPlayerReady) {
                Hide();
            }
        }

        private void OnRebindAction(object sender, EventArgs e) {
            UpdateInputTexts();
        }
    }
}
