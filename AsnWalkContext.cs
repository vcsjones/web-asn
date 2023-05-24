namespace WebAsn;

public record class AsnWalkContext(ReadOnlyMemory<byte> BaseDocument, bool Synthetic = false);
