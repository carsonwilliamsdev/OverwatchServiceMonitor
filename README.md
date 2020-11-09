# OverwatchServiceMonitor
Author: Carson Williams
Description: Windows service that automatically sets Overwatch.exe process priority to High.

This project uses WiX 4 for the MSI installer.
Solution created with VS2017. 
The service writes event logs to Windows Event Logs.
You may need to manually start the service the first time after installation. After that it should start automatically.
The service can be uninstalled from Control Panel.
