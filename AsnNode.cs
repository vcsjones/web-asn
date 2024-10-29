using System.Diagnostics;
using System.Formats.Asn1;
using System.Linq;

namespace WebAsn;

public partial class AsnNode {
    public ReadOnlyMemory<byte> Contents { get; }
    public ReadOnlyMemory<byte> Raw { get; }
    public Asn1Tag Tag { get; }
    public virtual IEnumerable<AsnNode> GetChildren() => Enumerable.Empty<AsnNode>();
    protected AsnReader Reader { get; }
    protected AsnWalkContext Context { get; }

    public virtual string? Display => null;
    public virtual string Name => Tag.ToString();
    public bool Synthetic => Context.Synthetic;

    public virtual List<(string Name, string? Value)> GetAdorningAttributes() {
        List<(string Name, string? Value)> attributes = [
            ("Length", Raw.Length.ToString())
        ];

        if (Context.BaseDocument.Span.Overlaps(Raw.Span, out int offset)) {
            attributes.Add(("Offset", offset.ToString()));
        }
        else {
            Debug.Fail("Node contents are not contained within the base document. This should not happen.");
        }

        return attributes;
    }

    public AsnNode(Asn1Tag tag, AsnWalkContext context, AsnReader reader) {
        Raw = reader.PeekEncodedValue();
        Contents = reader.PeekContentBytes();
        Tag = tag;
        Reader = reader;
        Context = context;
    }

    protected static AsnNode? SyntheticDecode(AsnWalker walker) {
        try {
            AsnNode[] children = walker.Walk().ToArray();

            return children switch {
                [AsnNode node] => node,
                _ => null,
            };
        }
        catch {
            return null;
        }
    }

    protected static AsnNode[] DecodeChildren(AsnWalker walker) {
        try {
            // We want to up-front validate all of the contents so that we can back-out
            // if it turns out we can't walk it.
            AsnNode[] children = walker.Walk().ToArray();

            // Don't auto decoded non-univeral content that has a content length of zero.
            // Otherwise we might be confusing UTF-16 that doesnt contain a high byte for something that is an
            // application-specific tag with no length.
            if (children.All(c => c.Contents.IsEmpty && c.Tag.TagClass != TagClass.Universal)) {
                return Array.Empty<AsnNode>();
            }

            return children;
        }
        catch {
            return Array.Empty<AsnNode>();
        }
    }
}
