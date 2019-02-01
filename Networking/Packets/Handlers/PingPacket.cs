﻿using ImperialStudio.Core.Api.Networking.Packets;
using ZeroFormatter;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(PacketType.Ping)]
    [ZeroFormattable]
    public class PingPacket : IPacket
    {
        [Index(0)]
        public virtual ulong PingId { get; set; }
    }
}