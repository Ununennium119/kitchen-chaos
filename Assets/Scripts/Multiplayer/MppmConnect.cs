using System.Linq;
using Unity.Multiplayer.Playmode;
using Unity.Netcode;

namespace Multiplayer {
    public class MppmConnect : NetworkBehaviour {
        private void Start() {
            var multiplayerManager = MultiplayerManager.Instance;
            var mppmTag = CurrentPlayer.ReadOnlyTags();
            if (mppmTag.Contains("Server")) {
                multiplayerManager.StartServer();
            } else if (mppmTag.Contains("Host")) {
                multiplayerManager.StartHost();
            } else if (mppmTag.Contains("Client")) {
                multiplayerManager.StartClient();
            }
        }
    }
}
