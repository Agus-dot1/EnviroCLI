<p align="center">
  <img src="https://media3.giphy.com/media/v1.Y2lkPTc5MGI3NjExZ3Vmbm9vYjBtOGR6aTc1ZmhmejJ4d20wNm9vMTk4eXYxM2RhOXJyZyZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/TMwstqmuWCa6YHrvn6/giphy.gif" alt="EnviroCLI Demo" width="600" />
</p>

<h1 align="center">EnviroCLI</h1>

<p align="center">
  <strong>Launch your entire workflow with a single command.</strong>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/version-0.4.3-3498db" alt="version" />
  <img src="https://img.shields.io/badge/license-MIT-a1d01f" alt="license" />
  <img src="https://img.shields.io/badge/platform-Windows-0078d7" alt="platform" />
  <img src="https://img.shields.io/badge/.NET-8.0-512bd4" alt=".NET" />
</p>

---

EnviroCLI is a Windows tool to launch groups of applications at once. You can define environments (like "Development" or "Gaming") and start all associated apps in the order you want. It works from the command line and keeps everything in a local JSON file.

```
       ▶—————————————————————————▶——————————————————————————                   
        |                          |                           |
        |                          |                           |
┌─────────────────┐      ┌────────────────────┐      ┌──────────────────┐
│   You run       │      │ Select / Quick     │      │  Apps launch in  │
│   EnviroCLI     │      │  Launch Environment│      │  defined order   │
└─────────────────┘      └────────────────────┘      └──────────────────┘
```

## Features
* **Environment Management:** Create and edit named groups of apps.
* **App Discovery:** Automatically scans Windows folders and PATH for programs.
* **Ordered Launching:** Set specific sequences for your apps to start.
* **Quick Restart:** Reopen the last used environment instantly.
* **Interactive UI:** Simple terminal menus with arrow-key navigation.
* **No Dependencies:** Runs as a self-contained executable on Windows.

## Installation

### Using Scoop
```powershell
scoop bucket add enviroCLI https://github.com/Agus-dot1/scoop-enviroCLI
scoop install enviroCLI
```

### Manual
Download the latest executable from the [Releases](https://github.com/Agus-dot1/EnviroCLI/releases) page and run `EnviroCLI.exe`.

## Quick Start
1. Run `EnviroCLI`.
2. Select **Show Environments** -> **Add Environment**.
3. Name your environment and add apps by searching or entering a path.
4. Select **Initialize Environment** to launch your apps.

## Structure
```
EnviroCLI/
├── Program.cs             # Main entry and menu logic
├── Models/                # Data structures (Config, App, Environment)
├── Services/              # Logic for JSON, app management, and preferences
├── UI/                    # Console interface and formatting
├── Utils/                 # App discovery helpers
└── config/                # Local data (config.json)
```

## Build from Source
Requires .NET 8 SDK.
```powershell
git clone https://github.com/Agus-dot1/EnviroCLI.git
cd EnviroCLI
dotnet run
```

## License
MIT License.
