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

        public Lazy<Task<IJSObjectReference>> LazyFileTask { get; set; } = default!;

        public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
        {
            if (maxAllowedSize < Size)
                throw new InvalidOperationException($"Please increase {nameof(maxAllowedSize)} to at least {Size}");

            return new AsyncBlobStream(LazyFileTask, Size);
        }
    }
}
