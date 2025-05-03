using System;
using Game.Counter.Logic;
using Game.Player;
using UnityEngine;

namespace Game.Counter.AudioVisual {
    /// <summary>
    /// Controls visual indicators for a counter when it is selected by the local player.
    /// </summary>
    internal class SelectedCounterVisual : MonoBehaviour {
        [SerializeField, Tooltip("The counter")]
        private BaseCounter counter;
        [SerializeField, Tooltip("Game objects which should be shown when counter is selected")]
        private GameObject[] visualGameObjects;


        private void Awake() {
            PlayerController.OnLocalPlayerNetworkSpawned += OnLocalPlayerNetworkSpawnedAction;
        }


        /// <remarks>
        /// Invoked when the <see cref="PlayerController.OnLocalPlayerNetworkSpawned"/> event is triggered.
        /// </remarks>
        private void OnLocalPlayerNetworkSpawnedAction(object sender, EventArgs e) {
            PlayerController.LocalInstance.OnSelectedCounterChanged -= OnSelectedCounterChangedAction;
            PlayerController.LocalInstance.OnSelectedCounterChanged += OnSelectedCounterChangedAction;
        }

        /// <remarks>
        /// Invoked when the <see cref="PlayerController.OnSelectedCounterChanged"/> event is triggered.
        /// </remarks>
        private void OnSelectedCounterChangedAction(object sender, PlayerController.OnSelectedCounterChangedArgs e) {
            var active = e.SelectedCounter == counter;
            foreach (var visualGameObject in visualGameObjects) {
                visualGameObject.SetActive(active);
            }
        }
    }
}
