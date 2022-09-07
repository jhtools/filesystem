# LibFileSystem

A library of extensions to DirectoryInfo and FileInfo

## Installation

```powershell
nuget install jhtools.filesystem
```

## Usage

```csharp
using JHTools.FileSystem;

var d = new DirectoryInfo(@"C:\Temp");

var fileInfo = d.GetFile("test.txt"); // returns a FileInfo object for the file
var directoryInfo = d.GetDirectory("test"); // returns a DirectoryInfo object for the directory

fileInfo = d.GetExistingFile("test.txt"); // returns a FileInfo object for the file, or throws an exception if the file does not exist
directoryInfo = d.GetExistingDirectory("test"); // returns a DirectoryInfo object for the directory, or throws an exception if the directory does not exist

d.EnsureDeleted(); // deletes the directory if it exists
d.EnsureCreated(); // creates the directory if it does not exist
d.EnsureEmpty(); // makes sure the directory exists and is empty
await d.CopyToAsync(@"C:\Temp2", CopyOperation.?); // copies the directory to the specified location
d.CreateZipFile(@"C:\Temp2\test.zip"); // creates a zip file of the directory
d.ReadJsonFile<TestObject>("test.json"); // reads a json file from the directory
d.WriteJsonFile("test.json", new TestObject()); // writes a json file to the directory
d.UnzipTo(@"C:\Temp2"); // unzips a zip file to the directory

```