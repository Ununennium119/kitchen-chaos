using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class GameOverUI : MonoBehaviour {
        [SerializeField, Tooltip("The text to show number of delivered orders")]
        private TextMeshProUGUI deliveredOrdersCountText;
        [SerializeField, Tooltip("The main menu button")]
        private Button mainMenuButton;


        private GameManager _gameManager;
        private DeliveryManager _deliveryManager;


        private void Awake() {
            mainMenuButton.onClick.AddListener(() => SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene));
        }

        private void Start() {
            _gameManager = GameManager.Instance;
            _deliveryManager = DeliveryManager.Instance;

            _gameManager.OnStateChanged += OnGameStateChangedAction;
            gameObject.SetActive(false);
        }

        private void Update() {
            deliveredOrdersCountText.text = _deliveryManager.GetDeliveredOrdersCount().ToString();
        }


        private void OnGameStateChangedAction(object sender, GameManager.OnStateChangedArgs e) {
            var isActive = e.State == GameManager.State.GameOver;
            gameObject.SetActive(isActive);
            mainMenuButton.Select();
        }
    }
}
