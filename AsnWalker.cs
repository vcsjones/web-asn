using System.Formats.Asn1;

namespace WebAsn;

public sealed class AsnWalker {
    private readonly AsnWalkContext _context;
    private readonly AsnReader _reader;
    private static readonly AsnReaderOptions _options = new() { SkipSetSortOrderVerification = true };

    public AsnWalker(AsnWalkContext context, ReadOnlyMemory<byte> asn) {
        _context = context;
        _reader = new(asn, AsnEncodingRules.BER, _options);
    }

    public AsnWalker(AsnWalkContext context, AsnReader reader) {
        _context = context;
        _reader = reader;
    }


    public IEnumerable<AsnNode> Walk() {

        while (_reader.HasData) {
            Asn1Tag tag = _reader.PeekTag();

            AsnNode basicNode = new(tag, _context, _reader);

            if (tag == Asn1Tag.Sequence) {
                yield return basicNode.Sequence();
            }
            else if (tag == Asn1Tag.SetOf) {
                yield return basicNode.Set();
            }
            else if (tag == Asn1Tag.Integer) {
                yield return basicNode.Integer();
            }
            else if (tag == Asn1Tag.ObjectIdentifier) {
                yield return basicNode.ObjectIdentifier();
            }
            else if (tag == Asn1Tag.UtcTime) {
                yield return basicNode.UtcTime();
            }
            else if (tag == Asn1Tag.GeneralizedTime) {
                yield return basicNode.GeneralizedTime();
            }
            else if (
                tag == new Asn1Tag(UniversalTagNumber.IA5String) ||
                tag == new Asn1Tag(UniversalTagNumber.BMPString) ||
                tag == new Asn1Tag(UniversalTagNumber.PrintableString) ||
                tag == new Asn1Tag(UniversalTagNumber.GeneralString) ||
                tag == new Asn1Tag(UniversalTagNumber.NumericString) ||
                tag == new Asn1Tag(UniversalTagNumber.VisibleString) ||
                tag == new Asn1Tag(UniversalTagNumber.TeletexString) ||
                tag == new Asn1Tag(UniversalTagNumber.UTF8String)) {
                    yield return basicNode.String();
            }
            else if (tag == Asn1Tag.Boolean) {
                yield return basicNode.Boolean();
            }
            else if (tag == Asn1Tag.PrimitiveOctetString) {
                yield return basicNode.PrimitiveOctetString();
            }
            else if (tag == Asn1Tag.PrimitiveBitString) {
                yield return basicNode.PrimitiveBitString();
            }
            else if (tag == Asn1Tag.Enumerated) {
                yield return basicNode.Enumerated();
            }
            else if (tag == Asn1Tag.Null) {
                yield return basicNode.Null();
            }
            else {
                // We don't want EoC tags to be visible or synthetically decode them, however they must be consumed.
                // So consume the node as unknown, but only yield it if it is not EoC.
                AsnNode node = basicNode.Unknown();

                if (tag != new Asn1Tag(UniversalTagNumber.EndOfContents)) {
                    yield return node;
                }
            }
        }
    }
}
