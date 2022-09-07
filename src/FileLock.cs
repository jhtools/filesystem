namespace JHTools.FileSystem;

public class FileLock : IDisposable, IAsyncDisposable
{
    private readonly FileInfo _path;
    private readonly Stream _stream;

    public FileLock(FileInfo path, Stream stream)
    {
        _path = path;
        _stream = stream;
    }

    public void Dispose()
    {
        _stream.Dispose();
        _path.Delete();
    }

    public async ValueTask DisposeAsync()
    {
        await _stream.DisposeAsync();
        await Task.Run(() => _path.Delete());
    }
}