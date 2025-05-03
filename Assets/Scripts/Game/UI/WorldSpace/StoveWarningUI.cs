using Game.Counter.Logic;
using Game.UI.WorldSpace.Progress;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.WorldSpace {
    /// <summary>
    /// This class manages the UI that shows a warning when the stove's progress exceeds a certain threshold.
    /// </summary>
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


        /// <remarks>
        /// Invoked when the <see cref="StoveCounter.OnStateChanged"/> event is triggered.
        /// </remarks>
        private void OnStoveStateChangedAction(object sender, StoveCounter.OnStateChangedArgs e) {
            _currentStoveState = e.State;
        }

        /// <summary>
        /// Shows or hides the warning image based on stove's progress.
        /// </summary>
        /// <remarks>
        /// Invoked when the <see cref="StoveCounter.OnStateChanged"/> event is triggered.
        /// </remarks>
        private void OnProgressChangedAction(object sender, IHasProgress.OnProgressChangedArgs e) {
            var isActive = _currentStoveState == StoveCounter.State.Fried &&
                           // This condition is added because there were some cases were state changes before progress
                           // and warning is shown
                           e.ProgressNormalized >= warningThreshold && e.ProgressNormalized < 0.99f;
            gameObject.SetActive(isActive);
        }
    }
}
