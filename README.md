# BlazorBlobStream
A seekable File stream for Blazor.

[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/BlazorBlobStream)](https://www.nuget.org/packages/BlazorBlobStream)
[![GitHub license](https://img.shields.io/github/license/jonatan1024/BlazorBlobStream)](https://github.com/jonatan1024/BlazorBlobStream/blob/master/LICENSE.md)

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

0. Install this NuGet package:
```
dotnet add package BlazorBlobStream
```

1. Register the `BlobStreamService` in `Program.cs`:
```csharp
using BlazorBlobStream;
...
builder.Services.AddTransient<BlobStreamService>();
```

2. Inject and call the `BlobStreamService` in razor page:
```razor
@inject BlazorBlobStream.BlobStreamService BlobStreamService

<InputFile @ref="inputFile" OnChange="OnFileChange"/>

@code {
    InputFile inputFile = default!;

    async Task OnFileChange(InputFileChangeEventArgs fileChange)
    {
        var browserFiles = await BlobStreamService.GetBrowserFilesAsync(inputFile);
        var browserFile = browserFiles.First();
        await using var fileStream = browserFile.OpenReadStream(maxAllowedSize: browserFile.Size);
    }
}
```

3. Seek freely!
