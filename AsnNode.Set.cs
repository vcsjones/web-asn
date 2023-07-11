using System.Formats.Asn1;

namespace WebAsn;

public partial class AsnNode {
    public SetAsnNode Set() {
        if (Tag != Asn1Tag.SetOf) {
            throw new InvalidOperationException($"ASN.1 tag {Tag.TagValue} is invalid.");
        }

        return new SetAsnNode(Tag, Context, Reader);
    }
}

public sealed class SetAsnNode : AsnNode {
    private readonly AsnReader _set;

    public SetAsnNode(Asn1Tag tag, AsnWalkContext context, AsnReader reader) : base(tag, context, reader) {
        _set = reader.ReadSetOf(tag);
    }

    public override IEnumerable<AsnNode> GetChildren() {
        AsnWalker walker = new(Context, _set.Clone());
        return walker.Walk();
    }

    public override string Name => "SetOf";
}
