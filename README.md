# BlazorBlobStream
A seekable File stream for Blazor.

This is an alternative implementation of `Microsoft.AspNetCore.Components.Forms.IBrowserFile`.
The stream returned by `OpenReadStream` now supports setting the `Position` property and calling the `Seek` method.

## Why
The MS provided implementation of `IBrowserFile` returns a stream that [doesn't allow seeking](https://github.com/dotnet/aspnetcore/issues/38785).
If you need to seek in this stream, you are left with the only option of copying the whole stream into the memory.

## How
All browsers support seeking in Files by [slicing the Blob](https://developer.mozilla.org/en-US/docs/Web/API/Blob/slice).
This implementation uses blob-slicing to support seeking in Blazor.

## Usage
To get seekable file streams, you just have to:

1. Register the `BlobFileService` in `Program.cs`:
```csharp
using BlazorBlobStream;
...
builder.Services.AddTransient<BlobFileService>();
```

2. Inject and call the `BlobFileService` in razor page:
```razor
@inject BlazorBlobStream.BlobFileService BlobFileService

<InputFile @ref="inputFile" OnChange="OnFileChange"/>

@code {
    InputFile inputFile = default!;

    async Task OnFileChange(InputFileChangeEventArgs fileChange)
    {
        var blobFiles = await BlobFileService.GetBlobFiles(inputFile);
        var blobFile = blobFiles.First();
        using var fileStream = blobFile.OpenReadStream(maxAllowedSize: blobFile.Size);
    }
}
```

3. Seek freely!