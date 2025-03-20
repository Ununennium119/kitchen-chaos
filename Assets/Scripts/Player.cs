using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] private GameInput gameInput;
    [SerializeField] private float velocity = 7.5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float height = 2f;
    [SerializeField] private float radius = 0.7f;


    private bool _isWalking;


    private void Update() {
        var movementVector = gameInput.GetPlayerMovementVectorNormalized();

        // Calculate movement direction
        var movementDirection = new Vector3(movementVector.x, 0, movementVector.y);
        _isWalking = movementDirection != Vector3.zero;

        // Rotate
        transform.forward = Vector3.Slerp(transform.forward, movementDirection, rotationSpeed * Time.deltaTime);

        // Check collision
        if (!CanMove(movementDirection)) {
            // Cannot move towards movement direction
            // Test movement on the x-axis
            var xMovementDirection = new Vector3(movementDirection.x, 0, 0).normalized;
            if (CanMove(xMovementDirection)) {
                // Can only move on the x-axis
                movementDirection = xMovementDirection;
            } else {
                // Test movement on the z-axis
                var zMovementDirection = new Vector3(0, 0, movementDirection.z).normalized;
                if (CanMove(zMovementDirection)) {
                    // Can only move on the z-axis
                    movementDirection = zMovementDirection;
                } else {
                    // Cannot move in any direction
                    movementDirection = Vector3.zero;
                }
            }
        }

        // Move
        var movement = movementDirection * (velocity * Time.deltaTime);
        transform.position += movement;
    }


    public bool IsWalking() {
        return _isWalking;
    }


    private bool CanMove(Vector3 movement) {
        return !Physics.CapsuleCast(
            point1: transform.position,
            point2: transform.position + height * Vector3.up,
            radius: radius,
            direction: movement,
            maxDistance: velocity * Time.deltaTime
        );
    }
}
