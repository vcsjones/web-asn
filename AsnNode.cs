using System.Diagnostics;
using System.Formats.Asn1;
using System.Text;

namespace WebAsn;

public partial class AsnNode
{
    public ReadOnlyMemory<byte> Contents { get; }
    public ReadOnlyMemory<byte> Raw { get; }
    public Asn1Tag Tag { get; }
    public virtual IEnumerable<AsnNode> GetChildren() => Enumerable.Empty<AsnNode>();
    protected AsnReader Reader { get; }
    protected AsnWalkContext Context { get; }

    public virtual string? Display => null;
    public virtual string Name => Tag.ToString();
    public bool Synthetic => Context.Synthetic;

    public virtual List<(string Name, string Value)> GetAdorningAttributes()
    {
        List<(string Name, string Value)> attributes = new()
        {
            ("Length", Raw.Length.ToString())
        };

        if (Context.BaseDocument.Span.Overlaps(Raw.Span, out int offset))
        {
            attributes.Add(("Offset", offset.ToString()));
        }
        else
        {
            Debug.Fail("Node contents are not contained within the base document. This should not happen.");
        }

        return attributes;
    }

    public AsnNode(Asn1Tag tag, AsnWalkContext context, AsnReader reader)
    {
        Raw = reader.PeekEncodedValue();
        Contents = reader.PeekContentBytes();
        Tag = tag;
        Reader = reader;
        Context = context;
    }
}
