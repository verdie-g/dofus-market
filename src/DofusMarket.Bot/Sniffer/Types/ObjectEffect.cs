using DofusMarket.Bot.Serialization;

namespace DofusMarket.Bot.Sniffer.Types;

internal record ObjectEffect : INetworkMessage
{
    internal static ushort MessageId => 4941;

    public ushort ActionId { get; private set; }

    public void Deserialize(DofusBinaryReader reader)
    {
        ActionId = (ushort)reader.Read7BitEncodedInt();
    }
}

internal record ObjectEffectCreature : ObjectEffect, INetworkMessage
{
    internal new static ushort MessageId => 9763;

    public uint MonsterFamilyId { get; private set; }

    public new void Deserialize(DofusBinaryReader reader)
    {
        base.Deserialize(reader);
        MonsterFamilyId = (ushort)reader.Read7BitEncodedInt();
    }
}

internal record ObjectEffectDate : ObjectEffect, INetworkMessage
{
    internal new static ushort MessageId => 9560;

    public uint Year { get; private set; }
    public byte Month { get; private set; }
    public byte Day { get; private set; }
    public byte Hour { get; private set; }
    public byte Minute { get; private set; }

    public new void Deserialize(DofusBinaryReader reader)
    {
        base.Deserialize(reader);
        Year = (ushort)reader.Read7BitEncodedInt();
        Month = reader.ReadByte();
        Day = reader.ReadByte();
        Hour = reader.ReadByte();
        Minute = reader.ReadByte();
    }
}

internal record ObjectEffectDice : ObjectEffect, INetworkMessage
{
    internal new static ushort MessageId => 2568;

    public uint DiceNum { get; private set; }
    public uint DiceSide { get; private set; }
    public uint DiceConst { get; private set; }

    public new void Deserialize(DofusBinaryReader reader)
    {
        base.Deserialize(reader);
        DiceNum = (uint)reader.Read7BitEncodedInt();
        DiceSide = (uint)reader.Read7BitEncodedInt();
        DiceConst = (uint)reader.Read7BitEncodedInt();
    }
}

internal record ObjectEffectDuration : ObjectEffect, INetworkMessage
{
    internal new static ushort MessageId => 84 * 60;

    public ushort Days { get; private set; }
    public byte Hours { get; private set; }
    public byte Minutes { get; private set; }

    public new void Deserialize(DofusBinaryReader reader)
    {
        base.Deserialize(reader);
        Days = (ushort)reader.Read7BitEncodedInt();
        Hours = reader.ReadByte();
        Minutes = reader.ReadByte();
    }
}

internal record ObjectEffectInteger : ObjectEffect, INetworkMessage
{
    internal new static ushort MessageId => 7541;

    public uint Value { get; private set; }

    public new void Deserialize(DofusBinaryReader reader)
    {
        base.Deserialize(reader);
        Value = (uint)reader.Read7BitEncodedInt();
    }
}

internal record ObjectEffectLadder : ObjectEffect, INetworkMessage
{
    internal new static ushort MessageId => 5261;

    public uint MonsterCount { get; set; }

    public new void Deserialize(DofusBinaryReader reader)
    {
        base.Deserialize(reader);
        MonsterCount = (uint)reader.Read7BitEncodedInt();
    }
}

internal record ObjectEffectMinMax : ObjectEffect, INetworkMessage
{
    internal new static ushort MessageId => 1473;

    public uint Min { get; private set; }
    public uint Max { get; private set; }

    public new void Deserialize(DofusBinaryReader reader)
    {
        base.Deserialize(reader);
        Min = (uint)reader.Read7BitEncodedInt();
        Max = (uint)reader.Read7BitEncodedInt();
    }
}

internal record ObjectEffectMount : ObjectEffect, INetworkMessage
{
    internal new static ushort MessageId => 9223;

    public bool Sex { get; private set; }
    public bool IsRideable { get; private set; }
    public bool IsFeconded { get; private set; }
    public bool IsFecondationReady { get; private set; }
    public ulong Id { get; private set; }
    public ulong ExpirationDate { get; private set; }
    public uint Model { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Owner { get; private set; } = string.Empty;
    public byte Level { get; private set; }
    public int ReproductionCount { get; private set; }
    public uint ReproductionCountMax { get; private set; }
    public ObjectEffectInteger[] Effects { get; private set; } = Array.Empty<ObjectEffectInteger>();
    public uint[] Capacities { get; private set; } = Array.Empty<uint>();

    public new void Deserialize(DofusBinaryReader reader)
    {
        base.Deserialize(reader);

        byte flags = reader.ReadByte();
        Sex = (flags & 0x01) != 0;
        IsRideable = (flags & 0x02) != 0;
        IsFeconded = (flags & 0x04) != 0;
        IsFecondationReady = (flags & 0x08) != 0;

        Id = (ulong)reader.Read7BitEncodedInt64();
        ExpirationDate = (ulong)reader.Read7BitEncodedInt64();
        Model = (uint)reader.Read7BitEncodedInt();
        Name = reader.ReadString();
        Owner = reader.ReadString();
        Level = reader.ReadByte();
        ReproductionCount = reader.Read7BitEncodedInt();
        ReproductionCountMax = (uint)reader.Read7BitEncodedInt();
        Effects = reader.ReadObjectCollection<ObjectEffectInteger>();
        Capacities = reader.ReadCollection(r => (uint)r.Read7BitEncodedInt());
    }
}

internal record ObjectEffectString : ObjectEffect, INetworkMessage
{
    internal new static ushort MessageId => 3954;

    public string Value { get; private set; } = string.Empty;

    public new void Deserialize(DofusBinaryReader reader)
    {
        base.Deserialize(reader);
        Value = reader.ReadString();
    }
}
