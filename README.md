# FanControl.THV2Ultra

A [FanControl](https://github.com/Rem0o/FanControl.Releases) plugin that enables liquid temperature monitoring for the Thermaltake THV2 Ultra AIO cooler.

## Features

- Real-time liquid temperature monitoring
<img width="378" height="31" alt="image" width="100%" src="https://github.com/user-attachments/assets/3594a989-5a9e-4627-a085-934bf8233695" />
<br>
<img width="234" height="207" alt="image" src="https://github.com/user-attachments/assets/03a5bfd8-b5ac-45a8-8f6c-717b5e80e3ae" />

## Installation

1. Download the latest release from the [Releases](https://github.com/Semyntax/FanControl.THV2Ultra/releases) page
2. Extract the DLL files to your FanControl plugins directory:
   ```
   C:\Program Files (x86)\FanControl\Plugins\THV2Ultra\
   ```
   Or use the built-in "Install plugin..." button in FanControl settings
3. Restart FanControl
4. The liquid temperature sensor will appear automatically if the THV2 Ultra is detected

## Requirements

- Windows (x64)
- .NET 8.0 Runtime
- [FanControl](https://github.com/Rem0o/FanControl.Releases) application
- Thermaltake THV2 Ultra AIO cooler

**Quick check via PowerShell for THV2 Ultra Device**
```powershell
Get-PnpDevice | Where-Object { $_.InstanceId -like "*VID_264A*PID_233C*" }
```

If you don't see the USB/HID devices, ensure the AIO is properly connected via USB.

## Building from Source

```bash
git clone https://github.com/Semyntax/FanControl.THV2Ultra.git
cd FanControl.THV2Ultra
dotnet build -c Release
```

The compiled plugin will be in `Source/bin/Release/net8.0-windows/win-x64/`

### Build Notes

- The build script automatically downloads `FanControl.Plugins.dll` from the official FanControl repository on first build
- Only plugin DLL is included in the output
- The project targets .NET 8.0 with Windows-specific features

### Running Tests

```bash
dotnet test
```

## License

This project is licensed under the [MIT No Attribution](LICENSE) license.

## Acknowledgments

- [FanControl](https://github.com/Rem0o/FanControl.Releases) by Rem0o
- [HidSharp](https://www.zer7.com/software/hidsharp) for HID device communication
