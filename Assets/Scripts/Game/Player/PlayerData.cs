using System;
using Unity.Collections;
using Unity.Netcode;

namespace Game.Player {
    /// <summary>
    /// Represents the data associated with a player in a multiplayer game.
    /// </summary>
    public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable {
        /// <summary>
        /// The client ID assigned by the network.
        /// </summary>
        public ulong ClientId;

        /// <summary>
        /// The index representing the player's color selection.
        /// </summary>
        public int ColorIndex;
        
        /// <summary>
        /// The player's chosen display name (up to 32 bytes).
        /// </summary>
        public FixedString32Bytes Name;
        
        /// <summary>
        /// A unique identifier for the player assigned by lobby (up to 64 bytes).
        /// </summary>
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
