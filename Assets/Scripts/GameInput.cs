using UnityEngine;

public class GameInput : MonoBehaviour
{
    private InputSystemActions _inputSystemActions;


    private void Awake() {
        _inputSystemActions = new InputSystemActions();
        _inputSystemActions.Enable();
    }

    public Vector2 GetPlayerMovementVectorNormalized() {
        return _inputSystemActions.Player.Move.ReadValue<Vector2>();
    }
}