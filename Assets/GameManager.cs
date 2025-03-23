using System;
using UnityEngine;

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


    [SerializeField] private float maxWaitTime = 1f;
    [SerializeField] private float maxCountdownTime = 3f;
    [SerializeField] private float maxPlayTime = 10f;


    private GameInput _gameInput;
    private State _state;
    private float _currentWaitTime;
    private float _currentCountdownTime;
    private float _currentPlayTime;
    private bool _isGamePaused;


    public float GetCountdownTimer() {
        return _currentCountdownTime;
    }

    public bool IsPlaying() {
        return _state == State.Playing;
    }

    public float GetRemainingGameTimeNormalized() {
        return _currentPlayTime / maxPlayTime;
    }

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
        if (Instance != null) {
            Debug.LogError("There is more than one GameManager in the scene!");
        }
        Instance = this;

        ChangeState(State.WaitingToStart);
        _currentWaitTime = maxWaitTime;
        _currentCountdownTime = maxCountdownTime;
        _currentPlayTime = maxPlayTime;
        _isGamePaused = false;
    }

    private void Start() {
        _gameInput = GameInput.Instance;

        _gameInput.OnPause += OnPausePerformedAction;
    }

    private void Update() {
        switch (_state) {
            case State.WaitingToStart:
                _currentWaitTime -= Time.deltaTime;
                if (_currentWaitTime <= 0) {
                    ChangeState(State.Countdown);
                    _currentCountdownTime = maxCountdownTime;
                }
                break;
            case State.Countdown:
                _currentCountdownTime -= Time.deltaTime;
                if (_currentCountdownTime <= 0) {
                    ChangeState(State.Playing);
                    _currentPlayTime = maxPlayTime;
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

    private void OnPausePerformedAction(object sender, EventArgs e) {
        ToggleGamePause();
    }
}
