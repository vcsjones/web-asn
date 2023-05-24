using System.Formats.Asn1;
using System.Numerics;

namespace WebAsn;

public partial class AsnNode {
    public IntegerAsnNode Integer() {
        if (Tag != Asn1Tag.Integer) {
            throw new InvalidOperationException($"ASN.1 tag {Tag.TagValue} is invalid.");
        }

        return new IntegerAsnNode(Tag, Context, Reader);
    }
}

public sealed class IntegerAsnNode : AsnNode {
    private readonly BigInteger _value;

    public IntegerAsnNode(Asn1Tag tag, AsnWalkContext context, AsnReader reader) : base(tag, context, reader) {
        _value = reader.ReadInteger();
    }

    public override string Display => _value.ToString("R");

    public override List<(string Name, string Value)> GetAdorningAttributes() {
        var attributes = base.GetAdorningAttributes();

        // If the integer is sufficiently big, print it in hex as well. In that case
        // it's probably a serial number or something likely to be "stringified" in
        // hex. However for small integers, we don't need to print things like
        // certificate versions (0-2) in hex as well.
        if (_value > int.MaxValue) {
            attributes.Add(("Hex", $"0x{_value:X2}"));
        }

        return attributes;
    }
}
