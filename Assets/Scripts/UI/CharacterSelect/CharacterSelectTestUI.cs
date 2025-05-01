using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterSelect {
    public class CharacterSelectTestUI : MonoBehaviour {
        [SerializeField, Tooltip("The ready button")]
        private Button readyButton;


        private CharacterSelectReadyManager _characterSelectReadyManager;


        private void Start() {
            _characterSelectReadyManager = CharacterSelectReadyManager.Instance;

            readyButton.onClick.AddListener(() => { _characterSelectReadyManager.SetPlayerReady(); });
        }
    }
}
