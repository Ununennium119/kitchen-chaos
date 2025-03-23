using UnityEngine.SceneManagement;

public static class SceneLoader {
    public enum Scene {
        MainMenuScene,
        LoadingScene,
        GameScene,
    }


    private static Scene _targetScene;


    public static void LoadScene(Scene scene) {
        _targetScene = scene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoadSceneCallback() {
        SceneManager.LoadScene(_targetScene.ToString());
    }
}
