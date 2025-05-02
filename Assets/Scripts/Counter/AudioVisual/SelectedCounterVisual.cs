using System;
using Counter.Logic;
using Player;
using UnityEngine;

namespace Counter.AudioVisual {
    internal class SelectedCounterVisual : MonoBehaviour {
        [SerializeField, Tooltip("The counter")]
        private BaseCounter counter;
        [SerializeField, Tooltip("Game objects which should be shown when counter is selected")]
        private GameObject[] visualGameObjects;


        private void Awake() {
            PlayerController.OnLocalPlayerNetworkSpawned += OnLocalPlayerNetworkSpawnedAction;
        }


        private void OnLocalPlayerNetworkSpawnedAction(object sender, EventArgs e) {
            PlayerController.LocalInstance.OnSelectedCounterChanged -= OnSelectedCounterChangedAction;
            PlayerController.LocalInstance.OnSelectedCounterChanged += OnSelectedCounterChangedAction;
        }

        private void OnSelectedCounterChangedAction(object sender, PlayerController.OnSelectedCounterChangedArgs e) {
            var active = e.SelectedCounter == counter;
            foreach (var visualGameObject in visualGameObjects) {
                visualGameObject.SetActive(active);
            }
        }
    }
}
