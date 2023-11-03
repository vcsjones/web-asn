using System.Formats.Asn1;
using System.Text;

namespace WebAsn;

public partial class AsnNode {
    public UnknownAsnNode Unknown() {
        return new UnknownAsnNode(Tag, Context, Reader);
    }
}

public sealed class UnknownAsnNode : AsnNode {
    private readonly AsnNode[] _children;

    public UnknownAsnNode(Asn1Tag tag, AsnWalkContext context, AsnReader reader) : base(tag, context, reader) {
        // We need to advance the reader or we'll loop forever.
        reader.ReadEncodedValue();

        AsnWalker walker = new(context with { Synthetic = context.Synthetic || !tag.IsConstructed }, Contents);
        _children = DecodeChildren(walker);
    }

    public override IEnumerable<AsnNode> GetChildren() => _children;

    public override string? Display {
        get {
            // If we auto-decoded some child elements, then we don't want to display
            // a value.
            if (_children.Length > 0) {
                return base.Display;
            }

            // If it's universal we should be handling them already. This would
            // mean there is a universal tag we don't know how to display.
            if (Tag.TagClass == TagClass.Universal) {
                return base.Display;
            }

            bool valid = true;
            ReadOnlySpan<byte> contents = Contents.Span;

            for (int i = 0; i < contents.Length && valid; i++) {
                byte c = contents[i];

                if (c is > 0x7E or (< 0x20 and not 0x0A and not 0x0C)) {
                    valid = false;
                }
            }

            if (valid) {
                return Encoding.ASCII.GetString(contents);
            }
            else {
                return Convert.ToHexString(contents);
            }
        }
    }

    public override string Name {
        get {
            if (Tag.TagClass == TagClass.ContextSpecific) {
                return $"[{Tag.TagValue}]";
            }
            else if (Tag.TagClass == TagClass.Application) {
                return $"{{{Tag.TagValue}}}";
            }

            return Tag.ToString();
        }
    }
}
