using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace Common.Utility {
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


        /// <summary>
        /// Loads the specified scene.
        /// </summary>
        /// <param name="scene">The scene to load.</param>
        public static void LoadScene(Scene scene) {
            _targetScene = scene;
            SceneManager.LoadScene(Scene.LoadingScene.ToString());
        }

        /// <summary>
        /// Loads a networked scene directly.
        /// </summary>
        /// <param name="scene">The scene to load.</param>
        public static void LoadNetwork(Scene scene) {
            NetworkManager.Singleton.SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
        }

        /// <summary>
        /// This method is called after a loading scene to load the actual target scene.
        /// </summary>
        public static void LoadSceneCallback() {
            SceneManager.LoadScene(_targetScene.ToString());
        }

        /// <param name="scene">The scene to check.</param>
        /// <returns><c>true</c> if the scene is active, otherwise <c>false</c>.</returns>
        public static bool IsSceneActive(Scene scene) {
            return SceneManager.GetActiveScene().name == scene.ToString();
        }
    }
}
