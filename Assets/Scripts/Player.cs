using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float velocity = 7.5f;
    [SerializeField] private float rotationSpeed = 10f;

    private void Update() {
        // Get input
        var inputVector = new Vector2();
        if (Input.GetKey(KeyCode.D)) inputVector.x = 1;
        if (Input.GetKey(KeyCode.A)) inputVector.x = -1;
        if (Input.GetKey(KeyCode.W)) inputVector.y = 1;
        if (Input.GetKey(KeyCode.S)) inputVector.y = -1;
        inputVector.Normalize();

        var movementDirection = new Vector3(inputVector.x, 0, inputVector.y);

        // Move
        var movement = movementDirection * (velocity * Time.deltaTime);
        transform.position += movement;

        // Rotate
        transform.forward = Vector3.Slerp(transform.forward, movementDirection, rotationSpeed * Time.deltaTime);
    }
}