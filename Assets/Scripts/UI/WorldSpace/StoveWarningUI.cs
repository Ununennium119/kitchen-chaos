using Counter.Logic;
using UI.WorldSpace.Progress;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WorldSpace {
    public class StoveWarningUI : MonoBehaviour {
        [SerializeField, Tooltip("The threshold after which warning is shown")]
        private float warningThreshold;
        [SerializeField, Tooltip("The stove counter")]
        private StoveCounter stoveCounter;
        [SerializeField, Tooltip("The warning image")]
        private Image warningImage;


        private StoveCounter.State _currentStoveState;


        private void Start() {
            stoveCounter.OnStateChanged += OnStoveStateChangedAction;
            stoveCounter.OnProgressChanged += OnProgressChangedAction;

            gameObject.SetActive(false);
        }


        private void OnStoveStateChangedAction(object sender, StoveCounter.OnStateChangedArgs e) {
            _currentStoveState = e.State;
        }

        private void OnProgressChangedAction(object sender, IHasProgress.OnProgressChangedArgs e) {
            var isActive = _currentStoveState == StoveCounter.State.Fried &&
                           // This condition is added because there were some cases were state changes before progress
                           // and warning is shown
                           e.ProgressNormalized >= warningThreshold && e.ProgressNormalized < 0.99f;
            gameObject.SetActive(isActive);
        }
    }
}
