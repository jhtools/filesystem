namespace OpusMajor.FileSystem;

public class FileLock : IDisposable
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
        _stream.Close();
        _stream.Dispose();
        _path.Delete();
    }
}