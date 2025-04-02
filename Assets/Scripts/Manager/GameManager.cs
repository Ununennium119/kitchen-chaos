using System;
using UnityEngine;

namespace Manager {
    /// <summary>This class is responsible for managing game state.</summary>
    /// <remarks>This class is singleton.</remarks>
    public class GameManager : MonoBehaviour {
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

        public event EventHandler<OnPauseToggledArgs> OnPauseToggled;
        public class OnPauseToggledArgs : EventArgs {
            public bool IsGamePaused;
        }


        [SerializeField, Tooltip("Duration of countdown")]
        private float countdownDuration = 3f;
        [SerializeField, Tooltip("Duration of game in \"Playing\" state")]
        private float playDuration = 60f;


        private InputManager _inputManager;
        private State _state;
        private float _currentCountdownTime;
        private float _currentPlayTime;
        private bool _isGamePaused;


        /// <returns>Current countdown time</returns>
        public float GetCountdownTime() {
            return _currentCountdownTime;
        }

        /// <returns>true if game is in <see cref="State.Playing"/> state.</returns>
        public bool IsPlaying() {
            return _state == State.Playing;
        }

        /// <returns>Normalized (between 0 and 1) game time.</returns>
        public float GetRemainingGameTimeNormalized() {
            return _currentPlayTime / playDuration;
        }

        /// <summary>
        /// Toggles game pause if game is not in <see cref="State.GameOver"/> status.
        /// </summary>
        public void ToggleGamePause() {
            if (_state == State.GameOver) return;

            _isGamePaused = !_isGamePaused;
            if (_isGamePaused) {
                Time.timeScale = 0f;
            } else {
                Time.timeScale = 1f;
            }
            OnPauseToggled?.Invoke(this, new OnPauseToggledArgs { IsGamePaused = _isGamePaused });
        }


        private void Awake() {
            Debug.Log("Setting up GameManager...");
            if (Instance != null) {
                Debug.LogError("There is more than one GameManager in the scene!");
            }
            Instance = this;

            _state = State.WaitingToStart;
            _currentCountdownTime = countdownDuration;
            _currentPlayTime = playDuration;
            _isGamePaused = false;
        }

        private void Start() {
            _inputManager = InputManager.Instance;

            _inputManager.OnPausePerformed += OnPausePerformedPerformedAction;
            _inputManager.OnInteractPerformed += OnInteractPerformedPerformedAction;

            // TODO: Remove (DEBUG)
            ChangeState(State.Playing);
        }

        private void Update() {
            switch (_state) {
                case State.WaitingToStart:
                    // Do Nothing
                    break;
                case State.Countdown:
                    _currentCountdownTime -= Time.deltaTime;
                    if (_currentCountdownTime <= 0) {
                        ChangeState(State.Playing);
                        _currentPlayTime = playDuration;
                    }
                    break;
                case State.Playing:
                    _currentPlayTime -= Time.deltaTime;
                    if (_currentPlayTime <= 0) {
                        ChangeState(State.GameOver);
                    }
                    break;
                case State.GameOver:
                    Time.timeScale = 0f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private void ChangeState(State state) {
            _state = state;
            OnStateChanged?.Invoke(this, new OnStateChangedArgs { State = state });
        }

        private void OnPausePerformedPerformedAction(object sender, EventArgs e) {
            ToggleGamePause();
        }

        private void OnInteractPerformedPerformedAction(object sender, EventArgs e) {
            if (_state == State.WaitingToStart && !_isGamePaused) {
                ChangeState(State.Countdown);
            }
        }
    }
}
