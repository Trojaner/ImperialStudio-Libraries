﻿using System;
using System.Linq;
using ImperialStudio.Api.Game;
using ImperialStudio.Api.Logging;
using ImperialStudio.Api.Networking;
using ImperialStudio.Api.Serialization;
using ImperialStudio.Extensions.Logging;
using ImperialStudio.Networking.Client;

namespace ImperialStudio.Networking.Packets.Handlers
{
    public abstract class BasePacketHandler<T> : BasePacketHandler where T : class, IPacket, new()
    {
        protected sealed override void OnHandleVerifiedPacket(IncomingPacket incomingPacket)
        {
            T deserialized;

            try
            {
                deserialized = ObjectSerializer.Deserialize<T>(incomingPacket.Data);
            }
            catch (Exception ex)
            {
                Reject(incomingPacket, "Failed to deserialize: " + ex.Message);
                return;
            }

            OnHandleVerifiedPacket(incomingPacket.Peer, deserialized);
        }

        protected BasePacketHandler(
            IObjectSerializer objectSerializer,
            IGamePlatformAccessor gamePlatformAccessor,
            IConnectionHandler connectionHandler,
            ILogger logger)
            : base(objectSerializer,
                gamePlatformAccessor,
                connectionHandler,
                logger)
        {
        }

        protected abstract void OnHandleVerifiedPacket(INetworkPeer sender, T incomingPacket);
    }

    public abstract class BasePacketHandler : IPacketHandler
    {
        protected IObjectSerializer ObjectSerializer { get; }
        public byte PacketId { get; }

        private readonly IGamePlatformAccessor m_GamePlatformAccessor;
        private readonly IConnectionHandler m_ConnectionHandler;
        private readonly ILogger m_Logger;

        protected BasePacketHandler(IObjectSerializer objectSerializer, IGamePlatformAccessor gamePlatformAccessor,
            IConnectionHandler connectionHandler,
            ILogger logger)
        {
            ObjectSerializer = objectSerializer;
            PacketId = ((PacketTypeAttribute[])GetType().GetCustomAttributes(typeof(PacketTypeAttribute), false)).First().PacketId;
            m_GamePlatformAccessor = gamePlatformAccessor;
            m_ConnectionHandler = connectionHandler;
            m_Logger = logger;
        }

        public void HandlePacket(IncomingPacket incomingPacket)
        {
            OnVerifyPacket(incomingPacket);

            if (incomingPacket.IsVerified)
                OnHandleVerifiedPacket(incomingPacket);
#if LOG_NETWORK
            else
                m_Logger.LogWarning($"Dropped packet from {incomingPacket.Peer}: Packet could not be verified.");
#endif
        }

        protected virtual void OnVerifyPacket(IncomingPacket incomingPacket)
        {
            var currentPlatform = m_GamePlatformAccessor.GamePlatform;
            var packetDescription = m_ConnectionHandler.GetPacketDescription(incomingPacket.PacketId);

            // Validate channel
            if (incomingPacket.ChannelId != packetDescription.ChannelId)
            {
                Reject(incomingPacket, $"Channel mismatch: received: {incomingPacket.ChannelId}, expected: {packetDescription.ChannelId}");
                return;
            }

            // Validate authentication
            if (packetDescription.NeedsAuthentication)
            {
                if (!incomingPacket.Peer.IsAuthenticated)
                {
                    Reject(incomingPacket, "Unauthenticated peer");
                    return;
                }
            }

            // Validate packet direction
            switch (packetDescription.Direction)
            {
                case PacketDirection.ClientToServer when currentPlatform == GamePlatform.Client:
                    {
                        Reject(incomingPacket, "Server tried to send a client packet");
                        return;
                    }

                case PacketDirection.ServerToClient when currentPlatform == GamePlatform.Server:
                    {
                        Reject(incomingPacket, "Client tried to send a server packet");
                        return;
                    }

                case PacketDirection.ClientToClient:
                    if (currentPlatform == GamePlatform.Server)
                    {
                        Reject(incomingPacket, "Client tried to send a client packet");
                        return;
                    }

                    var clientConnectionHandler = (ClientConnectionHandler)m_ConnectionHandler;
                    if (incomingPacket.Peer.Id == clientConnectionHandler.ServerPeer.Id)
                    {
                        Reject(incomingPacket, "Server tried to send a client packet");
                        return;
                    }

                    break;

                case PacketDirection.Any:
                    break;
            }

            incomingPacket.IsVerified = true;
        }

        protected void Reject(IncomingPacket incomingPacket, string reason)
        {
            m_Logger.LogWarning($"Dropped packet \"{incomingPacket.PacketId.ToString()}\" from {incomingPacket.Peer}: {reason}");
        }

        protected abstract void OnHandleVerifiedPacket(IncomingPacket incomingPacket);
    }
}