﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace BlazorBlobStream
{
    public class BlobStreamService : IAsyncDisposable
    {
        Task<IJSObjectReference> _interopTask;
        public BlobStreamService(IJSRuntime jsRuntime)
        {
            _interopTask = jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/BlazorBlobStream/jsInterop.js").AsTask();
        }

        public ValueTask<IReadOnlyList<IBrowserFile>> GetBrowserFilesAsync(InputFile inputFile)
            => GetBrowserFilesAsync(inputFile.Element!.Value);

        public async ValueTask<IReadOnlyList<IBrowserFile>> GetBrowserFilesAsync(ElementReference inputFileElement)
        {
            var interop = await _interopTask;
            var blobFiles = await interop.InvokeAsync<IReadOnlyList<BrowserFile>>("getFileInfo", inputFileElement);
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
