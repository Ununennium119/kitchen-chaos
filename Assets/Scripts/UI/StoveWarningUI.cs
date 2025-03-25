using Counter;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class StoveWarningUI : MonoBehaviour {
        [SerializeField] private float warningThreshold;
        [SerializeField] private StoveCounter stoveCounter;
        [SerializeField] private Image warningImage;

        private StoveCounter.State _currentState;


        private void Start() {
            stoveCounter.OnStateChanged += OnStateChangedAction;
            stoveCounter.OnProgressChanged += OnProgressChangedAction;

            gameObject.SetActive(false);
        }


        private void OnStateChangedAction(object sender, StoveCounter.OnStateChangedArgs e) {
            _currentState = e.State;
        }

        private void OnProgressChangedAction(object sender, IHasProgress.OnProgressChangedArgs e) {
            // This condition is added because there were some cases were state changes before progress
            // and warning is shown: e.ProgressNormalized < 0.99f
            var isActive = _currentState == StoveCounter.State.Fried &&
                           e.ProgressNormalized >= warningThreshold && e.ProgressNormalized < 0.99f;
            gameObject.SetActive(isActive);
        }
    }
}
