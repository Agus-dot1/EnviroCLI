# Publishing EnviroCLI

This guide explains how to publish EnviroCLI as a self-contained executable and make it available through Scoop.

## Creating a Release

1. Build the self-contained release:
```powershell
dotnet publish -c Release -r win-x64 --self-contained true
```

2. Create a zip file containing:
   - EnviroCLI.exe
   - config/ (empty directory for persistence)

3. Calculate SHA256 hash of the zip:
```powershell
Get-FileHash EnviroCLI-win-x64.zip -Algorithm SHA256
```

4. Update the hash in `scoop/enviroCLI.json`

## Publishing to Scoop

1. Create a new GitHub repository for your Scoop bucket:
```
scoop-enviroCLI
└── bucket
    └── enviroCLI.json
```

2. Copy `scoop/enviroCLI.json` to your bucket repository

3. Users can then install EnviroCLI using:
```powershell
scoop bucket add enviroCLI https://github.com/yourusername/scoop-enviroCLI
scoop install enviroCLI
```

## Updating the Package

1. Update version in `EnviroCLI.csproj`
2. Create a new release on GitHub
3. Update version and hash in `enviroCLI.json`
4. Push changes to your Scoop bucket repository
