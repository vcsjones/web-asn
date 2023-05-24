using System.Formats.Asn1;

namespace WebAsn;

public partial class AsnNode
{
    public PrimitiveOctetStringAsnNode PrimitiveOctetString()
    {
        if (Tag != Asn1Tag.PrimitiveOctetString)
        {
            throw new InvalidOperationException($"ASN.1 tag {Tag.TagValue} is invalid.");
        }

        return new PrimitiveOctetStringAsnNode(Tag, Context, Reader);
    }
}

public sealed class PrimitiveOctetStringAsnNode : AsnNode
{
    private readonly ReadOnlyMemory<byte> _value;

    public PrimitiveOctetStringAsnNode(Asn1Tag tag, AsnWalkContext context, AsnReader reader) : base(tag, context, reader)
    {
        _value = reader.ReadOctetString(Tag);
    }

    public override string Display => Convert.ToHexString(_value.Span);

    public override IEnumerable<AsnNode> GetChildren()
    {
        AsnWalker walker = new(Context with { Synthetic = true }, Contents);

        try
        {
            // We want to up-front validate all of the contents so that we can back-out
            // if it turns out we can't walk it.
            return walker.Walk().ToArray();
        }
        catch
        {
            return Array.Empty<AsnNode>();
        }

    }
}
