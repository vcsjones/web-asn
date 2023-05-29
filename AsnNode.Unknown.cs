using System.Formats.Asn1;
using System.Text;

namespace WebAsn;

public partial class AsnNode {
    public UnknownAsnNode Unknown() {
        return new UnknownAsnNode(Tag, Context, Reader);
    }
}

public sealed class UnknownAsnNode : AsnNode {
    private static readonly Encoding UTF8Throwing = new UTF8Encoding(false, true);

    private readonly AsnNode[] _children;

    public UnknownAsnNode(Asn1Tag tag, AsnWalkContext context, AsnReader reader) : base(tag, context, reader) {
        // We need to advance the reader or we'll loop forever.
        reader.ReadEncodedValue();

        AsnWalker walker = new(context with { Synthetic = context.Synthetic || !tag.IsConstructed }, Contents);

        try {
            // We want to up-front validate all of the contents so that we can back-out
            // if it turns out we can't walk it.
            _children = walker.Walk().ToArray();
        }
        catch {
            _children = Array.Empty<AsnNode>();
        }
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

            string ascii = Encoding.ASCII.GetString(Contents.Span);

            try {
                string utf8 = UTF8Throwing.GetString(Contents.Span);

                // We only want text if they appear to be ASCII. If the UTF8
                // and ASCII encoding disagree on the decoded text, then substitution
                // took place and we don't want the string.
                if (utf8 == ascii)
                {
                    return utf8;
                }
            }
            catch (DecoderFallbackException) {
            }

            return Convert.ToHexString(Contents.Span);
        }
    }

    public override string Name {
        get {
            if (Tag.TagClass == TagClass.ContextSpecific) {
                return $"[{Tag.TagValue}]";
            }
            else if (Tag.TagClass == TagClass.Application) {
                return $"{{Tag.TagValue}}";
            }

            return Tag.ToString();
        }
    }
}
