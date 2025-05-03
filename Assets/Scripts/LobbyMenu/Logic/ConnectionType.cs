using System;

namespace LobbyMenu.Logic {
    [Serializable]
    internal enum ConnectionType {
        Udp,
        Dtls,
        Wss
    }

    internal static class ConnectionTypeMethods {
        public static string GetValue(this ConnectionType connectionType) {
            return connectionType.ToString().ToLower();
        }
    }
}
