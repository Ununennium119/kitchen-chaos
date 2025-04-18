using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Manager {
    /// <summary>This class is responsible for managing game state.</summary>
    /// <remarks>This class is singleton.</remarks>
    public class GameManager : NetworkBehaviour {
        public enum State {
            WaitingToStart,
            Countdown,
            Playing,
            GameOver,
        }


        public static GameManager Instance { get; private set; }


        public event EventHandler<OnStateChangedArgs> OnStateChanged;
        public class OnStateChangedArgs : EventArgs {
            public State State;
        }

        public event EventHandler<OnLocalPauseToggledArgs> OnLocalPauseToggled;
        public class OnLocalPauseToggledArgs : EventArgs {
            public bool IsGamePaused;
        }

        public event EventHandler<OnPauseToggledArgs> OnPauseToggled;
        public class OnPauseToggledArgs : EventArgs {
            public bool IsGamePaused;
        }

        public event EventHandler<OnLocalPlayerReadyChangedArgs> OnLocalPlayerReadyChanged;
        public class OnLocalPlayerReadyChangedArgs : EventArgs {
            public bool IsLocalPlayerReady;
        }


        [SerializeField, Tooltip("Duration of countdown")]
        private float countdownDuration = 3f;
        [SerializeField, Tooltip("Duration of game in \"Playing\" state")]
        private float playDuration = 60f;


        private InputManager _inputManager;
        private readonly Dictionary<ulong, bool> _gamePausedDictionary = new();
        private readonly Dictionary<ulong, bool> _playerReadyDictionary = new();
        private readonly NetworkVariable<bool> _isGamePaused = new();
        private readonly NetworkVariable<State> _state = new();
        private readonly NetworkVariable<float> _currentCountdownTime = new();
        private readonly NetworkVariable<float> _currentPlayTime = new();
        private bool _isLocalGamePaused;
        private bool _isLocalPlayerReady;


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
            SetGamePausedServerRpc(_isLocalGamePaused);
            OnLocalPauseToggled?.Invoke(this, new OnLocalPauseToggledArgs { IsGamePaused = _isLocalGamePaused });
        }


        private void Awake() {
            Debug.Log("Setting up GameManager...");
            if (Instance != null) {
                Debug.LogError("There is more than one GameManager in the scene!");
            }
            Instance = this;

            _isLocalGamePaused = false;
        }

        private void Start() {
            _inputManager = InputManager.Instance;

            _inputManager.OnPausePerformed += OnPausePerformedPerformedAction;
            _inputManager.OnInteractPerformed += OnInteractPerformedPerformedAction;
        }

        public override void OnNetworkSpawn() {
            _state.OnValueChanged += OnStateValueChangedAction;
            _isGamePaused.OnValueChanged += OnIsGamePausedChangedAction;

            if (IsServer) {
                _currentCountdownTime.Value = countdownDuration;
                _currentPlayTime.Value = playDuration;
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


        private void OnPausePerformedPerformedAction(object sender, EventArgs e) {
            ToggleGamePause();
        }

        private void OnInteractPerformedPerformedAction(object sender, EventArgs e) {
            if (_state.Value == State.WaitingToStart && !_isLocalGamePaused) {
                _isLocalPlayerReady = true;
                OnLocalPlayerReadyChanged?.Invoke(
                    this,
                    new OnLocalPlayerReadyChangedArgs { IsLocalPlayerReady = _isLocalPlayerReady }
                );
                SetPlayerReadyServerRpc();
            }
        }

        private void OnStateValueChangedAction(State previousValue, State newValue) {
            OnStateChanged?.Invoke(this, new OnStateChangedArgs { State = newValue });
        }

        private void OnIsGamePausedChangedAction(bool previousValue, bool newValue) {
            if (newValue) {
                Time.timeScale = 0f;
            } else {
                Time.timeScale = 1f;
            }
            OnPauseToggled?.Invoke(this, new OnPauseToggledArgs { IsGamePaused = newValue });
        }


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

        [ServerRpc(RequireOwnership = false)]
        private void SetGamePausedServerRpc(bool isPaused, ServerRpcParams serverRpcParams = default) {
            _gamePausedDictionary[serverRpcParams.Receive.SenderClientId] = isPaused;

            var gamePausedList = NetworkManager.Singleton.ConnectedClientsIds.Select(
                playerId => _gamePausedDictionary.TryGetValue(playerId, out var isGamePaused) && isGamePaused
            );
            _isGamePaused.Value = gamePausedList.Any(isGamePaused => isGamePaused);
        }
    }
}
