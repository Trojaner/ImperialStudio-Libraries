﻿using ImperialStudio.Core.Api.Networking;
using ImperialStudio.Core.Eventing;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    public sealed class PongEvent : Event
    {
        public PongEvent(INetworkPeer peer, PongPacket pongPacket)
        {
            Peer = peer;
            PongPacket = pongPacket;
        }

        public INetworkPeer Peer { get; }
        public PongPacket PongPacket { get; }
    }
}