using System.Linq;
using UnityEngine;

namespace Counter {
    public class StoveCounterAudioSource : MonoBehaviour {
        [SerializeField] private StoveCounter stoveCounter;


        private readonly StoveCounter.State[] _playingStates = { StoveCounter.State.Frying, StoveCounter.State.Fried };
        private AudioSource _audioSource;


        private void Awake() {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start() {
            stoveCounter.OnStateChanged += OnStateChangedAction;
        }


        private void OnStateChangedAction(object sender, StoveCounter.OnStateChangedArgs e) {
            var playSound = _playingStates.Contains(e.State);
            if (playSound) {
                _audioSource.Play();
            } else {
                _audioSource.Stop();
            }
        }
    }
}
