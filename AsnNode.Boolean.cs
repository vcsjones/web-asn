using System.Formats.Asn1;

namespace WebAsn;

public partial class AsnNode
{
    public BooleanAsnNode Boolean()
    {
        if (Tag != Asn1Tag.Boolean)
        {
            throw new InvalidOperationException($"ASN.1 tag {Tag.TagValue} is invalid.");
        }

        return new BooleanAsnNode(Tag, Context, Reader);
    }
}

public sealed class BooleanAsnNode : AsnNode
{
    private readonly bool _value;

    public BooleanAsnNode(Asn1Tag tag, AsnWalkContext context, AsnReader reader) : base(tag, context, reader)
    {
        _value = reader.ReadBoolean(tag);
    }

    public override string Display => _value.ToString();
}
