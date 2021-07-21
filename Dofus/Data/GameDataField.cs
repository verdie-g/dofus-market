namespace Dofus.Data
{
    public record GameDataField(string Name, GameDataType Type, GameDataField? InnerField);
}
