﻿namespace ImperialStudio.Core.Api.Networking
{
    public class IncomingPacket
    {
        public INetworkPeer Peer { get; set; }
        public byte PacketId { get; set; }
        public byte[] Data { get; set; }
        public bool IsVerified { get; set; }
        public byte ChannelId { get; set; }
    }
}