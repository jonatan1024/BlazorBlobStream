using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace BlazorBlobStream
{
    public class BlobStreamService : IAsyncDisposable
    {
        CancellationTokenSource _interopCTS;
        Task<IJSObjectReference> _interopTask;

        public BlobStreamService(IJSRuntime jsRuntime)
        {
            _interopCTS = new CancellationTokenSource();
            _interopTask = jsRuntime.InvokeAsync<IJSObjectReference>("import", _interopCTS.Token, "/_content/BlazorBlobStream/jsInterop.js").AsTask();
        }

        public ValueTask<IReadOnlyList<IBrowserFile>> GetBrowserFilesAsync(InputFile inputFile, CancellationToken cancellationToken = default)
            => GetBrowserFilesAsync(inputFile.Element!.Value, cancellationToken: cancellationToken);

        public async ValueTask<IReadOnlyList<IBrowserFile>> GetBrowserFilesAsync(ElementReference inputFileElement, CancellationToken cancellationToken = default)
        {
            var interop = await _interopTask;

            var blobFiles = await interop.InvokeAsync<IReadOnlyList<BrowserFile>>("getFileInfo", cancellationToken, inputFileElement);
            foreach (var blobFile in blobFiles)
            {
                blobFile.ModuleInterop = interop;
                blobFile.FileElement = inputFileElement;
            }
            return blobFiles;
        }

        public async ValueTask DisposeAsync()
        {
            _interopCTS.Cancel(throwOnFirstException: false);

            if (_interopTask.Status == TaskStatus.RanToCompletion)
                await _interopTask.Result.DisposeAsync();
            _interopTask.Dispose();

            _interopCTS.Dispose();
        }
    }
}
