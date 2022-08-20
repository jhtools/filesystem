
Get-ChildItem -Path .\Out\*.nupkg | ForEach-Object {
    & dotnet nuget push $($_.FullName) --skip-duplicate --source github
}