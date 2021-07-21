namespace Dofus
{
    internal readonly struct RawNetworkMessage
    {
        public readonly int Id;
        public readonly byte[] Content;

        public RawNetworkMessage(int id, byte[] content)
        {
            Id = id;
            Content = content;
        }
    }
}
