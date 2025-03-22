using System.Linq;
using UnityEngine;

namespace Counter {
    public class StoveCounterAnimator : MonoBehaviour {
        private readonly StoveCounter.State[] _onStates = { StoveCounter.State.Frying, StoveCounter.State.Fried };


        [SerializeField] private StoveCounter stoveCounter;
        [SerializeField] private GameObject stoveParticles;
        [SerializeField] private GameObject stoveGlove;


        private void Start() {
            stoveCounter.OnStateChanged += OnStateChangedAction;
        }


        private void OnStateChangedAction(object sender, StoveCounter.OnStateChangedArgs args) {
            var isOn = _onStates.Contains(args.State);
            stoveParticles.SetActive(isOn);
            stoveGlove.SetActive(isOn);
        }
    }
}
