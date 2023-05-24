using System.Formats.Asn1;
using System.Security.Cryptography;

namespace WebAsn;

public partial class AsnNode
{
    public ObjectIdentiferAsnNode ObjectIdentifier()
    {
        if (Tag != Asn1Tag.ObjectIdentifier)
        {
            throw new InvalidOperationException($"ASN.1 tag {Tag.TagValue} is invalid.");
        }

        return new ObjectIdentiferAsnNode(Tag, Context, Reader);
    }
}

public sealed class ObjectIdentiferAsnNode : AsnNode
{
    private readonly string _value;

    public ObjectIdentiferAsnNode(Asn1Tag tag, AsnWalkContext context, AsnReader reader) : base(tag, context, reader)
    {
        _value = reader.ReadObjectIdentifier(tag);
    }

    public override string Display
    {
        get
        {
            Oid oid = new(_value);

            if (!string.IsNullOrWhiteSpace(oid.FriendlyName))
            {
                return $"{_value} ({oid.FriendlyName})";
            }

            return _value;
        }
    }
}
