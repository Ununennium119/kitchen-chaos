using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Manager {
    /// <summary>This class is responsible for game inputs.</summary>
    /// <remarks>This class is singleton</remarks>
    public class InputManager : MonoBehaviour {
        public enum Binding {
            MoveUp,
            MoveDown,
            MoveRight,
            MoveLeft,
            Interact,
            AlternativeInteract,
            Pause,
            GamepadInteract,
            GamepadAlternativeInteract,
            GamepadPause,
        }


        public static InputManager Instance { get; private set; }


        public event EventHandler OnInteractPerformed;
        public event EventHandler OnInteractAlternatePerformed;
        public event EventHandler OnPausePerformed;

        public event EventHandler OnRebind;

        private InputSystemActions _inputSystemActions;


        /// <returns>Normalized vector of player movement input.</returns>
        public Vector2 GetPlayerMovementVectorNormalized() {
            return _inputSystemActions.Player.Move.ReadValue<Vector2>();
        }

        /// <param name="binding">The binding</param>
        /// <returns>Display string of the binding</returns>
        /// <exception cref="ArgumentOutOfRangeException">If binding is null</exception>
        public string GetPlayerBindingDisplayString(Binding binding) {
            var bindingString = binding switch {
                Binding.MoveUp => _inputSystemActions.Player.Move.bindings[1].ToDisplayString(),
                Binding.MoveDown => _inputSystemActions.Player.Move.bindings[2].ToDisplayString(),
                Binding.MoveRight => _inputSystemActions.Player.Move.bindings[3].ToDisplayString(),
                Binding.MoveLeft => _inputSystemActions.Player.Move.bindings[4].ToDisplayString(),
                Binding.Interact => _inputSystemActions.Player.Interact.bindings[0].ToDisplayString(),
                Binding.AlternativeInteract => _inputSystemActions.Player.InteractAlternate.bindings[0]
                    .ToDisplayString(),
                Binding.Pause => _inputSystemActions.Player.Pause.bindings[0].ToDisplayString(),
                Binding.GamepadInteract => _inputSystemActions.Player.Interact.bindings[1].ToDisplayString(),
                Binding.GamepadAlternativeInteract => _inputSystemActions.Player.InteractAlternate.bindings[1]
                    .ToDisplayString(),
                Binding.GamepadPause => _inputSystemActions.Player.Pause.bindings[1].ToDisplayString(),
                _ => throw new ArgumentOutOfRangeException(nameof(binding), binding, null)
            };
            return bindingString;
        }

        /// <summary>
        /// Sets the player binding using interactive rebinding.
        /// </summary>
        /// <param name="binding">The binding</param>
        /// <param name="onRebindCompleteAction">The action which is called after successful rebinding</param>
        /// <exception cref="ArgumentOutOfRangeException">if binding is null</exception>
        public void SetPlayerBinding(Binding binding, Action onRebindCompleteAction = null) {
            InputAction inputAction;
            int bindingIndex;
            switch (binding) {
                case Binding.MoveUp:
                    inputAction = _inputSystemActions.Player.Move;
                    bindingIndex = 1;
                    break;
                case Binding.MoveDown:
                    inputAction = _inputSystemActions.Player.Move;
                    bindingIndex = 2;
                    break;
                case Binding.MoveRight:
                    inputAction = _inputSystemActions.Player.Move;
                    bindingIndex = 3;
                    break;
                case Binding.MoveLeft:
                    inputAction = _inputSystemActions.Player.Move;
                    bindingIndex = 4;
                    break;
                case Binding.Interact:
                    inputAction = _inputSystemActions.Player.Interact;
                    bindingIndex = 0;
                    break;
                case Binding.AlternativeInteract:
                    inputAction = _inputSystemActions.Player.InteractAlternate;
                    bindingIndex = 0;
                    break;
                case Binding.Pause:
                    inputAction = _inputSystemActions.Player.Pause;
                    bindingIndex = 0;
                    break;
                case Binding.GamepadInteract:
                    inputAction = _inputSystemActions.Player.Interact;
                    bindingIndex = 1;
                    break;
                case Binding.GamepadAlternativeInteract:
                    inputAction = _inputSystemActions.Player.InteractAlternate;
                    bindingIndex = 1;
                    break;
                case Binding.GamepadPause:
                    inputAction = _inputSystemActions.Player.Pause;
                    bindingIndex = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(binding), binding, null);
            }

            _inputSystemActions.Disable();
            inputAction.PerformInteractiveRebinding(bindingIndex)
                .OnCancel(callback => {
                    callback.Dispose();
                    _inputSystemActions.Enable();
                })
                .OnComplete(callback => {
                    callback.Dispose();
                    _inputSystemActions.Enable();
                    onRebindCompleteAction?.Invoke();

                    var bindingsJson = _inputSystemActions.SaveBindingOverridesAsJson();
                    PlayerPrefsManager.SetPlayerBindingsJsonString(bindingsJson);

                    OnRebind?.Invoke(this, EventArgs.Empty);
                })
                .Start();
        }


        private void Awake() {
            Debug.Log("Settings up InputManager...");
            if (Instance != null) {
                Debug.LogError("There is more than one InputManager instance!");
            }
            Instance = this;

            _inputSystemActions = new InputSystemActions();
            if (PlayerPrefsManager.HasPlayerBindingsJsonString()) {
                var bindingsJson = PlayerPrefsManager.GetPlayerBindingsJsonString();
                _inputSystemActions.LoadBindingOverridesFromJson(bindingsJson);
            }
            _inputSystemActions.Enable();

            _inputSystemActions.Player.Interact.performed += InteractPerformed;
            _inputSystemActions.Player.InteractAlternate.performed += InteractAlternatePerformed;
            _inputSystemActions.Player.Pause.performed += PausePerformed;
        }

        private void OnDestroy() {
            _inputSystemActions.Player.Interact.performed -= InteractPerformed;
            _inputSystemActions.Player.InteractAlternate.performed -= InteractAlternatePerformed;
            _inputSystemActions.Player.Pause.performed -= PausePerformed;

            _inputSystemActions.Dispose();
        }


        private void InteractPerformed(InputAction.CallbackContext context) {
            OnInteractPerformed?.Invoke(this, EventArgs.Empty);
        }

        private void InteractAlternatePerformed(InputAction.CallbackContext context) {
            OnInteractAlternatePerformed?.Invoke(this, EventArgs.Empty);
        }

        private void PausePerformed(InputAction.CallbackContext context) {
            OnPausePerformed?.Invoke(this, EventArgs.Empty);
        }
    }
}
