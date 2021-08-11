namespace Dofus
{
    internal readonly struct RawNetworkMessage
    {
        public readonly ushort Id;
        public readonly byte[] Content;

        public RawNetworkMessage(ushort id, byte[] content)
        {
            Id = id;
            Content = content;
        }
    }
}
