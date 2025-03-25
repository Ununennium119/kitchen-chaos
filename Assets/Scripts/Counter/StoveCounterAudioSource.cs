using System.Linq;
using Audio;
using UI;
using UnityEngine;

namespace Counter {
    public class StoveCounterAudioSource : MonoBehaviour {
        private readonly StoveCounter.State[] _playingStates = { StoveCounter.State.Frying, StoveCounter.State.Fried };


        [SerializeField] private float warningThreshold = 0.5f;
        [SerializeField] private float warningAudioMaxTime = 0.35f;
        [SerializeField] private StoveCounter stoveCounter;


        private AudioManager _audioManager;
        private AudioSource _audioSource;
        private StoveCounter.State _currentState;
        private bool _isWarningActive;
        private float _currentWarningAudioTime;


        private void Awake() {
            _audioSource = GetComponent<AudioSource>();

            _currentWarningAudioTime = warningAudioMaxTime;
        }

        private void Start() {
            _audioManager = AudioManager.Instance;

            stoveCounter.OnStateChanged += OnStateChangedAction;
            stoveCounter.OnProgressChanged += OnProgressChangedAction;
        }

        private void Update() {
            if (!_isWarningActive) return;

            _currentWarningAudioTime -= Time.deltaTime;
            if (_currentWarningAudioTime > 0) return;

            _audioManager.PlayWarningAudioClip(stoveCounter.transform.position);
            _currentWarningAudioTime = warningAudioMaxTime;
        }


        private void OnStateChangedAction(object sender, StoveCounter.OnStateChangedArgs e) {
            _currentState = e.State;
            var playSound = _playingStates.Contains(e.State);
            if (playSound) {
                _audioSource.Play();
            } else {
                _audioSource.Stop();
            }
        }

        private void OnProgressChangedAction(object sender, IHasProgress.OnProgressChangedArgs e) {
            // This condition is added because there were some cases were state changes before progress
            // and warning is shown: e.ProgressNormalized < 0.99f
            _isWarningActive = _currentState == StoveCounter.State.Fried &&
                               e.ProgressNormalized >= warningThreshold && e.ProgressNormalized < 0.99f;
        }
    }
}
