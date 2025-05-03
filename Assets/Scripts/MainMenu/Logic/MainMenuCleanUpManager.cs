using Common.Logic;
using LobbyMenu.Logic;
using Unity.Netcode;
using UnityEngine;

namespace MainMenu.Logic {
    /// <summary>
    /// This class handles the cleanup of "DontDestroyOnLoad" game objects when transitioning to the main menu scene.
    /// </summary>
    public class MainMenuCleanUpManager : MonoBehaviour {
        private void Awake() {
            var isHost = NetworkManager.Singleton?.IsHost == true;
            if (!isHost) return;

            if (MultiplayerManager.Instance != null) {
                Destroy(MultiplayerManager.Instance.gameObject);
            }
            if (LobbyManager.Instance != null) {
                Destroy(LobbyManager.Instance.gameObject);
            }
            if (NetworkManager.Singleton != null) {
                Destroy(NetworkManager.Singleton.gameObject);
            }
        }
    }
}
