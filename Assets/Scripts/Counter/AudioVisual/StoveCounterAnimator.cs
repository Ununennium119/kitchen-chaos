using System.Linq;
using Counter.Logic;
using UnityEngine;

namespace Counter.AudioVisual {
    public class StoveCounterAnimator : MonoBehaviour {
        private readonly StoveCounter.State[] _onStates = { StoveCounter.State.Frying, StoveCounter.State.Fried };


        [SerializeField, Tooltip("The stove counter")]
        private StoveCounter stoveCounter;
        [SerializeField, Tooltip("Particle effects of the stove when it's turned on")]
        private GameObject stoveParticles;
        [SerializeField, Tooltip("Game object of the stove glove when it's turned on")]
        private GameObject stoveGlove;


        private void Start() {
            stoveCounter.OnStateChanged += OnStoveStateChangedAction;
        }


        private void OnStoveStateChangedAction(object sender, StoveCounter.OnStateChangedArgs e) {
            var isOn = _onStates.Contains(e.State);
            stoveParticles.SetActive(isOn);
            stoveGlove.SetActive(isOn);
        }
    }
}
