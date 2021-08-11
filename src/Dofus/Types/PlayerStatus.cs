using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class PlayerStatus : INetworkMessage
    {
        internal static ushort MessageId => 223;

        public static readonly PlayerStatus Offline = new() { StatusId = 0 };
        public static readonly PlayerStatus Unknown = new() { StatusId = 1 };
        public static readonly PlayerStatus Available = new() { StatusId = 10 };
        public static readonly PlayerStatus Idle = new() { StatusId = 20 };
        public static readonly PlayerStatus Afk = new() { StatusId = 21 };
        public static readonly PlayerStatus Private = new() { StatusId = 30 };
        public static readonly PlayerStatus Solo = new() { StatusId = 40 };

        public byte StatusId { get; set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            writer.Write(StatusId);
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}
