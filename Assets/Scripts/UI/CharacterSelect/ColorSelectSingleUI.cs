using System;
using Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterSelect {
    [RequireComponent(typeof(Button), typeof(Image))]
    public class ColorSelectSingleUI : MonoBehaviour {
        [SerializeField, Tooltip("The index of the color")]
        private int colorIndex;

        [SerializeField, Tooltip("The selected game object")]
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

            _image.color = _multiplayerManager.GetPlayerColor(colorIndex);
            UpdateIsSelected();
        }

        private void OnDestroy() {
            _multiplayerManager.OnPlayerDataListChanged -= OnPlayerDataListChangedAction;
        }


        private void UpdateIsSelected() {
            var isSelected = _multiplayerManager.GetLocalPlayerData().ColorIndex == colorIndex;
            selectedGameObject.SetActive(isSelected);
        }


        private void OnPlayerDataListChangedAction(object sender, EventArgs e) {
            UpdateIsSelected();
        }
    }
}
