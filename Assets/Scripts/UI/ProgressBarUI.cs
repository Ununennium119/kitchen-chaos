using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace UI {
    public class ProgressBarUI : MonoBehaviour {
        [SerializeField] private Image progressBarImage;
        [SerializeField] private GameObject progressOwnerGameObject;


        private void Awake() {
            gameObject.SetActive(true);
        }

        private void Start() {
            var progressOwner = progressOwnerGameObject.GetComponent<IHasProgress>();
            progressOwner.OnProgressChanged += OnProgressChangedAction;

            progressBarImage.fillAmount = 0f;
            gameObject.SetActive(false);
        }


        private void OnProgressChangedAction(object sender, IHasProgress.OnProgressChangedArgs e) {
            progressBarImage.fillAmount = e.ProgressNormalized;
            gameObject.SetActive(e.ProgressNormalized is not (0 or 1f));
        }
    }
}
