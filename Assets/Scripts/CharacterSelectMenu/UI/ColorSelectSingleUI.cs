using System;
using Common.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterSelectMenu.UI {
    /// <summary>
    /// Represents the UI for a single color selection in the character select menu.
    /// </summary>
    [RequireComponent(typeof(Button), typeof(Image))]
    public class ColorSelectSingleUI : MonoBehaviour {
        [SerializeField, Tooltip("The index of the color")]
        private int colorIndex;

        [SerializeField, Tooltip("The game object which is shown when the color is selected")]
        private GameObject selectedGameObject;


        private MultiplayerManager _multiplayerManager;
        private Button _button;
        private Image _image;


        private void Awake() {
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();

            _button.onClick.AddListener(() => { _multiplayerManager.ChangePlayerColor(colorIndex); });
        }

        private void Start() {
            _multiplayerManager = MultiplayerManager.Instance;

            _multiplayerManager.OnPlayerDataListChanged += OnPlayerDataListChangedAction;

            UpdateImageColor();
            UpdateIsSelected();
        }

        private void OnDestroy() {
            _multiplayerManager.OnPlayerDataListChanged -= OnPlayerDataListChangedAction;
        }


        /// <summary>
        /// Updates the color of the <see cref="_image"/> component based on the <see cref="colorIndex"/>.
        /// </summary>
        private void UpdateImageColor() {
            _image.color = _multiplayerManager.GetPlayerColor(colorIndex);
        }

        /// <summary>
        /// Updates the visibility of the <see cref="selectedGameObject"/>.
        /// </summary>
        private void UpdateIsSelected() {
            var isSelected = _multiplayerManager.GetLocalPlayerData().ColorIndex == colorIndex;
            selectedGameObject.SetActive(isSelected);
        }


        /// <remarks>
        /// Invoked when the <see cref="MultiplayerManager.OnPlayerDataListChanged"/> event is triggered.
        /// </remarks>
        private void OnPlayerDataListChangedAction(object sender, EventArgs e) {
            UpdateIsSelected();
        }
    }
}
