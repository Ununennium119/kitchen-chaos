using System.Linq;
using Audio;
using Counter.Logic;
using UI.WorldSpace.Progress;
using UnityEngine;

namespace Counter.AudioVisual {
    [RequireComponent(typeof(AudioSource))]
    public class StoveCounterSounds : MonoBehaviour {
        [SerializeField, Tooltip("The stove counter")]
        private StoveCounter stoveCounter;
        [SerializeField, Tooltip("The threshold after which stove's warning is activated")]
        private float warningThreshold = 0.5f;
        [SerializeField, Tooltip("The duration between stove's warning sound effect")]
        private float warningCooldown = 0.35f;


        private SoundEffectManager _soundEffectManager;
        private AudioSource _audioSource;
        private StoveCounter.State _currentState;
        private bool _isWarningActive;
        private float _warningCooldownRemaining;


        private void Awake() {
            _audioSource = GetComponent<AudioSource>();

            _warningCooldownRemaining = warningCooldown;
        }

        private void Start() {
            _soundEffectManager = SoundEffectManager.Instance;

            stoveCounter.OnStateChanged += OnStoveStateChangedAction;
            stoveCounter.OnProgressChanged += OnStoveProgressChangedAction;
        }

        private void Update() {
            if (!_isWarningActive) return;

            _warningCooldownRemaining -= Time.deltaTime;
            if (_warningCooldownRemaining > 0) return;

            _soundEffectManager.PlayWarningAudioClip(stoveCounter.transform.position);
            _warningCooldownRemaining = warningCooldown;
        }


        private void OnStoveStateChangedAction(object sender, StoveCounter.OnStateChangedArgs e) {
            _currentState = e.State;
            var playSound = new[] { StoveCounter.State.Frying, StoveCounter.State.Fried }.Contains(e.State);
            if (playSound) {
                _audioSource.Play();
            } else {
                _audioSource.Stop();
            }
        }

        private void OnStoveProgressChangedAction(object sender, IHasProgress.OnProgressChangedArgs e) {
            // This condition is added because there were some cases were state changes before progress
            // and warning is shown: e.ProgressNormalized < 0.99f
            _isWarningActive = _currentState == StoveCounter.State.Fried &&
                               e.ProgressNormalized >= warningThreshold && e.ProgressNormalized < 0.99f;
        }
    }
}
