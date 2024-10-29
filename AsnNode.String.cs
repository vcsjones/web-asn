using System.Formats.Asn1;

namespace WebAsn;

public partial class AsnNode {
    public StringAsnNode String() {
        return new StringAsnNode(Tag, Context, Reader);
    }
}

public sealed class StringAsnNode : AsnNode {
    private readonly string _value;
    private readonly bool _illegal;

    public StringAsnNode(Asn1Tag tag, AsnWalkContext context, AsnReader reader) : base(tag, context, reader) {
        try {
            _value = reader.ReadCharacterString((UniversalTagNumber)tag.TagValue);
        }
        catch (AsnContentException) {
            _illegal = true;
            _value = Convert.ToHexString(reader.PeekContentBytes().Span);
            reader.ReadEncodedValue();
        }
    }

    public override string Display => _value;

    public override List<(string Name, string? Value)> GetAdorningAttributes()
    {
        var baseAttributes = base.GetAdorningAttributes();

        if (_illegal) {
            baseAttributes.Insert(0, ("Illegal Encoding", null));
        }

        return baseAttributes;
    }
}
