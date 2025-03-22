using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {
    public event EventHandler OnInteract;
    public event EventHandler OnInteractAlternate;

    private InputSystemActions _inputSystemActions;


    public Vector2 GetPlayerMovementVectorNormalized() {
        return _inputSystemActions.Player.Move.ReadValue<Vector2>();
    }


    private void Awake() {
        _inputSystemActions = new InputSystemActions();
        _inputSystemActions.Enable();

        _inputSystemActions.Player.Interact.performed += InteractPerformed;
        _inputSystemActions.Player.InteractAlternate.performed += InteractAlternatePerformed;
    }


    private void InteractPerformed(InputAction.CallbackContext context) {
        OnInteract?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternatePerformed(InputAction.CallbackContext context) {
        OnInteractAlternate?.Invoke(this, EventArgs.Empty);
    }
}
