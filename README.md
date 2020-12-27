# Quest Discord Presence Client

An alternative client for Oculus Quest Beat Saber Discord RPC that works on the Quest 1 & 2, since due to issues with Oculus killing background
services, the [old client & app](https://github.com/madmagic007/Oculus-Quest-Presence) by [MadMagic](https://github.com/madmagic007) is no longer functional and can't really be fixed.

# Installation instructions

## Windows
1. Download the installer for the [latest release](https://github.com/Lauriethefish/Quest-Discord-Presence-Client/releases/latest).
2. Run the installer, and when you finish it, the config file will be opened.
3. Replace the area inside the quotes where it says ``REPLACE WITH QUEST IP`` with your Quest's IP address.
4. Save the config file. (CTRL+S)
5. Go to the start menu, type in Oculus Quest Presence Client and run the application. This won't open a window, as it runs in the background.

## Linux and MacOS
1. Download the correct ZIP file for your operating system from [here](https://github.com/Lauriethefish/Quest-Discord-Presence-Client/releases/latest).
2. Extract the ZIP somewhere on your computer.
3. Edit ``config.json``, and replace the area inside the quotes where it says ``REPLACE WITH QUEST IP`` with your Quest's IP address.
4. Save the config.
5. Run the ``Quest-Discord-Presence-Client`` to start querying the Quest.

# Notes
- If your Quest's IP address changes, you need to change it in the config file.
To do so, open the config file (On Windows, this is located at ``C:\Program Files\Quest Discord Presence Client\config.json``. On Linux and MacOS, it's ``extracted-location/config.json``), the change will be automatically picked up by the running app, this may take up to 20 seconds.
- You must have the [mod](https://github.com/Lauriethefish/Quest-Discord-Presence/releases/latest) installed on your Quest Beat Saber for this to work.
- Make sure that you have Discord on your PC, **not in the browser!**