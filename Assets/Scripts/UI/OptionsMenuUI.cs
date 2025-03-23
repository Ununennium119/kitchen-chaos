using Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class OptionsMenuUI : MonoBehaviour {
        public static OptionsMenuUI Instance { get; private set; }


        [SerializeField] private Button musicVolumeButton;
        [SerializeField] private TextMeshProUGUI musicVolumeText;
        [SerializeField] private Button soundEffectsVolumeButton;
        [SerializeField] private TextMeshProUGUI soundEffectsVolumeText;
        [SerializeField] private Button moveUpButton;
        [SerializeField] private TextMeshProUGUI moveUpText;
        [SerializeField] private Button moveDownButton;
        [SerializeField] private TextMeshProUGUI moveDownText;
        [SerializeField] private Button moveRightButton;
        [SerializeField] private TextMeshProUGUI moveRightText;
        [SerializeField] private Button moveLeftButton;
        [SerializeField] private TextMeshProUGUI moveLeftText;
        [SerializeField] private Button interactButton;
        [SerializeField] private TextMeshProUGUI interactText;
        [SerializeField] private Button alternativeInteractButton;
        [SerializeField] private TextMeshProUGUI alternativeInteractText;
        [SerializeField] private Button pauseInteractButton;
        [SerializeField] private TextMeshProUGUI pauseInteractText;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject pressToRebindMenuUI;


        private GameManager _gameManager;
        private MusicManager _musicManager;
        private AudioManager _audioManager;
        private GameInput _gameInput;


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
            moveUpButton.onClick.AddListener(() => { RebindPlayerBinding(GameInput.Binding.MoveUp); });
            moveDownButton.onClick.AddListener(() => { RebindPlayerBinding(GameInput.Binding.MoveDown); });
            moveRightButton.onClick.AddListener(() => { RebindPlayerBinding(GameInput.Binding.MoveRight); });
            moveLeftButton.onClick.AddListener(() => { RebindPlayerBinding(GameInput.Binding.MoveLeft); });
            interactButton.onClick.AddListener(() => { RebindPlayerBinding(GameInput.Binding.Interact); });
            alternativeInteractButton.onClick.AddListener(() => {
                RebindPlayerBinding(GameInput.Binding.AlternativeInteract);
            });
            pauseInteractButton.onClick.AddListener(() => { RebindPlayerBinding(GameInput.Binding.Pause); });
            closeButton.onClick.AddListener(Hide);
        }

        private void Start() {
            _gameManager = GameManager.Instance;
            _musicManager = MusicManager.Instance;
            _audioManager = AudioManager.Instance;
            _gameInput = GameInput.Instance;

            _gameManager.OnPauseToggled += OnPauseToggledAction;

            UpdateMusicVolume();
            UpdateSoundEffectsVolume();
            UpdateBindingsVisual();
            Hide();
            pressToRebindMenuUI.SetActive(false);
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

        private void UpdateBindingsVisual() {
            moveUpText.text = _gameInput.GetPlayerBindingDisplayString(GameInput.Binding.MoveUp);
            moveDownText.text = _gameInput.GetPlayerBindingDisplayString(GameInput.Binding.MoveDown);
            moveRightText.text = _gameInput.GetPlayerBindingDisplayString(GameInput.Binding.MoveRight);
            moveLeftText.text = _gameInput.GetPlayerBindingDisplayString(GameInput.Binding.MoveLeft);
            interactText.text = _gameInput.GetPlayerBindingDisplayString(GameInput.Binding.Interact);
            alternativeInteractText.text =
                _gameInput.GetPlayerBindingDisplayString(GameInput.Binding.AlternativeInteract);
            pauseInteractText.text = _gameInput.GetPlayerBindingDisplayString(GameInput.Binding.Pause);
        }

        private void RebindPlayerBinding(GameInput.Binding binding) {
            TogglePressToRebindMenuUI();
            _gameInput.SetPlayerBinding(binding, () => {
                UpdateBindingsVisual();
                TogglePressToRebindMenuUI();
            });
        }

        private void TogglePressToRebindMenuUI() {
            pressToRebindMenuUI.SetActive(!pressToRebindMenuUI.activeSelf);
        }


        private void OnPauseToggledAction(object sender, GameManager.OnPauseToggledArgs e) {
            if (e.IsGamePaused) return;

            Hide();
            if (pressToRebindMenuUI.activeSelf) {
                TogglePressToRebindMenuUI();
            }
        }
    }
}
