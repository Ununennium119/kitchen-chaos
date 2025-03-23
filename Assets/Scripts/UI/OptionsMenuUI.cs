using Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class OptionsMenuUI : MonoBehaviour {
        public static OptionsMenuUI Instance { get; private set; }


        [SerializeField] private Button musicVolumeButton;
        [SerializeField] private Button soundEffectsVolumeButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI musicVolumeText;
        [SerializeField] private TextMeshProUGUI soundEffectsVolumeText;


        private GameManager _gameManager;
        private MusicManager _musicManager;
        private AudioManager _audioManager;


        public void Show() {
            gameObject.SetActive(true);
        }


        private void Awake() {
            if (Instance != null) {
                Debug.LogError("There is more than one OptionsMenuUI in the scene!");
            }
            Instance = this;

            musicVolumeButton.onClick.AddListener(() => { 
                _musicManager.ChangeVolume();
                UpdateMusicVolume();
                
            });
            soundEffectsVolumeButton.onClick.AddListener(() => {
                _audioManager.ChangeVolume();
                UpdateSoundEffectsVolume();
            });
            closeButton.onClick.AddListener(Hide);
        }

        private void Start() {
            _gameManager = GameManager.Instance;
            _musicManager = MusicManager.Instance;
            _audioManager = AudioManager.Instance;

            _gameManager.OnPauseToggled += OnPauseToggledAction;

            UpdateMusicVolume();
            UpdateSoundEffectsVolume();
            Hide();
        }


        private void Hide() {
            gameObject.SetActive(false);
        }

        private void UpdateMusicVolume() {
            var volume = _musicManager.GetVolume();
            var volumeString = Mathf.RoundToInt(volume * 10).ToString();
            musicVolumeText.text = $"Music: {volumeString}";
        }

        private void UpdateSoundEffectsVolume() {
            var volume = _audioManager.GetVolume();
            var volumeString = Mathf.RoundToInt(volume * 10).ToString();
            soundEffectsVolumeText.text = $"Sound Effects: {volumeString}";
        }


        private void OnPauseToggledAction(object sender, GameManager.OnPauseToggledArgs e) {
            if (e.IsGamePaused == false) {
                Hide();
            }
        }
    }
}
