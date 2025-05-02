using Unity.Netcode;
using UnityEngine.SceneManagement;

/// <summary>
/// This class is responsible for loading scenes.
/// </summary>
public static class SceneLoader {
    public enum Scene {
        MainMenuScene,
        LoadingScene,
        GameScene,
        LobbyScene,
        CharacterSelectScene,
    }


    private static Scene _targetScene;


    public static void LoadScene(Scene scene) {
        _targetScene = scene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoadNetwork(Scene scene) {
        NetworkManager.Singleton.SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
    }

    public static void LoadSceneCallback() {
        SceneManager.LoadScene(_targetScene.ToString());
    }


    public static bool IsSceneActive(Scene scene) {
        return SceneManager.GetActiveScene().name == scene.ToString();
    }
}
