using System.Formats.Asn1;

namespace WebAsn;

public partial class AsnNode {
    public GeneralizedTimeAsnNode GeneralizedTime() {
        if (Tag != Asn1Tag.GeneralizedTime) {
            throw new InvalidOperationException($"ASN.1 tag {Tag.TagValue} is invalid.");
        }

        return new GeneralizedTimeAsnNode(Tag, Context, Reader);
    }
}

public sealed class GeneralizedTimeAsnNode : AsnNode {
    private readonly DateTimeOffset _value;
    private readonly string _stringValue;

    public GeneralizedTimeAsnNode(Asn1Tag tag, AsnWalkContext context, AsnReader reader) : base(tag, context, reader) {
        ReadOnlyMemory<byte> bytes = reader.PeekContentBytes();
        _value = reader.ReadGeneralizedTime(tag);

        // If reading as a generalized time worked, then we should be able to decode the contents as an ASCII string.
        _stringValue = System.Text.Encoding.ASCII.GetString(bytes.Span);
    }

    public override List<(string Name, string? Value)> GetAdorningAttributes()
    {
        var attributes = base.GetAdorningAttributes();
        attributes.Add(("Value", _stringValue));
        return attributes;
    }

    public override string Display => _value.ToString();
}
