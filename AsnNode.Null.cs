using System.Formats.Asn1;

namespace WebAsn;

public partial class AsnNode {
    public NullAsnNode Null() {
        if (Tag != Asn1Tag.Null) {
            throw new InvalidOperationException($"ASN.1 tag {Tag.TagValue} is invalid.");
        }

        return new NullAsnNode(Tag, Context, Reader);
    }
}

public sealed class NullAsnNode : AsnNode {
    public NullAsnNode(Asn1Tag tag, AsnWalkContext context, AsnReader reader) : base(tag, context, reader) {
        reader.ReadNull(tag);
    }

    public override string Display => "Null";
}
