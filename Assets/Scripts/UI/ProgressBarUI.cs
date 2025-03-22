using System;
using Counter;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class ProgressBarUI : MonoBehaviour {
        [SerializeField] private Image progressBarImage;
        [SerializeField] private CuttingCounter cuttingCounter;


        private void Awake() {
            gameObject.SetActive(true);
        }

        private void Start() {
            cuttingCounter.OnProgressChanged += OnProgressChangedAction;

            progressBarImage.fillAmount = 0f;
            gameObject.SetActive(false);
        }


        private void OnProgressChangedAction(object sender, CuttingCounter.OnProgressChangedArgs e) {
            progressBarImage.fillAmount = e.ProgressNormalized;
            gameObject.SetActive(e.ProgressNormalized is not (0 or 1f));
        }
    }
}
