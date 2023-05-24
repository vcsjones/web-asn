using System.Formats.Asn1;

namespace WebAsn;

public partial class AsnNode {
    public SequenceAsnNode Sequence() {
        if (Tag != Asn1Tag.Sequence) {
            throw new InvalidOperationException($"ASN.1 tag {Tag.TagValue} is invalid.");
        }

        return new SequenceAsnNode(Tag, Context, Reader);
    }
}

public sealed class SequenceAsnNode : AsnNode {
    public SequenceAsnNode(Asn1Tag tag, AsnWalkContext context, AsnReader reader) : base(tag, context, reader) {
        reader.ReadSequence(tag);
    }

    public override IEnumerable<AsnNode> GetChildren() {
        AsnWalker walker = new(Context, Contents);
        return walker.Walk();
    }

    public override string Name => "Sequence";
}
