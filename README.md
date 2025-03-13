# EnviroCLI

A powerful Windows command-line environment manager that helps you organize and launch multiple applications efficiently. Perfect for developers who work with multiple application setups and want to streamline their workflow.

## Features

### Core Functionality
- **Smart Environment Management**
  - Create, edit, and delete environment configurations
  - Add detailed descriptions for better organization
  - Persistent storage of last used environment
  - JSON-based configuration for easy portability

- **Streamlined Workflow**
  - Quick launch with "Init Last Environment" feature
  - Dedicated app management interface
  - Intuitive main menu focused on fast environment launching
  - Clear navigation with organized back options

- **Application Control**
  - Configurable launch order with visual feedback
  - Comprehensive app configuration options
  - Executable path validation
  - Process-level control using System.Diagnostics

## Requirements

- Windows OS

## Installation

### Via Scoop (Recommended)
```powershell
scoop bucket add enviroCLI https://github.com/Agus-dot1/scoop-enviroCLI
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
4. Use "Init Last Environment" for quick access to your setup

### Environment Management

Access all management features through the "Show Environments" menu:
- **Environment Configuration**
  - Add/Edit environment descriptions
  - Configure application settings
  - Set launch order preferences
  - Delete environments

- **Application Settings**
  - Specify executable paths
  - Configure window positions
  - Set launch parameters
  - Define startup order


## Technical Details

- **Configuration Storage**: JSON-based using System.Text.Json
- **Process Management**: System.Diagnostics.Process for reliable app launching
- **UI Framework**: Spectre.Console for enhanced CLI visuals
- **Error Handling**: Comprehensive try-catch blocks with user feedback


## Contributing

Pull requests are welcome! For major changes, please open an issue first to discuss what you would like to change.

## License

[MIT](LICENSE)
