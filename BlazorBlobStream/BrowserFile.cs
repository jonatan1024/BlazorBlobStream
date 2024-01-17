using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace BlazorBlobStream
{
    internal class BrowserFile : IBrowserFile
    {
        public string Name { get; set; } = default!;

        public DateTimeOffset LastModified { get; set; }

        public long Size { get; set; }

        public string ContentType { get; set; } = default!;

        public int Index { get; set; }

        public IJSObjectReference ModuleInterop { get; set; } = default!;
        public ElementReference FileElement { get; set; } = default!;

        public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
        {
            if (maxAllowedSize < Size)
                throw new InvalidOperationException($"Please increase {nameof(maxAllowedSize)} to at least {Size}");

            return new AsyncBlobStream(ModuleInterop, FileElement, Index, Size);
        }
    }
}
