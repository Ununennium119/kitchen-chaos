using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour {
    [SerializeField] private ClearCounter clearCounter;
    [SerializeField] private GameObject visualGameObject;

    private void Start() {
        Player.Instance.OnSelectedCounterChanged += OnSelectedCounterChangedAction;
    }


    private void OnSelectedCounterChangedAction(object sender, Player.OnSelectedCounterChangedArgs e) {
        if (e.SelectedCounter == clearCounter) {
            visualGameObject.SetActive(true);
        } else {
            visualGameObject.SetActive(false);
        }
    }
}
