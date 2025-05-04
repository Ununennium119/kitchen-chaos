using System.Linq;
using Game.Counter.Logic;
using UnityEngine;

namespace Game.Counter.AudioVisual {
    /// <summary>
    /// Controls the visual effects of the <see cref="stoveCounter"/> based on its state.
    /// </summary>
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


        /// <remarks>
        /// Invoked when the <see cref="StoveCounter.OnStateChanged"/> event is triggered.
        /// </remarks>
        private void OnStoveStateChangedAction(object sender, StoveCounter.OnStateChangedArgs e) {
            var isOn = _onStates.Contains(e.State);
            stoveParticles.SetActive(isOn);
            stoveGlove.SetActive(isOn);
        }
    }
}
