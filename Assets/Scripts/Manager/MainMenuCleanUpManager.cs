using Multiplayer;
using Unity.Netcode;
using UnityEngine;

namespace Manager {
    public class MainMenuCleanUpManager : MonoBehaviour {
        private void Awake() {
            if (NetworkManager.Singleton != null) {
                Destroy(NetworkManager.Singleton.gameObject);
            }
            if (MultiplayerManager.Instance != null) {
                Destroy(MultiplayerManager.Instance.gameObject);
            }
        }
    }
}
