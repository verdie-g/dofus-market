using System;
using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class IdentificationMessage : INetworkMessage
    {
        internal static int MessageId => 2767;

        public bool AutoConnect { get; set; }
        public bool UseCertificate { get; set; }
        public bool UseLoginToken { get; set; }
        public DofusVersion Version { get; set; } = new();
        public string Lang { get; set; } = string.Empty;
        public byte[] Credentials { get; set; } = Array.Empty<byte>();
        public short ServerId { get; set; }
        public long SessionOptionalSalt { get; set; }
        public short[] FailedAttempts { get; set; } = Array.Empty<short>();

        public void Serialize(DofusBinaryWriter writer)
        {
            int flags = (AutoConnect ? 0x1 : 0x0) | (UseCertificate ? 0x2 : 0x0) | (UseLoginToken ? 0x4 : 0x0);
            writer.Write((byte)flags);
            Version.Serialize(writer);
            writer.Write(Lang);
            writer.Write7BitEncodedInt(Credentials.Length);
            writer.Write(Credentials);
            writer.Write(ServerId);
            writer.Write7BitEncodedInt64(SessionOptionalSalt);
            writer.Write((short)FailedAttempts.Length);
            foreach (short failedAttempt in FailedAttempts)
            {
                writer.Write7BitEncodedInt(failedAttempt);
            }
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            int flags = reader.ReadByte();
            AutoConnect = (flags & 0x1) != 0;
            UseCertificate = (flags & 0x2) != 0;
            UseLoginToken = (flags & 0x4) != 0;
            Version = new DofusVersion();
            Version.Deserialize(reader);
            Lang = reader.ReadString();
            int credentialsLength = reader.Read7BitEncodedInt();
            Credentials = reader.ReadBytes(credentialsLength);
            ServerId = reader.ReadInt16();
            SessionOptionalSalt = reader.Read7BitEncodedInt64();
            short failedAttemptsLength = reader.ReadInt16();
            FailedAttempts = new short[failedAttemptsLength];
            for (short i = 0; i < failedAttemptsLength; i += 1)
            {
                FailedAttempts[i] = (short)reader.Read7BitEncodedInt();
            }
        }
    }
}
