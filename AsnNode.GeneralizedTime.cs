using System.Formats.Asn1;

namespace WebAsn;

public partial class AsnNode
{
    public GeneralizedTimeAsnNode GeneralizedTime()
    {
        if (Tag != Asn1Tag.GeneralizedTime)
        {
            throw new InvalidOperationException($"ASN.1 tag {Tag.TagValue} is invalid.");
        }

        return new GeneralizedTimeAsnNode(Tag, Context, Reader);
    }
}

public sealed class GeneralizedTimeAsnNode : AsnNode
{
    private readonly DateTimeOffset _value;

    public GeneralizedTimeAsnNode(Asn1Tag tag, AsnWalkContext context, AsnReader reader) : base(tag, context, reader)
    {
        _value = reader.ReadGeneralizedTime(tag);
    }

    public override string Display => _value.ToString();
}
