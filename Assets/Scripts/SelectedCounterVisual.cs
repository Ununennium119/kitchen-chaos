using Counter;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour {
    [SerializeField] private BaseCounter counter;
    [SerializeField] private GameObject[] visualGameObjects;

    private void Start() {
        Player.Instance.OnSelectedCounterChanged += OnSelectedCounterChangedAction;
    }


    private void OnSelectedCounterChangedAction(object sender, Player.OnSelectedCounterChangedArgs e) {
        var active = e.SelectedCounter == counter;
        foreach (var visualGameObject in visualGameObjects) {
            visualGameObject.SetActive(active);
        }
    }
}
