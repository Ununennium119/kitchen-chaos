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


    [SerializeField] private float maxWaitTime = 1f;
    [SerializeField] private float maxCountdownTime = 3f;
    [SerializeField] private float maxPlayTime = 10f;


    private State _state;
    private float _currentWaitTime;
    private float _currentCountdownTime;
    private float _currentPlayTime;


    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There is more than one GameManager in the scene!");
        }
        Instance = this;

        ChangeState(State.WaitingToStart);
        _currentWaitTime = maxWaitTime;
        _currentCountdownTime = maxCountdownTime;
        _currentPlayTime = maxPlayTime;
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
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private void ChangeState(State state) {
        _state = state;
        OnStateChanged?.Invoke(this, new OnStateChangedArgs { State = state });
    }


    public float GetCountdownTimer() {
        return _currentCountdownTime;
    }

    public bool IsPlaying() {
        return _state == State.Playing;
    }

    public float GetRemainingGameTimeNormalized() {
        return _currentPlayTime / maxPlayTime;
    }
}
