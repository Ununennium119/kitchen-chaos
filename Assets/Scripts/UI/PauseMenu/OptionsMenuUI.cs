using System;
using Audio;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PauseMenu {
    /// <remarks>This class is singleton.</remarks>
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
        [SerializeField] private Button pauseButton;
        [SerializeField] private TextMeshProUGUI pauseText;
        [SerializeField] private Button gamepadInteractButton;
        [SerializeField] private TextMeshProUGUI gamepadInteractText;
        [SerializeField] private Button gamepadAlternativeInteractButton;
        [SerializeField] private TextMeshProUGUI gamepadAlternativeInteractText;
        [SerializeField] private Button gamepadPauseButton;
        [SerializeField] private TextMeshProUGUI gamepadPauseText;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject pressToRebindMenuUI;


        private GameManager _gameManager;
        private MusicManager _musicManager;
        private SoundEffectManager _soundEffectManager;
        private InputManager _inputManager;
        private Action _onCloseCallback;


        public void Show(Action onCloseCallback) {
            gameObject.SetActive(true);
            musicVolumeButton.Select();
            _onCloseCallback = onCloseCallback;
        }


        private void Awake() {
            Debug.Log("Setting up OptionsMenuUI...");
            if (Instance != null) {
                Debug.LogError("There is more than one OptionsMenuUI in the scene!");
            }
            Instance = this;

            musicVolumeButton.onClick.AddListener(() => {
                _musicManager.ChangeVolume();
                UpdateMusicVolume();
            });
            soundEffectsVolumeButton.onClick.AddListener(() => {
                _soundEffectManager.ChangeVolume();
                UpdateSoundEffectsVolume();
            });
            moveUpButton.onClick.AddListener(() => { RebindPlayerBinding(InputManager.Binding.MoveUp); });
            moveDownButton.onClick.AddListener(() => { RebindPlayerBinding(InputManager.Binding.MoveDown); });
            moveRightButton.onClick.AddListener(() => { RebindPlayerBinding(InputManager.Binding.MoveRight); });
            moveLeftButton.onClick.AddListener(() => { RebindPlayerBinding(InputManager.Binding.MoveLeft); });
            interactButton.onClick.AddListener(() => { RebindPlayerBinding(InputManager.Binding.Interact); });
            alternativeInteractButton.onClick.AddListener(() => {
                RebindPlayerBinding(InputManager.Binding.AlternativeInteract);
            });
            pauseButton.onClick.AddListener(() => { RebindPlayerBinding(InputManager.Binding.Pause); });
            gamepadInteractButton.onClick.AddListener(() => {
                RebindPlayerBinding(InputManager.Binding.GamepadInteract);
            });
            gamepadAlternativeInteractButton.onClick.AddListener(() => {
                RebindPlayerBinding(InputManager.Binding.GamepadAlternativeInteract);
            });
            gamepadPauseButton.onClick.AddListener(() => { RebindPlayerBinding(InputManager.Binding.GamepadPause); });
            closeButton.onClick.AddListener(() => {
                Hide();
                _onCloseCallback?.Invoke();
            });
        }

        private void Start() {
            _gameManager = GameManager.Instance;
            _musicManager = MusicManager.Instance;
            _soundEffectManager = SoundEffectManager.Instance;
            _inputManager = InputManager.Instance;

            _gameManager.OnPauseToggled += OnPauseToggledAction;

            UpdateMusicVolume();
            UpdateSoundEffectsVolume();
            UpdateBindingsVisual();
            pressToRebindMenuUI.SetActive(false);
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
            var volume = _soundEffectManager.GetVolume();
            var volumeString = Mathf.RoundToInt(volume * 10).ToString();
            soundEffectsVolumeText.text = $"Sound Effects: {volumeString}";
        }

        private void UpdateBindingsVisual() {
            moveUpText.text = _inputManager.GetPlayerBindingDisplayString(InputManager.Binding.MoveUp);
            moveDownText.text = _inputManager.GetPlayerBindingDisplayString(InputManager.Binding.MoveDown);
            moveRightText.text = _inputManager.GetPlayerBindingDisplayString(InputManager.Binding.MoveRight);
            moveLeftText.text = _inputManager.GetPlayerBindingDisplayString(InputManager.Binding.MoveLeft);
            interactText.text = _inputManager.GetPlayerBindingDisplayString(InputManager.Binding.Interact);
            alternativeInteractText.text =
                _inputManager.GetPlayerBindingDisplayString(InputManager.Binding.AlternativeInteract);
            pauseText.text = _inputManager.GetPlayerBindingDisplayString(InputManager.Binding.Pause);
            gamepadInteractText.text = _inputManager.GetPlayerBindingDisplayString(InputManager.Binding.GamepadInteract);
            gamepadAlternativeInteractText.text =
                _inputManager.GetPlayerBindingDisplayString(InputManager.Binding.GamepadAlternativeInteract);
            gamepadPauseText.text = _inputManager.GetPlayerBindingDisplayString(InputManager.Binding.GamepadPause);
        }

        private void RebindPlayerBinding(InputManager.Binding binding) {
            TogglePressToRebindMenuUI();
            _inputManager.SetPlayerBinding(binding, () => {
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
