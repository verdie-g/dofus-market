using System;
using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class IdentificationSuccessMessage : INetworkMessage
    {
        internal static int MessageId => 7272;
        public bool HasRights { get; private set; }
        public bool HasConsoleRights { get; private set; }
        public bool WasAlreadyConnected { get; private set; }
        public string Login { get; private set; } = string.Empty;
        public AccountTagInformation AccountTag { get; private set; } = default!;
        public int AccountId { get; private set; }
        public byte CommunityId { get; private set; }
        public string SecretQuestion { get; private set; } = string.Empty;
        public double AccountCreation { get; private set; }
        public double SubscriptionElapsedDuration { get; private set; }
        public double SubscriptionEndDate { get; private set; }
        public sbyte HavenBagAvailableRoom { get; private set; }

        public void Serialize(DofusBinaryWriter writer) => throw new NotImplementedException();

        public void Deserialize(DofusBinaryReader reader)
        {
            byte flags = reader.ReadByte();
            HasRights = (flags & 1) != 0;
            HasConsoleRights = (flags & 2) != 0;
            WasAlreadyConnected = (flags & 4) != 0;
            Login = reader.ReadString();
            AccountTag = new AccountTagInformation();
            AccountTag.Deserialize(reader);
            AccountId = reader.ReadInt32();
            CommunityId = reader.ReadByte();
            SecretQuestion = reader.ReadString();
            AccountCreation = reader.ReadDouble();
            SubscriptionElapsedDuration = reader.ReadDouble();
            SubscriptionEndDate = reader.ReadDouble();
            HavenBagAvailableRoom = (sbyte)reader.ReadByte();
        }
    }
}
