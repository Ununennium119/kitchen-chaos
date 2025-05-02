using Common;
using Common.Logic;
using LobbyMenu;
using LobbyMenu.Logic;
using Unity.Netcode;
using UnityEngine;

namespace MainMenu.Logic {
    public class MainMenuCleanUpManager : MonoBehaviour {
        private void Awake() {
            if (NetworkManager.Singleton != null) {
                Destroy(NetworkManager.Singleton.gameObject);
            }
            if (MultiplayerManager.Instance != null) {
                Destroy(MultiplayerManager.Instance.gameObject);
            }
            if (LobbyManager.Instance != null) {
                Destroy(LobbyManager.Instance.gameObject);
            }
        }
    }
}
