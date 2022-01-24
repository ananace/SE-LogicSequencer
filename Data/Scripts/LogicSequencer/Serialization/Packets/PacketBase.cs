using ProtoBuf;
using Sandbox.ModAPI;
using VRage.ModAPI;

namespace LogicSequencer.Serialization.Packets
{
    // tag numbers in ProtoInclude collide with numbers from ProtoMember in the same class, therefore they must be unique.
    [ProtoInclude(100, typeof(EntityPacketBase))]
    [ProtoInclude(101, typeof(LogicSequencerBlockSettingsChanged))]
    [ProtoInclude(102, typeof(LogicSequencerBlockScriptChanged))]
    [ProtoContract(UseProtoMembersOnly = true)]
    public abstract class PacketBase
    {
        // this field's value will be sent if it's not the default value.
        // to define a default value you must use the [DefaultValue(...)] attribute.
        //[ProtoMember(1)]

        public ulong SenderId;
        public bool SentFromServer;

        public PacketBase()
        {
            SenderId = MyAPIGateway.Multiplayer.MyId;
            SentFromServer = MyAPIGateway.Multiplayer.IsServer;
        }

        /// <summary>
        /// Called when this packet is received on this machine.
        /// </summary>
        /// <returns>Return true if you want the packet to be sent to other clients (only works server side)</returns>
        public abstract bool Received();
    }

    [ProtoContract(UseProtoMembersOnly = true)]
    public abstract class EntityPacketBase : PacketBase
    {
        // this field's value will be sent if it's not the default value.
        // to define a default value you must use the [DefaultValue(...)] attribute.
        [ProtoMember(1)]
        public long EntityID;

        public IMyEntity AsEntity => MyAPIGateway.Entities.GetEntityById(EntityID);
        public IMyTerminalBlock AsBlock => MyAPIGateway.Entities.GetEntityById(EntityID) as IMyTerminalBlock;
    }
}
