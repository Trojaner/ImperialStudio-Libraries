﻿using ImperialStudio.Api.Eventing;
using ImperialStudio.Api.Game;
using ImperialStudio.Api.Logging;
using ImperialStudio.Api.Networking;
using ImperialStudio.Api.Serialization;

namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(Packets.PacketType.Ping)]
    public class PingHandler : BasePacketHandler<PingPacket>
    {
        private readonly IEventBus m_EventBus;
        private readonly IConnectionHandler m_ConnectionHandler;

        public PingHandler(IEventBus eventBus, IObjectSerializer objectSerializer, IGamePlatformAccessor gamePlatformAccessor, IConnectionHandler connectionHandler, ILogger logger) 
            : base(objectSerializer, gamePlatformAccessor,  connectionHandler, logger)
        {
            m_EventBus = eventBus;
            m_ConnectionHandler = connectionHandler;
        }

        protected override void OnHandleVerifiedPacket(INetworkPeer sender, PingPacket incomingPacket)
        {
            var @event = new PingEvent(sender, incomingPacket);
            m_EventBus.Emit(this, @event);

            if (@event.IsCancelled)
                return;

            PongPacket pongPacket = new PongPacket
            {
                PingId = incomingPacket.PingId
            };

            m_ConnectionHandler.Send(sender, pongPacket);
        }
    }
}