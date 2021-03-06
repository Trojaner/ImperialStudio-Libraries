﻿using System;
using System.Collections.Generic;
using ImperialStudio.Api.Networking;
using ImperialStudio.Networking.Packets;

namespace ImperialStudio.Networking
{
    public interface IConnectionHandler : IDisposable
    {
        void Send<T>(INetworkPeer peer, T packet, byte packetId) where T : class, IPacket;
        void Send<T>(INetworkPeer peer, T packet) where T : class, IPacket;
        void Send(OutgoingPacket outgoingPacket);
        void Flush();

        IReadOnlyCollection<INetworkPeer> GetPeers(bool authenticatedOnly = true);
        IReadOnlyCollection<INetworkPeer> GetPendingPeers();

        void RegisterPeer(INetworkPeer networkPeer);
        void UnregisterPeer(INetworkPeer networkPeer);

        void RegisterPacket(byte id, PacketDescription packetDescription);
        PacketDescription GetPacketDescription(byte id);

        INetworkPeer GetPeerByNetworkId(uint peerID, bool authenticatedOnly = true);
        INetworkPeer GetPeerBySteamId(ulong steamId, bool authenticatedOnly = true);
    }
}