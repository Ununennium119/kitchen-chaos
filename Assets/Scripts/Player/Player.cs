using System;
using Counter;
using KitchenObject;
using UnityEngine;

namespace Player {
    public class Player : MonoBehaviour, IKitchenObjectParent {
        public static Player Instance { get; private set; }


        public event EventHandler<OnSelectedCounterChangedArgs> OnSelectedCounterChanged;

        public class OnSelectedCounterChangedArgs : EventArgs {
            public BaseCounter SelectedCounter;
        }

        public event EventHandler OnObjectPickup;
        public event EventHandler OnObjectDrop;


        [Header("Player Stats")]
        [SerializeField] private float velocity = 7.5f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float height = 2f;
        [SerializeField] private float radius = 0.7f;
        [SerializeField] private float interactDistance = 2f;

        [Header("Input & Detection")]
        [SerializeField] private GameInput gameInput;
        [SerializeField] private LayerMask counterLayer;

        [Header("Other")]
        [SerializeField] private Transform holdPoint;


        private GameManager _gameManager;
        private bool _isWalking;
        private BaseCounter _selectedCounter;
        private KitchenObject.KitchenObject _kitchenObject;


        public bool IsWalking() {
            return _isWalking;
        }


        public Transform GetKitchenObjectFollowTransform() {
            return holdPoint;
        }

        public KitchenObject.KitchenObject GetKitchenObject() {
            return _kitchenObject;
        }

        public void SetKitchenObject(KitchenObject.KitchenObject kitchenObject) {
            if (kitchenObject is not null) {
                OnObjectPickup?.Invoke(this, EventArgs.Empty);
            }
            _kitchenObject = kitchenObject;
        }

        public void ClearKitchenObject() {
            if (_kitchenObject is not null) {
                OnObjectDrop?.Invoke(this, EventArgs.Empty);
            }
            _kitchenObject = null;
        }

        public bool HasKitchenObject() {
            return _kitchenObject is not null;
        }


        private void Awake() {
            if (Instance == null) {
                Instance = this;
            } else {
                Debug.LogError("There are multiple instances of Player!");
            }
        }

        private void Start() {
            _gameManager = GameManager.Instance;

            gameInput.OnInteract += OnInteractAction;
            gameInput.OnInteractAlternate += OnInteractAlternateAction;
        }

        private void Update() {
            // Calculate movement direction
            var movementVector = gameInput.GetPlayerMovementVectorNormalized();
            var movementDirection = new Vector3(movementVector.x, 0, movementVector.y);
            _isWalking = movementDirection != Vector3.zero;

            HandleMovement(movementDirection);
            HandleInteraction();
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
                counterLayer
            );
            if (!didRaycastHit) {
                SetSelectedCounter(null);
                return;
            }
            if (!hitInfo.transform.TryGetComponent(out BaseCounter counter)) {
                SetSelectedCounter(null);
                return;
            }

            SetSelectedCounter(counter);
        }


        private void SetSelectedCounter(BaseCounter counter) {
            _selectedCounter = counter;
            OnSelectedCounterChanged?.Invoke(
                this,
                new OnSelectedCounterChangedArgs { SelectedCounter = _selectedCounter }
            );
        }


        private void OnInteractAction(object sender, EventArgs e) {
            if (!_gameManager.IsPlaying()) return;

            _selectedCounter?.Interact(this);
        }

        private void OnInteractAlternateAction(object sender, EventArgs e) {
            if (!_gameManager.IsPlaying()) return;

            _selectedCounter?.InteractAlternate();
        }
    }
}
