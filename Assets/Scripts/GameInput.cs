using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {
    public enum Binding {
        MoveUp,
        MoveDown,
        MoveRight,
        MoveLeft,
        Interact,
        AlternativeInteract,
        Pause,
    }
    
    private const string PLAYER_PREFS_BINDINGS = "BINDINGS";


    public static GameInput Instance { get; private set; }


    public event EventHandler OnInteract;
    public event EventHandler OnInteractAlternate;
    public event EventHandler OnPause;

    private InputSystemActions _inputSystemActions;


    public Vector2 GetPlayerMovementVectorNormalized() {
        return _inputSystemActions.Player.Move.ReadValue<Vector2>();
    }

    public string GetPlayerBindingDisplayString(Binding binding) {
        var bindingString = binding switch {
            Binding.MoveUp => _inputSystemActions.Player.Move.bindings[1].ToDisplayString(),
            Binding.MoveDown => _inputSystemActions.Player.Move.bindings[2].ToDisplayString(),
            Binding.MoveRight => _inputSystemActions.Player.Move.bindings[3].ToDisplayString(),
            Binding.MoveLeft => _inputSystemActions.Player.Move.bindings[4].ToDisplayString(),
            Binding.Interact => _inputSystemActions.Player.Interact.bindings[0].ToDisplayString(),
            Binding.AlternativeInteract => _inputSystemActions.Player.InteractAlternate.bindings[0].ToDisplayString(),
            Binding.Pause => _inputSystemActions.Player.Pause.bindings[0].ToDisplayString(),
            _ => throw new ArgumentOutOfRangeException(nameof(binding), binding, null)
        };
        return bindingString;
    }

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
                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, bindingsJson);
            })
            .Start();
    }


    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There is more than one GameInput instance!");
        }
        Instance = this;

        _inputSystemActions = new InputSystemActions();
        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS)) {
            var bindingsJson = PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS);
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
        OnInteract?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternatePerformed(InputAction.CallbackContext context) {
        OnInteractAlternate?.Invoke(this, EventArgs.Empty);
    }

    private void PausePerformed(InputAction.CallbackContext context) {
        OnPause?.Invoke(this, EventArgs.Empty);
    }
}
