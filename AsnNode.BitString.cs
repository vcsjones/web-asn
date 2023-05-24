using System.Formats.Asn1;
using System.Numerics;
using System.Text;

namespace WebAsn;

public partial class AsnNode {
    public PrimitiveBitStringAsnNode PrimitiveBitString() {
        if (Tag != Asn1Tag.PrimitiveBitString) {
            throw new InvalidOperationException($"ASN.1 tag {Tag.TagValue} is invalid.");
        }

        return new PrimitiveBitStringAsnNode(Tag, Context, Reader);
    }
}

public sealed class PrimitiveBitStringAsnNode : AsnNode {
    private readonly byte[] _value;
    private readonly int _unusedBits;

    public PrimitiveBitStringAsnNode(Asn1Tag tag, AsnWalkContext context, AsnReader reader) : base(tag, context, reader) {
        _value = reader.ReadBitString(out _unusedBits, tag);
    }

    public override List<(string Name, string Value)> GetAdorningAttributes() {
        var attributes = base.GetAdorningAttributes();
        attributes.Add(("Bits", (_value.Length * 8 - _unusedBits).ToString()));
        return attributes;
    }

    public override string Display {
        get {
            int totalBits = _value.Length * 8 - _unusedBits;

            if (totalBits == 0) {
                return string.Empty;
            }

            BigInteger integer = new(_value, isUnsigned: true, isBigEndian: true);
            StringBuilder builder = new(totalBits, totalBits);

            for (int i = totalBits - 1; i >= 0; i--) {
                if (((integer >> i) & 1) == 1) {
                    builder.Append('1');
                }
                else {
                    builder.Append('0');
                }
            }

            return builder.ToString();
        }
    }
}
