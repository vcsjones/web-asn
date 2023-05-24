using Microsoft.JSInterop;

namespace WebAsn;

internal class Exports {

    [JSInvokable]
    internal static int Add(int i, int j) {
        return i + j;
    }

}