# EnviroCLI

A powerful Windows command-line environment manager that helps you organize and launch multiple applications efficiently.

## Features

- **Environment Management**: Create, edit, and delete environment configurations
- **Quick Launch**: Initialize your last used environment with a single command
- **App Configuration**: 
  - Control launch order with visual feedback
  - Configure application settings
  - Add descriptions for better organization
- **Enhanced CLI**: Beautiful console interface using Spectre.Console

## Requirements

- Windows OS
- .NET runtime is not required (self-contained executable)

## Installation

### Via Scoop (Recommended)
```powershell
scoop bucket add enviroCLI https://github.com/yourusername/scoop-enviroCLI
scoop install enviroCLI
```

### Manual Installation
1. Download the latest release
2. Extract to your desired location
3. Run `EnviroCLI.exe`

## Usage

### Quick Start

1. Launch EnviroCLI
2. Select "Add Environment" to create your first environment
3. Add applications with their paths and configurations
4. Use "Init Last Environment" to quickly launch your setup

### Managing Environments

Access all management features through the "Show Environments" menu:
- Add/Edit environment descriptions
- Modify app configurations
- Control launch order
- Delete environments

## Experimental Features

### Window Positioning
EnviroCLI includes experimental support for automatic window positioning:
- Set custom window positions for each application
- Special handling for Electron-based apps (VS Code, Edge, Notion)
- Uses Win32 API for reliable window management

Note: This feature is still in development and may not work consistently across all applications.

## Contributing

Pull requests are welcome! For major changes, please open an issue first to discuss what you would like to change.

## License

[MIT](LICENSE)
