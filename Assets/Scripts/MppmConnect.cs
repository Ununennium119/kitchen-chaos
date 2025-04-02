using System.Linq;
using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using UnityEngine;

public class MppmConnect : MonoBehaviour {
    private void Start() {
        var mppmTag = CurrentPlayer.ReadOnlyTags();
        var networkManager = NetworkManager.Singleton;
        if (mppmTag.Contains("Server")) {
            networkManager.StartServer();
        } else if (mppmTag.Contains("Host")) {
            networkManager.StartHost();
        } else if (mppmTag.Contains("Client")) {
            networkManager.StartClient();
        }
    }
}
