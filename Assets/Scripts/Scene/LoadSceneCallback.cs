using UnityEngine;

/// <summary>
/// Calls back <see cref="SceneLoader"/> to load the target scene after LoadingScene is loaded.
/// </summary>
/// <remarks>This class should only be assigned to a single game object in LoadingScene.</remarks>
public class LoadSceneCallback : MonoBehaviour {
    private bool _isFirstUpdate = true;


    private void Update() {
        if (!_isFirstUpdate) return;

        _isFirstUpdate = false;
        SceneLoader.LoadSceneCallback();
    }
}
