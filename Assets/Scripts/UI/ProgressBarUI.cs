using Counter;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class ProgressBarUI : MonoBehaviour {
        [SerializeField] private Image progressBarImage;
        [SerializeField] private CuttingCounter cuttingCounter;


        private void Start() {
            cuttingCounter.OnProgressChanged += OnProgressChangedAction;

            progressBarImage.fillAmount = 0f;
            gameObject.SetActive(false);
        }


        private void OnProgressChangedAction(object sender, CuttingCounter.OnProgressChangedArgs e) {
            progressBarImage.fillAmount = e.ProgressNormalized;
            gameObject.SetActive(true);

            if (e.ProgressNormalized is 0 or 1f) {
                gameObject.SetActive(false);
            }
        }
    }
}
