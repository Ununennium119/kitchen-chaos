using Counter;
using UnityEngine;

internal class SelectedCounterVisual : MonoBehaviour {
    [SerializeField] private BaseCounter counter;
    [SerializeField] private GameObject[] visualGameObjects;

    private void Start() {
        Player.Player.Instance.OnSelectedCounterChanged += OnSelectedCounterChangedAction;
    }


    private void OnSelectedCounterChangedAction(object sender, Player.Player.OnSelectedCounterChangedArgs e) {
        var active = e.SelectedCounter == counter;
        foreach (var visualGameObject in visualGameObjects) {
            visualGameObject.SetActive(active);
        }
    }
}
