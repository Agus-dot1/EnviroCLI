# EnviroCLI

🚀 **Launch Your Entire Workflow with a Single Command**

Tired of manually opening multiple applications every time you start working? **EnviroCLI** is a command-line tool that lets you define and launch entire environments effortlessly—perfect for developers, designers, and multitaskers.
<p align="center">
  <img src="https://media3.giphy.com/media/v1.Y2lkPTc5MGI3NjExZ3Vmbm9vYjBtOGR6aTc1ZmhmejJ4d20wNm9vMTk4eXYxM2RhOXJyZyZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/TMwstqmuWCa6YHrvn6/giphy.gif" alt="Enviro demo" />
</p>

---

## 📥 Installation

### ✅ Via Scoop (Recommended)
```powershell
scoop bucket add enviroCLI https://github.com/Agus-dot1/scoop-enviroCLI
scoop install enviroCLI
```

### 📦 Manual Installation
1. Download the latest release from [GitHub Releases](https://github.com/Agus-dot1/EnviroCLI/releases)
2. Extract it to your desired location
3. Run `EnviroCLI.exe`

### 🛠 Requirements
- Currently only on windows
- Performance Note: EnviroCLI has no app limit, but launching many resource-heavy programs at once may slow down low-end systems. This depends on your hardware, not EnviroCLI itself.

---

## 🚀 Getting Started

1. Open your terminal and run:
   ```powershell
   EnviroCLI
   ```
   The application will:
   - Create a `config` directory in your installation folder
   - Initialize an empty `config.json` file
   - Show the main menu ready for setup

2. **Create your first environment**:
   - Navigate to **"Show Environments"**
   - Select **"Add Environment"**
   - Name your environment (e.g., "Development", "Gaming")
   - Add applications by specifying their executable paths

3. **Launch your environment**:
   - Use **"Init Last Environment"** for quick access
   - Or select **"Show Environments"** to view and manage all environments

---

## 🌟 Features

✅ **Quick Environment Switching** – Define workspaces and launch them instantly.  
✅ **Persistent Settings** – Your configurations are saved between sessions.  
✅ **Interactive CLI** – Uses Spectre.Console for an intuitive user experience.  
✅ **Custom Launch Order** – Control the startup sequence of apps.  
✅ **Lightweight & Fast** – No background services, just run and go.  

---

## 🔧 Configuration

- All settings are stored in `config/config.json`
- The file is automatically created on first run
- Environments and preferences are preserved between sessions
- The `config` directory is excluded from version control for privacy

---

## 📂 Example Setups

### **Development Environment**
- 💻 Visual Studio Code
- 🖥️ Windows Terminal
- 🌐 Browser
- 🗄️ Database tools (MySQL, MongoDB)
- 🐳 Docker
- 🔗 API Client (Postman)

### **Gaming Environment**
- 🎮 Steam, Epic Games, Battle.net
- 🎵 Spotify
- 🎙 Discord

### **Work Environment**
- 📧 Email Client (Outlook, Thunderbird)
- 🌐 Browser
- 📊 Spreadsheet Software (Excel, LibreOffice Calc)
- 📅 Task Manager (Notion, Todoist, Trello)
- 💬 Communication Apps (Slack, Zoom, Teams)

---

## 🔍 Technical Details

### **Configuration**
- JSON-based using `System.Text.Json`
- Error handling with user feedback
- Persistent settings storage

### **Process Management**
- Uses `System.Diagnostics.Process` for launching applications
- Executable path validation
- Optimized process creation

### **User Interface**
- Fully interactive CLI built with `Spectre.Console`
- Organized tables and visual feedback
- Clean console management

---

## 🤝 Contributing

We welcome contributions to EnviroCLI! Here's how you can help:

### 📌 Development Setup
1. **Fork the repository**
2. **Clone your fork**
3. **Create a new branch** for your feature
4. **Make your changes**
5. **Submit a pull request**

### 📜 Code Guidelines
- Follow **C# coding conventions**
- Add **comments for complex logic**
- Update **documentation** for new features
- Include **tests** when possible
- Use **clear, descriptive names**
- Familiarize yourself with **Spectre.Console documentation**

### 🚀 Areas for Contribution
- 🆕 New features
- 🐞 Bug fixes
- 📖 Documentation improvements
- ⚡ Performance optimizations

---

## 📜 License

[MIT](LICENSE)

