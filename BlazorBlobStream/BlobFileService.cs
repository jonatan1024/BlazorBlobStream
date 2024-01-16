using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace BlazorBlobStream
{
    public class BlobFileService : IAsyncDisposable
    {
        Task<IJSObjectReference> _interopTask;
        public BlobFileService(IJSRuntime jsRuntime)
        {
            _interopTask = jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/BlazorBlobStream/jsInterop.js").AsTask();
        }

        public ValueTask<IReadOnlyList<IBrowserFile>> GetBlobFiles(InputFile inputFile)
            => GetBlobFiles(inputFile.Element!.Value);

        public async ValueTask<IReadOnlyList<IBrowserFile>> GetBlobFiles(ElementReference inputFileElement)
        {
            var interop = await _interopTask;
            var blobFiles = await interop.InvokeAsync<IReadOnlyList<BlobFile>>("getFileInfo", inputFileElement);
            foreach (var blobFile in blobFiles)
            {
                blobFile.ModuleInterop = interop;
                blobFile.FileElement = inputFileElement;
            }
            return blobFiles;
        }

        public async ValueTask DisposeAsync()
        {
            if (_interopTask.Status == TaskStatus.RanToCompletion)
                await _interopTask.Result.DisposeAsync();

            _interopTask.Dispose();
        }
    }
}
