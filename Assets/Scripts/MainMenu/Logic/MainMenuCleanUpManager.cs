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
            if (MultiplayerManager.Instance != null) {
                Debug.Log("Destroying multiplayer manager...");
                Destroy(MultiplayerManager.Instance.gameObject);
            }
            if (LobbyManager.Instance != null) {
                Debug.Log("Destroying lobby manager...");
                Destroy(LobbyManager.Instance.gameObject);
            }
            if (NetworkManager.Singleton != null) {
                Debug.Log("Destroying network manager...");
                Destroy(NetworkManager.Singleton.gameObject);
            }
        }
    }
}
