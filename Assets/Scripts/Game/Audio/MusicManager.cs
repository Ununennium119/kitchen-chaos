using Common.Utility;
using UnityEngine;
using Logger = Common.Utility.Logger;

namespace Game.Audio {
    /// <summary>
    /// Manages playing the music and modifying its volume.
    /// </summary>
    /// <remarks>This class is singleton.</remarks>
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour {
        public static MusicManager Instance { get; private set; }


        private AudioSource _audioSource;
        /// <summary>
        /// Adjusts the volume of music, configurable by the player in the options menu.
        /// </summary>
        private float _volume;


        /// <returns>Music volume</returns>
        public float GetVolume() {
            return _volume;
        }

        /// <summary>
        /// Increases volume of music by 0.1. If volume is 1, sets it to 0.
        /// </summary>
        public void ChangeVolume() {
            _volume += 0.1f;
            if (_volume >= 1.1f) {
                _volume = 0f;
            }
            _audioSource.volume = _volume;

            PlayerPrefsManager.SetMusicVolume(_volume);
        }


        private void Awake() {
            Logger.LogInitializingInstance(this);
            if (Instance != null) {
                Logger.LogMultipleInstancesError(this);
                Destroy(gameObject);
                return;
            }
            Instance = this;
            Logger.LogInstanceInitialized(this);

            _audioSource = gameObject.GetComponent<AudioSource>();

            UpdateVolume();
        }


        private void UpdateVolume() {
            _volume = PlayerPrefsManager.GetMusicVolume(defaultValue: .5f);
            _audioSource.volume = _volume;
        }
    }
}
