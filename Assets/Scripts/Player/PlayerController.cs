using System;
using Counter.Logic;
using KitchenObject;
using Manager;
using Unity.Netcode;
using UnityEngine;

namespace Player {
    /// <remarks>There is only one player controller per player.</remarks>
    public class PlayerController : NetworkBehaviour, IKitchenObjectParent {
        public static PlayerController LocalInstance { get; private set; }

        /// <summary>
        /// This event is invoked whenever the local player network is spawned.
        /// </summary>
        public static event EventHandler OnLocalPlayerNetworkSpawned;

        /// <summary>
        /// This event is invoked whenever any player picks up an object.
        /// </summary>
        public static event EventHandler<OnAnyObjectPickupArgs> OnAnyObjectPickup;
        public class OnAnyObjectPickupArgs : EventArgs {
            public Vector3 Position;
        }

        /// <summary>
        /// This event is invoked whenever any player drops an object.
        /// </summary>
        public static event EventHandler<OnAnyObjectDropArgs> OnAnyObjectDrop;
        public class OnAnyObjectDropArgs : EventArgs {
            public Vector3 Position;
        }


        public static void ResetStaticObjects() {
            OnLocalPlayerNetworkSpawned = null;
            OnAnyObjectPickup = null;
            OnAnyObjectDrop = null;
        }


        /// <summary>
        /// This event is invoked whenever the counter selected by the player changes.
        /// </summary>
        public event EventHandler<OnSelectedCounterChangedArgs> OnSelectedCounterChanged;
        public class OnSelectedCounterChangedArgs : EventArgs {
            public BaseCounter SelectedCounter;
        }


        [Header("Movement")]
        [SerializeField, Tooltip("The speed of the player's movement")]
        private float speed = 7.5f;
        [SerializeField, Tooltip("The speed of the player's rotation")]
        private float rotationSpeed = 10f;
        [SerializeField, Tooltip("The height of the player (Used to check collision when moving)")]
        private float height = 2f;
        [SerializeField, Tooltip("The radius of the player (Used to check collision when moving)")]
        private float radius = 0.7f;

        [Header("Interaction")]
        [SerializeField, Tooltip("Maximum distance in which player can select and interact with things")]
        private float interactDistance = 2f;
        [SerializeField, Tooltip("The layer of the containers")]
        private LayerMask counterLayer;

        [Header("Other")]
        [SerializeField, Tooltip("The position in which player holds its kitchen object")]
        private Transform holdPoint;


        private GameManager _gameManager;
        private InputManager _inputManager;
        private bool _isWalking;
        private BaseCounter _selectedCounter;
        private KitchenObject.KitchenObject _kitchenObject;


        /// <returns>true if player is walking</returns>
        public bool IsWalking() {
            return _isWalking;
        }


        /// <inheritdoc cref="IKitchenObjectParent.GetKitchenObjectFollowTransform"/>
        /// <remark>Implementation of <see cref="IKitchenObjectParent.GetKitchenObjectFollowTransform"/>.</remark>
        public Transform GetKitchenObjectFollowTransform() {
            return holdPoint;
        }

        /// <inheritdoc cref="IKitchenObjectParent.GetKitchenObject"/>
        /// <remark>Implementation of <see cref="IKitchenObjectParent.GetKitchenObject"/>.</remark>
        public KitchenObject.KitchenObject GetKitchenObject() {
            return _kitchenObject;
        }

        /// <inheritdoc cref="IKitchenObjectParent.SetKitchenObject"/>
        /// <remark>Implementation of <see cref="IKitchenObjectParent.SetKitchenObject"/>.</remark>
        public void SetKitchenObject(KitchenObject.KitchenObject kitchenObject) {
            if (kitchenObject is not null) {
                OnAnyObjectPickup?.Invoke(this, new OnAnyObjectPickupArgs { Position = transform.position });
            }
            _kitchenObject = kitchenObject;
        }

        /// <inheritdoc cref="IKitchenObjectParent.ClearKitchenObject"/>
        /// <remark>Implementation of <see cref="IKitchenObjectParent.ClearKitchenObject"/>.</remark>
        public void ClearKitchenObject() {
            if (_kitchenObject is not null) {
                OnAnyObjectDrop?.Invoke(this, new OnAnyObjectDropArgs { Position = transform.position });
            }
            _kitchenObject = null;
        }

        /// <inheritdoc cref="IKitchenObjectParent.HasKitchenObject"/>
        /// <remark>Implementation of <see cref="IKitchenObjectParent.HasKitchenObject"/>.</remark>
        public bool HasKitchenObject() {
            return _kitchenObject is not null;
        }


        private void Start() {
            _gameManager = GameManager.Instance;
            _inputManager = InputManager.Instance;

            _inputManager.OnInteractPerformed += OnInteractPerformedAction;
            _inputManager.OnInteractAlternatePerformed += OnInteractAlternatePerformedAction;
        }

        private void Update() {
            if (!IsOwner) return;

            // Calculate movement direction and update walking
            var movementVector = _inputManager.GetPlayerMovementVectorNormalized();
            var movementDirection = new Vector3(movementVector.x, 0, movementVector.y);
            _isWalking = movementDirection != Vector3.zero;

            HandleMovement(movementDirection);
            UpdateSelectedCounter();
        }

        public override void OnNetworkSpawn() {
            if (!IsOwner) return;

            Debug.Log("Setting up PlayerController...");
            if (LocalInstance != null) {
                Debug.LogError("There is more than one PlayerController in the scene!");
            }
            LocalInstance = this;
            OnLocalPlayerNetworkSpawned?.Invoke(this, EventArgs.Empty);
        }


        private void HandleMovement(Vector3 movementDirection) {
            // Rotate
            transform.forward = Vector3.Slerp(transform.forward, movementDirection, rotationSpeed * Time.deltaTime);

            // Check collision and change movement direction based on it
            if (!CanMove(movementDirection)) {
                // Cannot move towards movement direction
                // Test movement on the x-axis
                var xMovementDirection = new Vector3(movementDirection.x, 0, 0).normalized;
                if (movementDirection.x is > 0.5f or < -0.5f && CanMove(xMovementDirection)) {
                    // Can only move on the x-axis
                    movementDirection = xMovementDirection;
                } else {
                    // Test movement on the z-axis
                    var zMovementDirection = new Vector3(0, 0, movementDirection.z).normalized;
                    if (movementDirection.z is < -0.5f or > 0.5f && CanMove(zMovementDirection)) {
                        // Can only move on the z-axis
                        movementDirection = zMovementDirection;
                    } else {
                        // Cannot move in any direction
                        movementDirection = Vector3.zero;
                    }
                }
            }

            // Move
            var movement = movementDirection * (speed * Time.deltaTime);
            transform.position += movement;
        }

        private bool CanMove(Vector3 movement) {
            return !Physics.CapsuleCast(
                point1: transform.position,
                point2: transform.position + height * Vector3.up,
                radius: radius,
                direction: movement,
                maxDistance: speed * Time.deltaTime
            );
        }

        private void UpdateSelectedCounter() {
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


        private void OnInteractPerformedAction(object sender, EventArgs e) {
            if (!_gameManager.IsPlaying()) return;

            _selectedCounter?.Interact(this);
        }

        private void OnInteractAlternatePerformedAction(object sender, EventArgs e) {
            if (!_gameManager.IsPlaying()) return;

            _selectedCounter?.InteractAlternate();
        }
    }
}
