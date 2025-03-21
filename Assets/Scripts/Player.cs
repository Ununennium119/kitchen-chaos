using System;
using UnityEngine;

public class Player : MonoBehaviour {
    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedArgs : EventArgs {
        public ClearCounter SelectedCounter;
    }

    [Header("Player Stats")]
    [SerializeField] private float velocity = 7.5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float height = 2f;
    [SerializeField] private float radius = 0.7f;
    [SerializeField] private float interactDistance = 2f;

    [Header("Input & Detection")]
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask clearCounterLayer;


    private bool _isWalking;
    private ClearCounter _selectedCounter;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Debug.LogError("There are multiple instances of Player!");
        }
    }

    private void Start() {
        gameInput.OnInteract += OnInteractAction;
    }

    private void Update() {
        // Calculate movement direction
        var movementVector = gameInput.GetPlayerMovementVectorNormalized();
        var movementDirection = new Vector3(movementVector.x, 0, movementVector.y);
        _isWalking = movementDirection != Vector3.zero;

        HandleMovement(movementDirection);
        HandleInteraction();
    }


    public bool IsWalking() {
        return _isWalking;
    }


    private void HandleMovement(Vector3 movementDirection) {
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

    private bool CanMove(Vector3 movement) {
        return !Physics.CapsuleCast(
            point1: transform.position,
            point2: transform.position + height * Vector3.up,
            radius: radius,
            direction: movement,
            maxDistance: velocity * Time.deltaTime
        );
    }

    private void HandleInteraction() {
        var didRaycastHit = Physics.Raycast(
            transform.position,
            transform.forward,
            out var hitInfo,
            interactDistance,
            clearCounterLayer
        );
        if (!didRaycastHit) {
            SetSelectedCounter(null);
            return;
        }
        if (!hitInfo.transform.TryGetComponent(out ClearCounter clearCounter)) {
            SetSelectedCounter(null);
            return;
        }

        SetSelectedCounter(clearCounter);
    }

    private void SetSelectedCounter(ClearCounter clearCounter) {
        _selectedCounter = clearCounter;
        OnSelectedCounterChanged?.Invoke(
            this,
            new OnSelectedCounterChangedArgs { SelectedCounter = _selectedCounter }
        );
    }

    private void OnInteractAction(object sender, EventArgs e) {
        _selectedCounter?.Interact();
    }
}
