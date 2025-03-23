using UnityEngine;

namespace Audio {
    public class MusicManager : MonoBehaviour {
        private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";


        public static MusicManager Instance { get; private set; }


        private AudioSource _audioSource;
        private float _volume;


        public float GetVolume() {
            return _volume;
        }

        public void ChangeVolume() {
            _volume += 0.1f;
            if (_volume >= 1.1f) {
                _volume = 0f;
            }
            _audioSource.volume = _volume;

            PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, _volume);
            PlayerPrefs.Save();
        }


        private void Awake() {
            if (Instance != null) {
                Debug.LogError("There is more than one MusicManager in the scene!");
            }
            Instance = this;

            _audioSource = gameObject.GetComponent<AudioSource>();
            _volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, 0.5f);
            _audioSource.volume = _volume;
        }
    }
}
