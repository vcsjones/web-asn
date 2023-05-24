using System.Formats.Asn1;

namespace WebAsn;

public partial class AsnNode
{
    public StringAsnNode String()
    {
        return new StringAsnNode(Tag, Context, Reader);
    }
}

public sealed class StringAsnNode : AsnNode
{
    private readonly string _value;

    public StringAsnNode(Asn1Tag tag, AsnWalkContext context, AsnReader reader) : base(tag, context, reader)
    {
        _value = reader.ReadCharacterString((UniversalTagNumber)tag.TagValue);
    }

    public override string Display => _value;
}
