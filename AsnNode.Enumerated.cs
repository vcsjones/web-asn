using System.Formats.Asn1;
using System.Numerics;

namespace WebAsn;

public partial class AsnNode {
    public EnumeratedAsnNode Enumerated() {
        if (Tag != Asn1Tag.Enumerated) {
            throw new InvalidOperationException($"ASN.1 tag {Tag.TagValue} is invalid.");
        }

        return new EnumeratedAsnNode(Tag, Context, Reader);
    }
}

public sealed class EnumeratedAsnNode : AsnNode {
    private readonly BigInteger _value;

    public EnumeratedAsnNode(Asn1Tag tag, AsnWalkContext context, AsnReader reader) : base(tag, context, reader) {
        _value = new BigInteger(reader.ReadEnumeratedBytes().Span, isBigEndian: true);
    }

    public override string Display => _value.ToString("R");
}
