<p align="center">
  <img src="https://media3.giphy.com/media/v1.Y2lkPTc5MGI3NjExZ3Vmbm9vYjBtOGR6aTc1ZmhmejJ4d20wNm9vMTk4eXYxM2RhOXJyZyZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/TMwstqmuWCa6YHrvn6/giphy.gif" alt="EnviroCLI Demo" width="600" />
</p>

<h1 align="center">EnviroCLI</h1>

<p align="center">
  <strong>Launch your entire workflow with a single command.</strong>
</p>

<p align="center">
  <a href="https://github.com/Agus-dot1/EnviroCLI/releases"><img alt="Version" src="https://img.shields.io/badge/version-0.4.3-blue?style=flat-square" /></a>
  <a href="https://github.com/Agus-dot1/EnviroCLI/blob/main/LICENSE"><img alt="License" src="https://img.shields.io/badge/license-MIT-green?style=flat-square" /></a>
  <img alt="Platform" src="https://img.shields.io/badge/platform-Windows-0078D6?style=flat-square&logo=windows" />
  <img alt=".NET" src="https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet" />
</p>

---

## What is EnviroCLI?

**EnviroCLI** is a Windows command-line tool that lets you define, manage, and launch groups of applications — called **environments** — in a single action. Instead of manually opening 5–10 programs every time you sit down to work, game, or study, you define them once and launch them all instantly.

It's built with C# / .NET 8, uses [Spectre.Console](https://spectreconsole.net/) for a rich interactive terminal UI, and stores everything locally in a simple JSON config file. No accounts, no cloud, no background services — just a fast, portable executable.

---

## Table of Contents

- [Features](#-features)
- [How It Works](#-how-it-works)
- [Installation](#-installation)
- [Getting Started](#-getting-started)
- [Configuration](#-configuration)
- [Example Setups](#-example-setups)
- [Architecture](#-architecture)
- [Building from Source](#-building-from-source)
- [Contributing](#-contributing)
- [License](#-license)

---

## ✨ Features

| Feature | Description |
|---|---|
| **Environment Management** | Create, edit, rename, and delete named groups of applications. |
| **Smart App Discovery** | Automatically scans `Program Files`, `AppData`, `PATH`, and other common Windows directories to find installed executables — no need to hunt for file paths manually. |
| **Custom Launch Order** | Assign a numeric launch order to each app so heavier programs start first (or last). |
| **Quick Re-launch** | The "Init Last Environment" shortcut remembers your most recently used environment for one-click access. |
| **Live Launch Feedback** | A real-time status table shows each app's launch state (`Starting…`, `Started`, `Error`) as it boots. |
| **Persistent Config** | All environments and preferences are saved to a local `config.json` and survive between sessions. |
| **Tutorial Mode** | A built-in welcome panel guides first-time users through the workflow. Can be toggled off in Preferences. |
| **Zen Mode** | A preference toggle for a minimal experience (stored in config). |
| **Interactive CLI** | Arrow-key menus, search-enabled selectors, and color-coded output via Spectre.Console — no memorizing flags. |
| **Portable & Lightweight** | Publishes as a single self-contained `.exe`. No installer, no runtime dependency, no background services. |

---

## 🔄 How It Works

```
┌─────────────────┐      ┌────────────────────┐      ┌──────────────────┐
│   You run        │ ──▶  │  Select / Quick     │ ──▶  │  Apps launch in   │
│   EnviroCLI      │      │  Launch Environment │      │  defined order     │
└─────────────────┘      └────────────────────┘      └──────────────────┘
```

1. **Define** an environment — give it a name like `Development` or `Gaming`.
2. **Add apps** — EnviroCLI searches your system for known executables or lets you enter a path manually.
3. **Set launch order** — decide which apps start first.
4. **Initialize** the environment — every app opens in sequence with real-time status feedback.

EnviroCLI remembers your last-used environment so the next launch is just one menu selection away.

---

## 📥 Installation

### Via Scoop (Recommended)

```powershell
scoop bucket add enviroCLI https://github.com/Agus-dot1/scoop-enviroCLI
scoop install enviroCLI
```

### Manual Installation

1. Download the latest release from [GitHub Releases](https://github.com/Agus-dot1/EnviroCLI/releases).
2. Extract it to your preferred location.
3. Run `EnviroCLI.exe`.

### Requirements

- **OS:** Windows 10 / 11 (x64)
- **Runtime:** None — the release is self-contained (.NET 8 bundled)
- **Note:** EnviroCLI imposes no app limit, but launching many resource-heavy programs simultaneously may slow low-end hardware.

---

## 🚀 Getting Started

### 1. Launch

```powershell
EnviroCLI
```

On first run the tool creates a `config/config.json` alongside the executable and shows a welcome tutorial.

### 2. Create Your First Environment

- Navigate to **Show Environments** → **Add Environment**.
- Give it a name (e.g., `Web Dev`).
- Add applications one by one:
  - Type part of the app name — EnviroCLI searches common install directories and presents matches.
  - Select a match from the list, or choose **manual** to paste a custom `.exe` path.
  - Assign a launch order (defaults to sequential).
- Select **Finish** when done.

### 3. Launch It

Return to the main menu:

- **Init Last Environment** — instantly re-launches your most recently used environment.
- **Show Environments → Initialize Environment** — pick any saved environment to launch.

A live table displays each app's startup status:

```
╭─────────────────┬──────────╮
│ App             │ Status   │
├─────────────────┼──────────┤
│ VS Code         │ Started  │
│ Windows Terminal│ Started  │
│ Chrome          │ Started  │
│ Docker Desktop  │ Starting…│
╰─────────────────┴──────────╯
```

### 4. Manage

From **Show Environments** you can also:

- **Edit Environment** — rename it, or manage its apps (add / edit / delete individual apps, change paths and launch order).
- **Delete Environment** — removes it from config after confirmation.

### 5. Preferences

| Preference | Default | Description |
|---|---|---|
| Tutorial | `true` | Shows the welcome guide panel on the main menu. |
| Zen Mode | `false` | Stored in config for a minimal experience. |

Toggle preferences from the main menu under **Preferences**.

---

## ⚙️ Configuration

All state lives in a single JSON file:

```
<install_dir>/config/config.json
```

### Schema

```json
{
  "Environment": [
    {
      "Name": "Development",
      "Apps": [
        {
          "Name": "VS Code",
          "Route": "C:\\Users\\you\\AppData\\Local\\Programs\\Microsoft VS Code\\Code.exe",
          "LaunchOrder": 1
        },
        {
          "Name": "Chrome",
          "Route": "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
          "LaunchOrder": 2
        }
      ]
    }
  ],
  "LastUsedEnvironment": "Development",
  "Tutorial": true,
  "ZenMode": false
}
```

| Field | Type | Description |
|---|---|---|
| `Environment` | `array` | List of environment objects. |
| `Environment[].Name` | `string` | Display name of the environment. |
| `Environment[].Apps` | `array` | List of app entries in this environment. |
| `Apps[].Name` | `string` | Friendly name for the app. |
| `Apps[].Route` | `string` | Absolute path to the `.exe` file. |
| `Apps[].LaunchOrder` | `int` | Numeric order in which the app is launched (ascending). |
| `LastUsedEnvironment` | `string?` | Name of the last initialized environment (used by quick-launch). |
| `Tutorial` | `bool` | Whether the welcome panel is shown on the main menu. |
| `ZenMode` | `bool` | Minimal experience toggle. |

> **Privacy:** The `config/config.json` file is `.gitignore`'d by default so your personal paths are never committed.

---

## 📂 Example Setups

### Development

| Order | App |
|---|---|
| 1 | Visual Studio Code |
| 2 | Windows Terminal |
| 3 | Chrome |
| 4 | Docker Desktop |
| 5 | Postman |
| 6 | MySQL Workbench |

### Gaming

| Order | App |
|---|---|
| 1 | Steam |
| 2 | Discord |
| 3 | Spotify |

### Work / Office

| Order | App |
|---|---|
| 1 | Outlook |
| 2 | Microsoft Teams |
| 3 | Excel |
| 4 | Notion |
| 5 | Browser |

---

## 🏗️ Architecture

```
EnviroCLI/
├── Program.cs                      # Entry point — main loop & menu routing
├── EnviroCLI.csproj                # .NET 8, self-contained, single-file publish
│
├── Models/
│   ├── Config.cs                   # Root config model (environments, prefs)
│   ├── Environment.cs              # Environment model (name + apps list)
│   └── App.cs                      # App model (name, route, launch order)
│
├── Services/
│   ├── ConfigurationService.cs     # JSON load/save with System.Text.Json
│   ├── EnvironmentService.cs       # CRUD + initialization for environments
│   ├── AppService.cs               # CRUD for apps within an environment
│   └── PreferenceService.cs        # Tutorial & Zen Mode toggling
│
├── UI/
│   └── ConsoleUI.cs                # Figlet title, welcome panel, main menu
│
├── Utils/
│   └── FindAppHelper.cs            # Smart app discovery across Windows dirs
│
├── config/
│   └── config.json                 # User data (gitignored)
│
└── assets/
    └── icon.ico                    # Application icon
```

### Key Design Decisions

- **Spectre.Console** — Provides rich TUI elements (selection prompts with search, live-updating tables, figlet text, colored markup) without requiring a full GUI framework.
- **Static service classes** — Keeps the codebase simple and avoids DI overhead for a single-user CLI tool.
- **`System.Text.Json`** — Native .NET serialization with no extra dependencies.
- **`System.Diagnostics.Process`** — Launches apps with `UseShellExecute = false` and `CreateNoWindow = true` to prevent shell bloat and extra console windows.
- **Self-contained single-file publish** — The release is one `.exe` with .NET bundled, requiring zero pre-installed runtimes.

### Smart App Discovery

When adding an app, `FindAppHelper` scans:

1. `Program Files` and `Program Files (x86)`
2. `%LocalAppData%` and `%AppData%`
3. Known app-specific directories (VS Code, Chrome, Firefox, Edge, Git, Docker, Discord, Slack, Steam, Epic Games)
4. Every directory in `%PATH%`

It filters out system utilities (`Microsoft.*`, `Windows*`, `cmd`) and temporary files, then presents matches sorted alphabetically for quick selection.

---

## 🔨 Building from Source

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Build & Run

```powershell
# Clone the repo
git clone https://github.com/Agus-dot1/EnviroCLI.git
cd EnviroCLI

# Run in development
dotnet run

# Publish a self-contained single-file release
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishReadyToRun=true
```

The output binary will be in `bin/Release/net8.0-windows/win-x64/publish/`.

---

## 🤝 Contributing

Contributions are welcome! Here's how to get started:

1. **Fork** the repository.
2. **Clone** your fork and create a feature branch.
3. **Make changes** following the conventions below.
4. **Submit a pull request** with a clear description.

### Code Conventions

- Follow standard **C# naming and formatting** conventions.
- Add **XML comments** for public methods.
- Keep services **static** to match the existing architecture.
- Use **Spectre.Console** for all terminal output (no raw `Console.WriteLine`).
- Update the **README** if adding user-facing features.
- Use **`Spectre.Console.Color.Blue`** as the primary accent color to stay consistent with the existing UI theme.

### Areas for Contribution

- 🆕 New features (e.g., delayed launch, app arguments, URL launching)
- 🐞 Bug fixes
- 📖 Documentation improvements
- ⚡ Performance optimizations
- 🐧 Cross-platform support

---

## 📜 License

This project is licensed under the [MIT License](LICENSE).

---

<p align="center">
  Made with 💙 by <a href="https://github.com/Agus-dot1">Agus</a>
</p>
