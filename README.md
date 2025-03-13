# EnviroCLI

A powerful Windows command-line environment manager that helps you organize and launch multiple applications efficiently. Perfect for developers who work with multiple application setups and want to streamline their workflow.

## Features

- **Smart Environment Management**
  - Create and organize multiple application environments
  - Quick launch with "Init Last Environment"
  - Add descriptions for better organization
  - Persistent storage of settings

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

## Getting Started

1. Open your terminal and run:
```powershell
EnviroCLI
```
The application will automatically:
- Create a `config` directory in your installation folder
- Initialize an empty `config.json` file
- Show the main menu ready for setup

2. Create your first environment:
   - Go to "Show Environments"
   - Select "Add Environment"
   - Give it a descriptive name (e.g., "Development", "Design")
   - Add applications by providing their executable paths

3. Launch your environment:
   - Use "Init Last Environment" for quick access (appears after first use)
   - Or select "Show Environments" to view and manage all environments

### Configuration

EnviroCLI automatically manages your configuration:
- Settings are stored in `config/config.json`
- The file is created automatically on first run
- Your environments and preferences are preserved between sessions
- The config directory is excluded from version control for privacy

### Environment Management

Access all features through "Show Environments":
- **Configure Apps**: Add, remove, or edit applications
- **Set Launch Order**: Control the startup sequence
- **Add Descriptions**: Improve organization with clear labels


### Example Setups

**Development Environment**
- Visual Studio Code - Code editor
- Windows Terminal - Command line
- Browser
- Database tools - MySQL, MongoDB.
- Docker
- API Client (e.g., Postman)

**Design Workspace**
- Figma - UI/UX design
- Browser
- Image editor - Photoshop, GIMP, Affinity Photo.
- Font Manager

**Gaming Environment**
- Game Launcher - Steam, Epic Games, Battle.net.
- Spotify - Music
- Voice Chat - Discord, Teamspeak.

**Work Environment**
- Email Client - Outlook, Thunderbird.
- Browser
- Word Processor - Microsoft Word, LibreOffice Writer.
- SpreadSheet Software - Microsoft Excel, LibreOffice Calc.
- Task Manager - Notion, Todoist, Trello.
- Communication Apps - Slack, Zoom, Teams.


## Technical Details

- **Configuration**
  - JSON-based using System.Text.Json
  - Automatic config directory creation
  - Error handling with user feedback
  - Settings persistence between sessions

- **Process Management**
  - System.Diagnostics.Process for app launching
  - Executable path validation
  - Process creation optimizations

- **User Interface**
  - Interactive CLI using Spectre.Console
  - Organized tables and visual feedback
  - Clean console management

## Contributing

We welcome contributions to EnviroCLI! Here's how you can help:

### Development Setup
1. Fork the repository
2. Clone your fork
3. Create a new branch for your feature
4. Make your changes
5. Submit a pull request

### Code Guidelines
- Follow C# coding conventions
- Add comments for complex logic
- Update documentation for new features
- Include tests when possible
- Use clear, descriptive names
- Be sure you understand Spectre.Console documentation

### Areas for Contribution
- New features
- Bug fixes
- Documentation improvements
- Performance optimizations

For major changes, please open an issue first to discuss what you would like to change.

## License

[MIT](LICENSE)
