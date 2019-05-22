using Microsoft.AspNetCore.Server.Kestrel.Core.Adapter.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Pages
{
    public class BadRequestDiagnosticAdapter : IConnectionAdapter
    {
        private readonly ILogger _logger;
        private readonly int _bufferSize;

        public BadRequestDiagnosticAdapter(ILogger logger, int bufferSize)
        {
            _logger = logger;
            _bufferSize = bufferSize;
        }

        public bool IsHttps => false;

        public Task<IAdaptedConnection> OnConnectionAsync(ConnectionAdapterContext context)
        {
            return Task.FromResult<IAdaptedConnection>(
                new BadRequestDiagnosticAdapted(context.ConnectionStream, _logger, _bufferSize));
        }

        private class BadRequestDiagnosticAdapted : IAdaptedConnection
        {
            private readonly BadRequestDiagnosticStream _diagnosticStream;

            public BadRequestDiagnosticAdapted(Stream inner, ILogger logger, int bufferSize)
            {
                _diagnosticStream = new BadRequestDiagnosticStream(inner, logger, bufferSize);
            }

            public Stream ConnectionStream => _diagnosticStream;

            public void Dispose()
            {
                _diagnosticStream.Dispose();
            }
        }

        private class BadRequestDiagnosticStream : Stream
        {
            private readonly Stream _inner;
            private readonly ILogger _logger;
            private readonly int _bufferSize;
            private readonly byte[] _buffer;
            private readonly object _bufferLock = new object();

            private bool _empty = true;
            private int _head;
            private int _tail;
            private int _searchOffset;
            private bool _disposed;

            public BadRequestDiagnosticStream(Stream inner, ILogger logger, int bufferSize)
            {
                _inner = inner;
                _logger = logger;
                _bufferSize = bufferSize;
                _buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
            }

            public override bool CanRead => _inner.CanRead;
            public override bool CanSeek => _inner.CanSeek;
            public override bool CanWrite => _inner.CanWrite;
            public override long Length => _inner.Length;

            public override long Position
            {
                get => _inner.Position;
                set => _inner.Position = value;
            }

            public override void Flush()
            {
                _inner.Flush();
            }

            public override Task FlushAsync(CancellationToken cancellationToken)
            {
                return _inner.FlushAsync(cancellationToken);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                var read = _inner.Read(buffer, offset, count);
                Copy(buffer, offset, read);
                return read;
            }

            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                var read = await _inner.ReadAsync(buffer, offset, count, cancellationToken);
                Copy(buffer, offset, read);
                return read;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return _inner.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                _inner.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                Test400(buffer, offset, count);
                _inner.Write(buffer, offset, count);
            }

            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                Test400(buffer, offset, count);
                return _inner.WriteAsync(buffer, offset, count, cancellationToken);
            }

            public override string ToString()
            {
                lock (_bufferLock)
                {
                    if (_empty)
                    {
                        return string.Empty;
                    }

                    var builder = new StringBuilder(_bufferSize * 4 + 14);

                    var head = _head;
                    builder.Append("[HEX] ");
                    do
                    {
                        builder.Append(_buffer[head].ToString("X2"));
                        builder.Append(" ");
                        head = (head + 1) % _bufferSize;
                    } while (head != _tail);

                    builder.AppendLine();

                    head = _head;
                    builder.Append("[RAW] ");
                    do
                    {
                        builder.Append((char)_buffer[head]);
                        head = (head + 1) % _bufferSize;
                    } while (head != _tail);

                    return builder.ToString();
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    _disposed = true;
                    ArrayPool<byte>.Shared.Return(_buffer);
                }

                base.Dispose(disposing);
            }

            // The below APM methods call the underlying Read methods which will still be buffered.
            public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                var task = ReadAsync(buffer, offset, count, default(CancellationToken), state);
                if (callback != null)
                {
                    task.ContinueWith((t, state2) => ((AsyncCallback)state2).Invoke(t), callback);
                }
                return task;
            }

            public override int EndRead(IAsyncResult asyncResult)
            {
                return ((Task<int>)asyncResult).GetAwaiter().GetResult();
            }

            private Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken, object state)
            {
                var tcs = new TaskCompletionSource<int>(state);
                var task = ReadAsync(buffer, offset, count, cancellationToken);
                task.ContinueWith((task2, state2) =>
                {
                    var tcs2 = (TaskCompletionSource<int>)state2;
                    if (task2.IsCanceled)
                    {
                        tcs2.SetCanceled();
                    }
                    else if (task2.IsFaulted)
                    {
                        tcs2.SetException(task2.Exception);
                    }
                    else
                    {
                        tcs2.SetResult(task2.Result);
                    }
                }, tcs, cancellationToken);
                return tcs.Task;
            }

            public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                var task = WriteAsync(buffer, offset, count, default(CancellationToken), state);
                if (callback != null)
                {
                    task.ContinueWith((t, state2) => ((AsyncCallback)state2).Invoke(t), callback);
                }
                return task;
            }

            public override void EndWrite(IAsyncResult asyncResult)
            {
                ((Task<object>)asyncResult).GetAwaiter().GetResult();
            }

            private Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken, object state)
            {
                var tcs = new TaskCompletionSource<object>(state);
                var task = WriteAsync(buffer, offset, count, cancellationToken);
                task.ContinueWith((task2, state2) =>
                {
                    var tcs2 = (TaskCompletionSource<object>)state2;
                    if (task2.IsCanceled)
                    {
                        tcs2.SetCanceled();
                    }
                    else if (task2.IsFaulted)
                    {
                        tcs2.SetException(task2.Exception);
                    }
                    else
                    {
                        tcs2.SetResult(null);
                    }
                }, tcs, cancellationToken);
                return tcs.Task;
            }

            private void Copy(byte[] buffer, int offset, int count)
            {
                if (count == 0)
                {
                    return;
                }

                lock (_bufferLock)
                {
                    int totalCopyCount, sourceOffset;

                    if (count < _bufferSize)
                    {
                        totalCopyCount = count;
                        sourceOffset = offset;
                    }
                    else
                    {
                        totalCopyCount = _bufferSize;
                        sourceOffset = offset + count - _bufferSize;
                    }

                    if (totalCopyCount <= _bufferSize - _tail)
                    {
                        Buffer.BlockCopy(buffer, sourceOffset, _buffer, _tail, totalCopyCount);
                        if (!_empty && _head == _tail)
                        {
                            _head = _tail + totalCopyCount;
                        }

                        _tail += totalCopyCount;
                    }
                    else
                    {
                        var firstCopyCount = _bufferSize - _tail;
                        var secondCopyCount = totalCopyCount - firstCopyCount;

                        Buffer.BlockCopy(buffer, sourceOffset, _buffer, _tail, firstCopyCount);
                        Buffer.BlockCopy(buffer, sourceOffset + firstCopyCount, _buffer, 0, secondCopyCount);

                        _head = _tail = secondCopyCount;
                    }

                    _empty = false;
                }
            }

            private void Test400(byte[] buffer, int offset, int count)
            {
                // O(n). Only works because the first character in searchString is not repeated.
                const string searchString = "HTTP/1.1 400 Bad Request\r\n";

                var head = offset;
                var tail = offset + count;

                // Start in the middle of the sarch string if that's where we left off in the last buffer.
                var searchIndex = _searchOffset;

                while (head < tail)
                {
                    while (searchIndex < searchString.Length && head < tail)
                    {
                        if (buffer[head] == searchString[searchIndex])
                        {
                            head++;
                            searchIndex++;
                        }
                        else if (searchIndex == 0)
                        {
                            head++;
                        }
                        else
                        {
                            searchIndex = 0;
                        }
                    }

                    if (searchIndex == searchString.Length)
                    {
                        lock (_bufferLock)
                        {
                            var bytesBuffered = _head == _tail ? _bufferSize : _tail - _head;

                            _logger.LogError(
                                "Observed 400 response. The last {bytesBuffered} bytes of request data were: {newLine}{buffer}",
                                bytesBuffered, Environment.NewLine, this);
                        }

                        searchIndex = 0;
                    }
                }

                _searchOffset = searchIndex;
            }
        }
    }

}

