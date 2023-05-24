using System.Formats.Asn1;

namespace WebAsn;

public partial class AsnNode
{
    public UtcTimeAsnNode UtcTime()
    {
        if (Tag != Asn1Tag.UtcTime)
        {
            throw new InvalidOperationException($"ASN.1 tag {Tag.TagValue} is invalid.");
        }

        return new UtcTimeAsnNode(Tag, Context, Reader);
    }
}

public sealed class UtcTimeAsnNode : AsnNode
{
    private readonly DateTimeOffset _value;

    public UtcTimeAsnNode(Asn1Tag tag, AsnWalkContext context, AsnReader reader) : base(tag, context, reader)
    {
        _value = reader.ReadUtcTime(tag);
    }

    public override string Display => _value.ToString();
}
