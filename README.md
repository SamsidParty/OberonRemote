<a id="readme-top"></a>

<br />
<div align="center">
  <a href="https://github.com/SamsidParty/OberonRemote">
    <img src="./Oberon/Assets/LogoWithShadow.png" alt="Oberon Remote Input Logo" width="160" height="160">
  </a>

  <h3 align="center">Oberon Remote Input</h3>
  <p align="center">
    Remote input for Xbox consoles, use any controller with your Xbox
    <br />
    <br />
  </p>
  <div align="center">

  <a href="">![Download Count](https://img.shields.io/github/downloads/SamsidParty/OberonRemote/total.svg?style=for-the-badge)</a>
  <a href="">![Stars Count](https://img.shields.io/github/stars/SamsidParty/OberonRemote.svg?style=for-the-badge)</a>
  <a href="">![Code Size](https://img.shields.io/github/languages/code-size/SamsidParty/OberonRemote?style=for-the-badge)</a>
  <a href="">![Repo Size](https://img.shields.io/github/repo-size/SamsidParty/OberonRemote?style=for-the-badge)</a>
  <a href="https://apps.microsoft.com/detail/9pk5stjzff3s?hl=en-US&gl=US">![Get It From Microsoft](https://get.microsoft.com/images/en-us%20dark.svg)</a>
    
  </div>
</div>

# How It Works ⚙️
To use Oberon, you need an extra device on the same local network as your Xbox console. This device is referred to as the "remote" or the "input server". 
It takes inputs from your controller, (eg. a Dualsense), and sends it to your Xbox with websockets.

![image](https://github.com/user-attachments/assets/2476ba6a-29ad-4626-8135-a43bef0b3dc8)


# Disclaimer ⚠️

Some apps and games may not work properly with Oberon Remote Input. Apps are able to detect and block synthetic inputs so there's no guarantee that everything will work smoothly in all situations.


# Setup Instructions ⚒️

## 1. Install On Your Console

  - Retail  mode: Install from the [Microsoft Store](https://apps.microsoft.com/detail/9pk5stjzff3s?cid=DevShareMCLPCB&hl=en-US&gl=QA)
  - Dev mode: Install the latest [MSIX release](https://github.com/SamsidParty/OberonRemote/releases/latest/download/Oberon.Msixbundle)

## 2. Install The Remote (Input Server)
  - Windows:
    - Download the latest [release](https://github.com/SamsidParty/OberonRemote/releases/latest/download/Oberon.Remote.Windows.zip) ([virustotal report](https://www.virustotal.com/gui/file/9edaf1d7a07505b25d781bee5dd3bd2fbddb5bb3cd0ca1e6e92154782535efc6))
    - Extract to a semi-permanent location (the app is portable, currently no installer)
    - You may have to install the .NET runtime, Visual C++ redistributables, and the webview2 runtime
    - Run `Oberon Remote.exe`, and allow it through your firewall
  - Android:
    - Download the latest [release](https://github.com/SamsidParty/OberonRemote/releases/latest/download/Oberon.Remote.Android.apk)
    - Install the APK file, you may have to enable sideloading
    - Open the app, if your controller isn't detected, try closing the app and reopening it after connecting the controller
  - Linux:
    - Download the latest [release](https://github.com/SamsidParty/OberonRemote/releases/latest/download/Oberon.Remote.Linux.zip)
    - Extract to a semi-permanent location (the app is portable, currently no installer)
    - Run `chmod +x "./Oberon Remote.x86_64"` in the folder where you extracted the file
    - Run `"./Oberon Remote.x86_64"` to execute the app
  - Mac:
    - You have to run the following command in your terminal, since this app isn't officially notarized by Apple:
      
      ```bash
      curl "https://raw.githubusercontent.com/SamsidParty/OberonRemote/refs/heads/main/Tooling/install_mac.sh" | sudo zsh
      ```
        
      If it is installed succesfully, then Oberon Remote will automatically launch. The app will be copied to your `Applications` folder, so that's where you can uninstall it from.

## 3. Pair The Remote With Your Xbox
- On the Xbox, open the Oberon app and press "Add Remote"  
![image](https://github.com/user-attachments/assets/f6a45b09-7aaa-44c4-92e4-42b55ff55e86)

- The IP address of the remote will be shown at the top of the app, you must input this on the console
![image](https://github.com/user-attachments/assets/d986e2f9-2c2c-40a3-8862-643df535f86f)

- The pairing will not work unless both devices are on **exactly** the same network, if it still doesn't work try disabling the firewall
  
- Once pairing completes, you will see your device's name on the console, press `A` while it is selected to connect to it
  ![image](https://github.com/user-attachments/assets/1e823e27-91eb-4cdd-b2a7-93582ff0b2b4)
  
- If the connection works, you will see a notification that the connection was successful
![image](https://github.com/user-attachments/assets/966f3295-23fe-48a2-89d8-6a6721ebe5e8)

## 4. Connect Your Controller
- Oberon Remote supports a wide range of different controller types
- You can connect using bluetooth or USB
- You may have to press a few buttons on your controller before Oberon Remote detects it

## 5. Preventing Interruptions
- If you are using the mobile remote, you must keep the app open in the foreground. Putting your device to sleep or switching apps will disconnect the remote.
- If you are using the remote on a computer, you must also keep the remote window in the foreground, minimizing it will increase the input latency and may cause malfunctions.

- If you want to turn off your Xbox controller when using the remote, do *NOT* hold the Xbox button and turn off controller, this will disconnect the remote
![image](https://github.com/user-attachments/assets/72c6c037-1e7f-4b9f-9464-b8e0bce5e420)

- Instead, just take the batteries out of your controller, this will prevent the remote from disconnecting.
If you have an elite controller without batteries, simply hold the Xbox button for 10 seconds, that will force shutdown the controller.
