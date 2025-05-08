using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = Common.Utility.Logger;

namespace Game.Manager {
    /// <summary>This class is responsible for managing game state.</summary>
    /// <remarks>This class is singleton.</remarks>
    public class GameManager : NetworkBehaviour {
        public enum State {
            WaitingToStart,
            Countdown,
            Playing,
            GameOver
        }


        public static GameManager Instance { get; private set; }


        /// <summary>
        /// This event is triggered whenever the game state changes.
        /// </summary>
        public event EventHandler<OnStateChangedArgs> OnStateChanged;
        public class OnStateChangedArgs : EventArgs {
            public State State;
        }

        /// <summary>
        /// This event is triggered whenever the local player toggles the pause state.
        /// </summary>
        public event EventHandler<OnLocalPauseToggledArgs> OnLocalPauseToggled;
        public class OnLocalPauseToggledArgs : EventArgs {
            public bool IsGamePaused;
        }

        /// <summary>
        /// This event is triggered whenever the local player's ready state changes.
        /// </summary>
        public event EventHandler<OnLocalPlayerReadyChangedArgs> OnLocalPlayerReadyChanged;
        public class OnLocalPlayerReadyChangedArgs : EventArgs {
            public bool IsLocalPlayerReady;
        }


        [SerializeField, Tooltip("The player prefab")]
        private Transform playerPrefab;

        [SerializeField, Tooltip("Duration of countdown")]
        private float countdownDuration = 3f;
        [SerializeField, Tooltip("Duration of game in \"Playing\" state")]
        private float playDuration = 60f;

        private InputManager _inputManager;

        private readonly Dictionary<ulong, bool> _playerReadyDictionary = new();
        private readonly NetworkVariable<State> _state = new();
        private readonly NetworkVariable<float> _currentCountdownTime = new();
        private readonly NetworkVariable<float> _currentPlayTime = new();

        private bool _isLocalPlayerReady;
        private bool _isLocalGamePaused = false;


        private void Awake() {
            Logger.LogInitializingInstance(this);
            if (Instance != null) {
                Logger.LogMultipleInstancesError(this);
                Destroy(gameObject);
                return;
            }
            Instance = this;
            Logger.LogInstanceInitialized(this);

            _isLocalGamePaused = false;
        }

        private void Start() {
            _inputManager = InputManager.Instance;

            _inputManager.OnPausePerformed += OnPausePerformedAction;
            _inputManager.OnInteractPerformed += OnInteractPerformedAction;
        }

        public override void OnNetworkSpawn() {
            _state.OnValueChanged += OnStateValueChangedAction;

            if (IsServer) {
                _currentCountdownTime.Value = countdownDuration;
                _currentPlayTime.Value = playDuration;
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompletedAction;
            }
        }

        private void Update() {
            if (!IsServer) return;

            switch (_state.Value) {
                case State.WaitingToStart:
                    // Do Nothing
                    break;
                case State.Countdown:
                    _currentCountdownTime.Value -= Time.deltaTime;
                    if (_currentCountdownTime.Value <= 0) {
                        _state.Value = State.Playing;
                        _currentPlayTime.Value = playDuration;
                    }
                    break;
                case State.Playing:
                    _currentPlayTime.Value -= Time.deltaTime;
                    if (_currentPlayTime.Value <= 0) {
                        _state.Value = State.GameOver;
                    }
                    break;
                case State.GameOver:
                    Time.timeScale = 0f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        /// <returns>Current countdown time</returns>
        public float GetCountdownTime() {
            return _currentCountdownTime.Value;
        }

        /// <returns>true if game is in <see cref="State.Playing"/> state.</returns>
        public bool IsPlaying() {
            return _state.Value == State.Playing;
        }

        /// <returns>Normalized (between 0 and 1) game time.</returns>
        public float GetRemainingGameTimeNormalized() {
            return _currentPlayTime.Value / playDuration;
        }

        /// <summary>
        /// Toggles game pause if game is not in <see cref="State.GameOver"/> status.
        /// </summary>
        public void ToggleGamePause() {
            if (_state.Value == State.GameOver) return;

            _isLocalGamePaused = !_isLocalGamePaused;
            OnLocalPauseToggled?.Invoke(this, new OnLocalPauseToggledArgs { IsGamePaused = _isLocalGamePaused });
        }

        /// <summary>
        /// Determines whether the local game is currently paused.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the local game is paused; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLocalGamePaused() {
            return _isLocalGamePaused;
        }


        /// <remarks>
        /// Invoked when the <see cref="InputManager.OnPausePerformed"/> event is triggered.
        /// </remarks>
        private void OnPausePerformedAction(object sender, EventArgs e) {
            ToggleGamePause();
        }

        /// <remarks>
        /// Invoked when the <see cref="InputManager.OnInteractPerformed"/> event is triggered.
        /// </remarks>
        private void OnInteractPerformedAction(object sender, EventArgs e) {
            if (_state.Value == State.WaitingToStart && !_isLocalGamePaused) {
                _isLocalPlayerReady = true;
                OnLocalPlayerReadyChanged?.Invoke(
                    this,
                    new OnLocalPlayerReadyChangedArgs { IsLocalPlayerReady = _isLocalPlayerReady }
                );
                SetPlayerReadyServerRpc();
            }
        }

        /// <remarks>
        /// Invoked when the <see cref="_state"/> value changes.
        /// </remarks>
        private void OnStateValueChangedAction(State previousValue, State newValue) {
            OnStateChanged?.Invoke(this, new OnStateChangedArgs { State = newValue });
        }

        /// <remarks>
        /// Invoked when the <see cref="NetworkSceneManager.OnLoadEventCompleted"/> event is triggered.
        /// </remarks>
        private void OnLoadEventCompletedAction(
            string sceneName,
            LoadSceneMode loadSceneMode,
            List<ulong> clientsCompleted,
            List<ulong> clientsTimedOut
        ) {
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds) {
                var playerTransform = Instantiate(playerPrefab);
                playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            }
        }


        // --- SERVER LOGIC ---

        /// <summary>
        /// Server RPC that marks a player as ready.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
            _playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

            var playerReadyList = NetworkManager.Singleton.ConnectedClientsIds.Select(
                playerId => _playerReadyDictionary.TryGetValue(playerId, out var isReady) && isReady
            );
            if (playerReadyList.All(isPlayerReady => isPlayerReady)) {
                _state.Value = State.Countdown;
            }
        }
    }
}
