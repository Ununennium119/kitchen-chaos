using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Game.UI.WorldSpace.Progress {
    /// <summary>
    /// This class manages the UI representation of a progress bar.
    /// </summary>
    /// <seealso cref="IHasProgress"/>
    public class ProgressBarUI : MonoBehaviour {
        [SerializeField, Tooltip("The image of the progress bar used to show progress")]
        private Image progressBarImage;
        [SerializeField, Tooltip("The game object which has progress (It should implement IHasProgress)")]
        private GameObject progressOwnerGameObject;


        private void Awake() {
            gameObject.SetActive(true);
        }

        private void Start() {
            var progressOwner = progressOwnerGameObject.GetComponent<IHasProgress>();
            if (progressOwner == null) {
                Debug.LogError($"Progress owner {progressOwnerGameObject.name} should implement IHasProgress");
            } else {
                progressOwner.OnProgressChanged += OnProgressChangedAction;
            }

            progressBarImage.fillAmount = 0f;
            gameObject.SetActive(false);
        }


        /// <summary>
        /// Updates the progress bar's fill amount and shows or hides the progress bar based on the progress value.
        /// </summary>
        /// <remarks>
        /// Invoked when the <see cref="IHasProgress.OnProgressChanged"/> event is triggered.
        /// </remarks>
        private void OnProgressChangedAction(object sender, IHasProgress.OnProgressChangedArgs e) {
            progressBarImage.fillAmount = e.ProgressNormalized;
            gameObject.SetActive(e.ProgressNormalized is not (0 or 1f));
        }
    }
}
