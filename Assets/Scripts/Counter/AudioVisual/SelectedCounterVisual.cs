using Counter.Logic;
using Player;
using UnityEngine;

namespace Counter.AudioVisual {
    internal class SelectedCounterVisual : MonoBehaviour {
        [SerializeField, Tooltip("The counter")]
        private BaseCounter counter;
        [SerializeField, Tooltip("Game objects which should be shown when counter is selected")]
        private GameObject[] visualGameObjects;


        private void Start() {
            PlayerController.Instance.OnSelectedCounterChanged += OnSelectedCounterChangedAction;
        }


        private void OnSelectedCounterChangedAction(object sender, PlayerController.OnSelectedCounterChangedArgs e) {
            var active = e.SelectedCounter == counter;
            foreach (var visualGameObject in visualGameObjects) {
                visualGameObject.SetActive(active);
            }
        }
    }
}
