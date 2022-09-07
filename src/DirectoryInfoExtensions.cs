using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace JHTools.FileSystem;

public static class DirectoryInfoExtensions
{
    public static FileInfo GetFile(this DirectoryInfo d, params string[] names) =>
        new(Path.Combine(d.FullName, Path.Combine(names)));

    public static FileInfo GetExistingFile(this DirectoryInfo d, string name)
    {
        var f = d.GetFile(name);
        if (!f.Exists)
            throw new FileNotFoundException($"File {f.FullName} not found");
        return f;
    }

    public static DirectoryInfo GetDirectory(this DirectoryInfo d, params string[] names) =>
        new(Path.Combine(d.FullName, Path.Combine(names)));
    
    public static DirectoryInfo GetExistingDirectory(this DirectoryInfo d, string name)
    {
        var f = d.GetDirectory(name);
        if (!f.Exists)
            throw new DirectoryNotFoundException($"Directory {f.FullName} not found");
        return f;
    }
    
    public static void EnsureDeleted(this DirectoryInfo d)
    {
        if (!d.Exists) return;
        try
        {
            d.Delete(true);
        }
        catch (Exception)
        {
            if (!d.Exists) return;
            throw;
        }
    }

    public static void EnsureCreated(this DirectoryInfo d)
    {
        if (d.Exists) return;
        try
        {
            d.Create();
        }
        catch (Exception)
        {
            if (d.Exists) return;
            throw;
        }
    }
    
    public static Task EnsureCreatedAsync(this DirectoryInfo d)
    {
        if (d.Exists) return Task.CompletedTask;
        return Task.Run(() => d.EnsureCreated());
    }
    
    public static async Task EnsureDeletedAsync(this DirectoryInfo d)
    {
        await Task.Run(() => EnsureDeleted(d));
    }

    public static DirectoryInfo EnsureEmpty(this DirectoryInfo d)
    {
        if (d.Exists) d.EnsureDeleted();
        d.Create();
        return d;
    }

    public static async Task<DirectoryInfo> EnsureEmptyAsync(this DirectoryInfo d)
    {
        return await Task.Run(() => EnsureEmpty(d));
    }

    public static async Task<DirectoryInfo> CopyToAsync(this DirectoryInfo source, DirectoryInfo target, CopyOperation operation)
    {
        if (operation == CopyOperation.Force)
        {
            await target.EnsureEmptyAsync();
        }
        else
        {
            await target.EnsureCreatedAsync();
        }

        foreach (var file in source.EnumerateFiles())
        {
            var targetFile = new FileInfo(Path.Combine(target.FullName, file.Name));
            if (targetFile.Exists)
            {
                switch (operation)
                {
                    case CopyOperation.FailIfExists:
                        throw new Exception($"File already exists: {targetFile.FullName}");
                    case CopyOperation.SkipIfExists:
                        continue;
                }
            }
            await Task.Run(() => file.CopyTo(targetFile.FullName, true));
        }

        foreach (var directory in source.EnumerateDirectories())
        {
            await CopyToAsync(directory, target.GetDirectory(directory.Name), operation);
        }
        return target;
    }
    
    public static FileInfo CreateZipFile(
        this DirectoryInfo d,
        FileInfo destination,
        CompressionLevel compressionLevel = CompressionLevel.Optimal,
        bool includeBaseDirectory = false,
        Encoding? entryNameEncoding = null)
    {
        ZipFile.CreateFromDirectory(
            d.FullName,
            destination.FullName,
            compressionLevel,
            includeBaseDirectory,
            entryNameEncoding);
        return destination;
    }

    public static async Task<FileInfo> CreateZipFileAsync(
        this DirectoryInfo d,
        FileInfo destination,
        CompressionLevel compressionLevel = CompressionLevel.Optimal,
        bool includeBaseDirectory = false,
        Encoding? entryNameEncoding = null) =>
        await Task.Run(() =>
            CreateZipFile(d, destination, compressionLevel, includeBaseDirectory, entryNameEncoding));

    public static async Task<T?> ReadJsonAsync<T>(this DirectoryInfo d, string name) =>
        await d.GetFile(name).GetJsonAsync<T>();

    public static T? ReadJson<T>(this DirectoryInfo d, string name) =>
        d.GetFile(name).GetJson<T>();

    public static async Task WriteJsonAsync<T>(this DirectoryInfo d,
        string name,
        T data,
        JsonSerializerOptions? options = null) where T:class
    {
        var txt = JsonSerializer.Serialize(data, options);
        await d.GetFile(name).WriteAllTextAsync(txt);
    }

    public static void WriteJson<T>(this DirectoryInfo d,
        string name,
        T data,
        JsonSerializerOptions? options = null) where T:class
    {
        var txt = JsonSerializer.Serialize(data, options);
        d.GetFile(name).WriteAllText(txt);
    }

    public static async Task<DirectoryInfo> UnzipToAsync(this FileInfo zipFile,
        DirectoryInfo target,
        bool deleteContents=false)
    {
        
        if (deleteContents)
        {
            await target.EnsureEmptyAsync();
        }
        else
        {
            await target.EnsureCreatedAsync();
        }

        await Task.Run(() => ZipFile.ExtractToDirectory(zipFile.FullName, target.FullName));
        return target;
    }
    
    public static DirectoryInfo UnzipTo(this FileInfo zipFile,
        DirectoryInfo target,
        bool deleteContents=false)
    {
        if (deleteContents)
        {
            target.EnsureEmpty();
        }
        else
        {
            target.EnsureCreated();
        }

        ZipFile.ExtractToDirectory(zipFile.FullName, target.FullName);
        return target;
    }
    
    
}