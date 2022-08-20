
using System.IO;

namespace OpusMajor.FileSystem;

public class FileLockFactory
{
    public async Task<FileLock> Create(FileInfo path, TimeSpan timeOut)
    {
        var startTime = DateTime.Now;
        while (true)
        {
            try
            {
                var stream = File.Open(path.FullName, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                return new FileLock(path, stream);
            }
            catch (IOException)
            {
                if (DateTime.Now - startTime > timeOut) throw;
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
