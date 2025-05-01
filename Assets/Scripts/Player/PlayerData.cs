using System;
using Unity.Netcode;

namespace Player {
    public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable {
        public ulong ClientId;
        public int ColorIndex;


        public bool Equals(PlayerData other) {
            return other.ClientId == ClientId && other.ColorIndex == ColorIndex;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref ColorIndex);
        }

        public override string ToString() {
            return $"{ClientId}-{ColorIndex}";
        }
    }
}
