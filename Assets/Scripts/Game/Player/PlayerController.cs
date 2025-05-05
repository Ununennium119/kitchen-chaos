using System;
using System.Diagnostics.CodeAnalysis;
using Common.Logic;
using Game.Counter.Logic;
using Game.KitchenObject;
using Game.Manager;
using Unity.Netcode;
using UnityEngine;
using Logger = Common.Utility.Logger;

namespace Game.Player {
    /// <summary>
    /// Manages player behavior including movement, interaction, and object handling.
    /// </summary>
    /// <remarks>There is only one player controller per player.</remarks>
    public class PlayerController : NetworkBehaviour, IKitchenObjectParent {
        public static PlayerController LocalInstance { get; private set; }

        /// <summary>
        /// This event is triggered whenever the local player network is spawned.
        /// </summary>
        public static event EventHandler OnLocalPlayerNetworkSpawned;

        /// <summary>
        /// This event is triggered whenever any player picks up an object.
        /// </summary>
        public static event EventHandler<OnAnyObjectPickupArgs> OnAnyObjectPickup;
        public class OnAnyObjectPickupArgs : EventArgs {
            public Vector3 Position;
        }

        /// <summary>
        /// This event is triggered whenever any player drops an object.
        /// </summary>
        public static event EventHandler<OnAnyObjectDropArgs> OnAnyObjectDrop;
        public class OnAnyObjectDropArgs : EventArgs {
            public Vector3 Position;
        }


        /// <summary>
        /// Resets the static objects, specifically the OnTrash event.
        /// This method is used to clean up the event subscription.
        /// </summary>
        public static void ResetStaticObjects() {
            OnLocalPlayerNetworkSpawned = null;
            OnAnyObjectPickup = null;
            OnAnyObjectDrop = null;
        }


        /// <summary>
        /// This event is triggered whenever the counter selected by the player changes.
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
        [SerializeField, Tooltip("The radius of the player (Used to check collision when moving)")]
        private float radius = 0.7f;

        [Header("Interaction")]
        [SerializeField, Tooltip("Maximum distance in which player can select and interact with things")]
        private float interactDistance = 2f;
        [SerializeField, Tooltip("The layer mask of the containers")]
        private LayerMask counterLayerMask;
        [SerializeField, Tooltip("The layer mask of the object which player should collide with")]
        private LayerMask collisionsLayerMask;

        [Header("Other")]
        [SerializeField, Tooltip("The position in which player holds its kitchen object")]
        private Transform holdPoint;
        [SerializeField, Tooltip("The position in which players are being spawned")]
        private Vector3[] spawnPositions;
        [SerializeField, Tooltip("The player visual")]
        private PlayerVisual playerVisual;


        /// <remarks>
        /// This field is only set in the server.
        /// </remarks>
        private GameManager _gameManager;

        /// <remarks>
        /// This field is only set in the owner.
        /// </remarks>
        private InputManager _inputManager;

        /// <remarks>
        /// This field is only updated in the owner.
        /// </remarks>
        private bool _isWalking;

        /// <remarks>
        /// This field is only updated in the server.
        /// </remarks>
        private KitchenObject.KitchenObject _kitchenObject;

        private MultiplayerManager _multiplayerManager;

        /// <summary>
        /// The selected counter.
        /// </summary>
        /// <remarks>
        /// This field is only set in the server.
        /// </remarks>
        private BaseCounter _selectedCounter;
        private readonly NetworkVariable<NetworkObjectReference> _selectedCounterRef = new();


        private void Start() {
            if (IsServer) {
                _gameManager = GameManager.Instance;
            }

            if (IsOwner) {
                _inputManager = InputManager.Instance;
                _inputManager.OnInteractPerformed += OnInteractPerformedAction;
                _inputManager.OnInteractAlternatePerformed += OnInteractAlternatePerformedAction;
            }

            var playerData = _multiplayerManager.GetPlayerData(OwnerClientId);
            var color = _multiplayerManager.GetPlayerColor(playerData.ColorIndex);
            playerVisual.SetColor(color);
        }

        private void Update() {
            if (IsOwner) {
                var movementVector = _inputManager.GetPlayerMovementVectorNormalized();
                SendMovementVectorServerRpc(movementVector);

                // Update walking
                _isWalking = movementVector != Vector2.zero;
            }
        }

        public override void OnNetworkSpawn() {
            _multiplayerManager = MultiplayerManager.Instance;
            if (IsServer) {
                var playerDataIndex = _multiplayerManager.GetPlayerDataIndex(OwnerClientId);
                transform.position = spawnPositions[playerDataIndex];
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallbackAction;
            }

            if (IsOwner) {
                Logger.LogInitializingInstance(this);
                if (LocalInstance != null) {
                    Logger.LogMultipleInstancesError(this);
                    Destroy(gameObject);
                    return;
                }
                LocalInstance = this;
                Logger.LogInstanceInitialized(this);

                OnLocalPlayerNetworkSpawned?.Invoke(this, EventArgs.Empty);
            }
        }


        /// <returns>true if player is walking</returns>
        /// <remarks>Only valid for the owner because <see cref="_isWalking"/> is only updated in the owner</remarks>
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
            _kitchenObject = kitchenObject;

            // Notify Clients
            if (kitchenObject is not null) {
                TriggerOnAnyObjectPickupClientRpc();
            }
        }

        /// <inheritdoc cref="IKitchenObjectParent.ClearKitchenObject"/>
        /// <remark>Implementation of <see cref="IKitchenObjectParent.ClearKitchenObject"/>.</remark>
        public void ClearKitchenObject() {
            _kitchenObject = null;

            // Notify Clients
            if (_kitchenObject is not null) {
                TriggerOnAnyObjectDropClientRpc();
            }
        }

        /// <inheritdoc cref="IKitchenObjectParent.HasKitchenObject"/>
        /// <remark>Implementation of <see cref="IKitchenObjectParent.HasKitchenObject"/>.</remark>
        public bool HasKitchenObject() {
            return _kitchenObject is not null;
        }

        /// <inheritdoc cref="IKitchenObjectParent.GetNetworkObject"/>
        /// <remark>Implementation of <see cref="IKitchenObjectParent.GetNetworkObject"/>.</remark>
        public NetworkObjectReference GetNetworkObject() {
            return NetworkObject;
        }


        /// <summary>
        /// Handles interaction input event.
        /// </summary>
        /// <remarks>
        /// Invoked when the <see cref="InputManager.OnInteractPerformed"/> event is triggered.
        /// </remarks>
        private void OnInteractPerformedAction(object sender, EventArgs e) {
            if (!_gameManager.IsPlaying()) return;

            InteractPerformedServerRpc();
        }

        /// <summary>
        /// Handles alternate interaction input event.
        /// </summary>
        /// <remarks>
        /// Invoked when the <see cref="InputManager.OnInteractAlternatePerformed"/> event is triggered.
        /// </remarks>
        private void OnInteractAlternatePerformedAction(object sender, EventArgs e) {
            if (!_gameManager.IsPlaying()) return;

            AlternateInteractPerformedServerRpc();
        }

        /// <summary>
        /// Destroys any held kitchen object.
        /// </summary>
        /// <remarks>
        /// Invoked when the <see cref="NetworkManager.OnClientDisconnectCallback"/> event is triggered.
        /// </remarks>
        private void OnClientDisconnectCallbackAction(ulong clientId) {
            if (OwnerClientId == clientId) {
                GetKitchenObject()?.DestroySelf();
            }
        }


        // --- SERVER LOGIC ---

        /// <summary>
        /// Sends movement vector to the server.
        /// </summary>
        /// <param name="movementVector">Movement vector.</param>
        [ServerRpc]
        private void SendMovementVectorServerRpc(Vector2 movementVector) {
            var movementDirection = new Vector3(movementVector.x, 0, movementVector.y);
            HandleMovement(movementDirection);
        }


        /// <summary>
        /// Handles player movement and collision logic.
        /// </summary>
        /// <param name="movementDirection">Direction of movement.</param>
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

            UpdateSelectedCounter();
        }

        /// <summary>
        /// Determines if the player can move in the given direction without hitting a collision.
        /// </summary>
        /// <param name="movement">Direction to test.</param>
        /// <returns>True if movement is allowed.</returns>
        private bool CanMove(Vector3 movement) {
            return !Physics.BoxCast(
                center: transform.position,
                halfExtents: Vector3.one * radius,
                direction: movement,
                orientation: Quaternion.identity,
                maxDistance: speed * Time.deltaTime,
                layerMask: collisionsLayerMask
            );
        }

        /// <summary>
        /// Updates which counter the player is currently targeting.
        /// </summary>
        private void UpdateSelectedCounter() {
            var didRaycastHit = Physics.Raycast(
                transform.position,
                transform.forward,
                out var hitInfo,
                interactDistance,
                counterLayerMask
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

        /// <summary>
        /// Sets the currently selected counter and triggers the corresponding event.
        /// </summary>
        /// <param name="counter">The counter to select.</param>
        private void SetSelectedCounter(BaseCounter counter) {
            _selectedCounter = counter;
            _selectedCounterRef.Value = counter
                ? counter.GetNetworkObject()
                : default;

            // Only for owner client
            TriggerOnSelectedCounterChangedClientRpc(
                new ClientRpcParams {
                    Send = new ClientRpcSendParams {
                        TargetClientIds = new[] { OwnerClientId }
                    }
                }
            );
        }


        /// <summary>
        /// Sends interact performed action to the server.
        /// </summary>
        [ServerRpc]
        private void InteractPerformedServerRpc() {
            if (!_gameManager.IsPlaying()) return;

            _selectedCounter?.Interact(this);
        }

        /// <summary>
        /// Sends alternate interact performed action to the server.
        /// </summary>
        [ServerRpc]
        private void AlternateInteractPerformedServerRpc() {
            if (!_gameManager.IsPlaying()) return;

            _selectedCounter?.InteractAlternate();
        }


        // --- CLIENT LOGIC ---

        /// <summary>
        /// Triggers <see cref="OnSelectedCounterChanged"/> for the owner client.
        /// </summary>
        /// <param name="rpcParams">Client RPC params.</param>
        [ClientRpc]
        private void TriggerOnSelectedCounterChangedClientRpc(
            [SuppressMessage("ReSharper", "UnusedParameter.Local")]
            ClientRpcParams rpcParams = default
        ) {
            if (!_selectedCounterRef.Value.TryGet(out var counterNetworkObject)) {
                counterNetworkObject = null;
            }
            var counter = counterNetworkObject?.GetComponent<BaseCounter>();
            OnSelectedCounterChanged?.Invoke(
                this,
                new OnSelectedCounterChangedArgs { SelectedCounter = counter }
            );
        }

        /// <summary>
        /// Triggers <see cref="OnAnyObjectPickup"/> for the owner client.
        /// </summary>
        [ClientRpc]
        private void TriggerOnAnyObjectPickupClientRpc() {
            OnAnyObjectPickup?.Invoke(
                this,
                new OnAnyObjectPickupArgs { Position = transform.position }
            );
        }

        /// <summary>
        /// Triggers <see cref="OnAnyObjectDrop"/> for the owner client.
        /// </summary>
        [ClientRpc]
        private void TriggerOnAnyObjectDropClientRpc() {
            OnAnyObjectDrop?.Invoke(
                this,
                new OnAnyObjectDropArgs { Position = transform.position }
            );
        }
    }
}
