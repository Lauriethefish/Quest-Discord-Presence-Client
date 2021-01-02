# Quest Discord Presence Client

An alternative client for Oculus Quest Beat Saber Discord RPC that works on the Quest 1 & 2, since due to issues with Oculus killing background
services, the [old client & app](https://github.com/madmagic007/Oculus-Quest-Presence) by [MadMagic](https://github.com/madmagic007) is no longer functional and can't really be fixed.

# Installation instructions

## Windows
1. Download the installer for the [latest release](https://github.com/Lauriethefish/Quest-Discord-Presence-Client/releases/latest).
2. Run the installer, accept the license, and it will open the app once you are finished.
3. Put your Quest's IP addresss (found in SideQuest) in the box and hit confirm.
4. Open Beat Saber and check that it says that fetching the presence was successful.
5. You can now close the app and it will run in the background, automatically starting/stopping presence whenever BS is open/closed.
6. If you need to change your Quest IP, open the app from the start menu.

## Linux and MacOS
1. Download the correct ZIP file for your operating system from [here](https://github.com/Lauriethefish/Quest-Discord-Presence-Client/releases/latest).
2. Extract the ZIP somewhere on your computer.
3. Run ``Quest-Discord-Presence-Client``. NOTE: If on Linux, you may need to mark the file as executable. To do this, run ``chmod +x Quest-Discord-Presence-Client`` in the directory where you extracted it.
4. Put your Quest's IP addresss (found in SideQuest) in the box and hit confirm.
5. Open Beat Saber and check that it says that fetching the presence was successful.
6. You can now close the app and it will run in the background, automatically starting/stopping presence whenever BS is open/closed.
7. If you need to change your Quest IP, open the app again.

# Notes
- If your Quest's IP address changes, you need to change it in the config file.
To do so, open the config file (On Windows, this is located at ``C:\Program Files\Quest Discord Presence Client\config.json``. On Linux and MacOS, it's ``extracted-location/config.json``), the change will be automatically picked up by the running app, this may take up to 20 seconds.
- You must have the [mod](https://github.com/Lauriethefish/Quest-Discord-Presence/releases/latest) installed on your Quest Beat Saber for this to work.
- Make sure that you have Discord on your PC, **not in the browser!**