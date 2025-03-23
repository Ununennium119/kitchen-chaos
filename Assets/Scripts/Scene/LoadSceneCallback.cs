using UnityEngine;

public class LoadSceneCallback : MonoBehaviour {
    private bool _isFirstUpdate = true;


    private void Update() {
        if (!_isFirstUpdate) return;

        _isFirstUpdate = false;
        SceneLoader.LoadSceneCallback();
    }
}