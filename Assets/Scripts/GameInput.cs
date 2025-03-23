using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {
    public static GameInput Instance { get; private set; }


    public event EventHandler OnInteract;
    public event EventHandler OnInteractAlternate;
    public event EventHandler OnPause;

    private InputSystemActions _inputSystemActions;


    public Vector2 GetPlayerMovementVectorNormalized() {
        return _inputSystemActions.Player.Move.ReadValue<Vector2>();
    }


    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There is more than one GameInput instance!");
        }
        Instance = this;

        _inputSystemActions = new InputSystemActions();
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
