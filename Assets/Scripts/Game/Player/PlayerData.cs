using System;
using Unity.Collections;
using Unity.Netcode;

namespace Game.Player {
    public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable {
        public ulong ClientId;
        public int ColorIndex;
        public FixedString32Bytes Name;
        public FixedString64Bytes PlayerId;


        public bool Equals(PlayerData other) {
            return other.ClientId == ClientId
                   && other.ColorIndex == ColorIndex
                   && other.Name == Name
                   && other.PlayerId == PlayerId;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref ColorIndex);
            serializer.SerializeValue(ref Name);
            serializer.SerializeValue(ref PlayerId);
        }
    }
}
