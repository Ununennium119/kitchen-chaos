using System;
using System.Diagnostics.CodeAnalysis;

namespace LobbyMenu.Logic {
    [Serializable]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
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
