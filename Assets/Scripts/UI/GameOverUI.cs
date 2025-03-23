using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class GameOverUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI recipesDeliveredCountText;
        [SerializeField] private Button mainMenuButton;


        private GameManager _gameManager;
        private DeliveryManager _deliveryManager;


        private void Awake() {
            mainMenuButton.onClick.AddListener(() => SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene));
        }

        private void Start() {
            _gameManager = GameManager.Instance;
            _deliveryManager = DeliveryManager.Instance;

            _gameManager.OnStateChanged += OnStateChangedAction;
            gameObject.SetActive(false);
        }

        private void Update() {
            recipesDeliveredCountText.text = _deliveryManager.GetDeliveredRecipesCount().ToString();
        }


        private void OnStateChangedAction(object sender, GameManager.OnStateChangedArgs e) {
            var isActive = e.State == GameManager.State.GameOver;
            gameObject.SetActive(isActive);
            mainMenuButton.Select();
        }
    }
}
