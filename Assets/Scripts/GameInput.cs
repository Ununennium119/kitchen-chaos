using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {
    public event EventHandler OnInteract;


    private InputSystemActions _inputSystemActions;


    private void Awake() {
        _inputSystemActions = new InputSystemActions();
        _inputSystemActions.Enable();

        _inputSystemActions.Player.Interact.performed += InteractPerformed;
    }

    public Vector2 GetPlayerMovementVectorNormalized() {
        return _inputSystemActions.Player.Move.ReadValue<Vector2>();
    }


    private void InteractPerformed(InputAction.CallbackContext context) {
        OnInteract?.Invoke(this, EventArgs.Empty);
    }
}
