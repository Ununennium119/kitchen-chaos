using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace UI.WorldSpace.Progress {
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


        private void OnProgressChangedAction(object sender, IHasProgress.OnProgressChangedArgs e) {
            progressBarImage.fillAmount = e.ProgressNormalized;
            gameObject.SetActive(e.ProgressNormalized is not (0 or 1f));
        }
    }
}
