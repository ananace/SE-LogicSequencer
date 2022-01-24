using System;
using System.Collections.Generic;
using LogicSequencer.Serialization.Packets;
using ProtoBuf;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.ModAPI;
using VRage.Utils;

// From Digi's examples
namespace LogicSequencer.Util
{
    /// <summary>
    /// Simple network communication example.
    ///
    /// Always send to server as clients can't send to eachother directly.
    /// Then decide in the packet if it should be relayed to everyone else (except sender and server of course).
    ///
    /// Security note:
    ///  SenderId is not reliable and can be altered by sender to claim they're someone else (like an admin).
    ///  If you need senderId to be secure, a more complicated process is required involving sending
    ///   every player a unique random ID and they sending that ID would confirm their identity.
    /// </summary>
    public class Networking
    {
        public static ushort ModChannel = 1337;

        readonly static Networking _Instance = new Networking(ModChannel);
        public static Networking Instance => _Instance;

        public readonly ushort ChannelId;

        private List<IMyPlayer> tempPlayers = null;

        /// <summary>
        /// <paramref name="channelId"/> must be unique from all other mods that also use network packets.
        /// </summary>
        public Networking(ushort channelId)
        {
            ChannelId = channelId;
        }

        /// <summary>
        /// Register packet monitoring, not necessary if you don't want the local machine to handle incomming packets.
        /// </summary>
        public void Register()
        {
            MyAPIGateway.Multiplayer.RegisterSecureMessageHandler(ChannelId, ReceivedPacket);
        }

        /// <summary>
        /// This must be called on world unload if you called <see cref="Register"/>.
        /// </summary>
        public void Unregister()
        {
            MyAPIGateway.Multiplayer.UnregisterSecureMessageHandler(ChannelId, ReceivedPacket);
        }

        private void ReceivedPacket(ushort _, byte[] rawData, ulong senderId, bool fromServer) // executed when a packet is received on this machine
        {
            try
            {
                var packet = MyAPIGateway.Utilities.SerializeFromBinary<PacketBase>(rawData);
                packet.SenderId = senderId;
                packet.SentFromServer = fromServer;

                HandlePacket(packet, rawData);
            }
            catch(Exception e)
            {
                Log.Error(e, GetType());
            }
        }

        private void HandlePacket(PacketBase packet, byte[] rawData = null)
        {
            var relay = packet.Received();

            if(relay)
                RelayToClients(packet, rawData);
        }

        public void Send(PacketBase packet)
        {
            if(MyAPIGateway.Multiplayer.IsServer)
                RelayToClients(packet);
            else
                SendToServer(packet);
        }

        /// <summary>
        /// Send a packet to the server.
        /// Works from clients and server.
        /// </summary>
        public void SendToServer(PacketBase packet)
        {
            if(MyAPIGateway.Multiplayer.IsServer)
            {
                HandlePacket(packet);
                return;
            }

            var bytes = MyAPIGateway.Utilities.SerializeToBinary(packet);

            MyAPIGateway.Multiplayer.SendMessageToServer(ChannelId, bytes);
        }

        /// <summary>
        /// Send a packet to a specific player.
        /// Only works server side.
        /// </summary>
        public void SendToPlayer(PacketBase packet, ulong steamId)
        {
            if(!MyAPIGateway.Multiplayer.IsServer)
                return;

            var bytes = MyAPIGateway.Utilities.SerializeToBinary(packet);

            MyAPIGateway.Multiplayer.SendMessageTo(ChannelId, bytes, steamId);
        }

        /// <summary>
        /// Sends packet (or supplied bytes) to all players except server player and supplied packet's sender.
        /// Only works server side.
        /// </summary>
        public void RelayToClients(PacketBase packet, byte[] rawData = null)
        {
            if(!MyAPIGateway.Multiplayer.IsServer)
                return;

            if(tempPlayers == null)
                tempPlayers = new List<IMyPlayer>(MyAPIGateway.Session.SessionSettings.MaxPlayers);
            else
                tempPlayers.Clear();

            MyAPIGateway.Players.GetPlayers(tempPlayers);

            foreach(var p in tempPlayers)
            {
                if(p.IsBot)
                    continue;

                if(p.SteamUserId == MyAPIGateway.Multiplayer.ServerId)
                    continue;

                if(p.SteamUserId == packet.SenderId)
                    continue;

                if(rawData == null)
                    rawData = MyAPIGateway.Utilities.SerializeToBinary(packet);

                MyAPIGateway.Multiplayer.SendMessageTo(ChannelId, rawData, p.SteamUserId);
            }

            tempPlayers.Clear();
        }
    }
}
