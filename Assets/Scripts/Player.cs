using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float velocity = 7.5f;
    [SerializeField] private float rotationSpeed = 10f;


    private bool _isWalking;


    private void Update() {
        // Get input
        var inputVector = new Vector2();
        if (Input.GetKey(KeyCode.D)) inputVector.x = 1;
        if (Input.GetKey(KeyCode.A)) inputVector.x = -1;
        if (Input.GetKey(KeyCode.W)) inputVector.y = 1;
        if (Input.GetKey(KeyCode.S)) inputVector.y = -1;
        inputVector.Normalize();

        // Calculate movement direction
        var movementDirection = new Vector3(inputVector.x, 0, inputVector.y);
        _isWalking = movementDirection != Vector3.zero;

        // Move
        var movement = movementDirection * (velocity * Time.deltaTime);
        transform.position += movement;

        // Rotate
        transform.forward = Vector3.Slerp(transform.forward, movementDirection, rotationSpeed * Time.deltaTime);
    }


    public bool IsWalking() {
        return _isWalking;
    }
}