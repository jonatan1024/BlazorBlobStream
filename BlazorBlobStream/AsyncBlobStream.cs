using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorBlobStream
{
    internal class AsyncBlobStream : Stream, IAsyncDisposable
    {
        readonly IJSObjectReference _moduleInterop;
        readonly ElementReference _fileElement;
        readonly int _fileIndex;
        readonly long _size;
        long _position;

        Task? _getStreamTask;
        CancellationTokenSource? _getStreamCTS;
        IJSStreamReference? _jsStream;
        Stream? _innerStream;

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length => _size;

        public override long Position { get => _position; set => Seek(value, SeekOrigin.Begin); }

        public AsyncBlobStream(IJSObjectReference moduleInterop, ElementReference fileElement, int fileIndex, long size)
        {
            _moduleInterop = moduleInterop;
            _fileElement = fileElement;
            _fileIndex = fileIndex;
            _size = size;
            _position = 0;

            StartStreamTask();
        }

        void StartStreamTask()
        {
            _getStreamCTS?.Cancel(throwOnFirstException: false);
            _getStreamTask?.Dispose();
            _getStreamCTS?.Dispose();

            _getStreamCTS = new CancellationTokenSource();
            _getStreamTask = GetStreamAsync(_getStreamCTS.Token);
        }

        async Task GetStreamAsync(CancellationToken cancellationToken = default)
        {
            _innerStream?.Dispose();
            await (_jsStream?.DisposeAsync() ?? ValueTask.CompletedTask);

            _jsStream = await _moduleInterop.InvokeAsync<IJSStreamReference>("getFileSlice", cancellationToken, _fileElement, _fileIndex, _position);
            _innerStream = await _jsStream.OpenReadStreamAsync(maxAllowedSize: _size, cancellationToken);
        }

        public override void Flush() => throw new InvalidOperationException();

        public override int Read(byte[] buffer, int offset, int count) => throw new InvalidOperationException($"{nameof(ReadAsync)} must be used instead!");

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => await ReadAsync(buffer.AsMemory(offset, count), cancellationToken);

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            await _getStreamTask!;
            var readBytes = await _innerStream!.ReadAsync(buffer, cancellationToken);
            _position += readBytes;
            return readBytes;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long newPosition = origin switch
            {
                SeekOrigin.Current => _position + offset,
                SeekOrigin.End => _size + offset,
                _ => offset
            };

            if (newPosition != _position)
            {
                _position = newPosition;
                StartStreamTask();
            }

            return _position;
        }

        public override void SetLength(long value) => throw new InvalidOperationException();

        public override void Write(byte[] buffer, int offset, int count) => throw new InvalidOperationException();

        public override async ValueTask DisposeAsync()
        {
            _getStreamCTS?.Cancel(throwOnFirstException: false);
            _getStreamTask?.Dispose();
            _getStreamCTS?.Dispose();

            _getStreamTask = default;
            _getStreamCTS = default;

            _innerStream?.Dispose();
            await (_jsStream?.DisposeAsync() ?? ValueTask.CompletedTask);

            _innerStream = default;
            _jsStream = default;
        }
    }
}
