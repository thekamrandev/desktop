# MultiControl

## 🔍 What is MultiControl?

MultiControl is an easy-to-use application that lets you control multiple laptops with just one mouse and keyboard! When you move your mouse on the admin computer, the cursor moves the same way on all connected laptops in real-time. Perfect for teachers, trainers, or anyone who needs to perform the same actions on multiple computers at once.

![MultiControl Concept](https://via.placeholder.com/800x400?text=MultiControl+Concept)

## ✨ Features

- **🖱️ One Mouse, Many Computers**: Control multiple laptops simultaneously
- **🔄 Two Control Modes**: 
  - **Simultaneous Mode**: Control all laptops at once
  - **Individual Mode**: Control just one laptop at a time
- **🔒 Independent Use**: Each laptop can still be used normally with its own mouse and keyboard
- **🔐 Secure Connection**: Safe and reliable network communication
- **👥 User-friendly**: Simple interface anyone can use

## 💻 System Requirements

### For Both Admin and Client Computers:
- **Operating System**: Windows 10 or Windows 11
- **Processor**: 1.6 GHz or faster
- **Memory**: 2 GB RAM minimum (4 GB recommended)
- **Disk Space**: 50 MB of free space
- **Network**: All computers must be on the same local network
- **Software**: .NET 6.0 Runtime ([Download here](https://dotnet.microsoft.com/download/dotnet/6.0))

## 📋 Before You Start

- Make sure all computers are connected to the same network
- Close any applications that might interfere with keyboard/mouse control
- Have administrator rights ready in case you need to run the applications as admin

## 📥 Installation Guide

### Step 1: Download the Applications

1. Download both applications:
   - **MultiControl.Server.zip** - For the admin computer
   - **MultiControl.Client.zip** - For each client laptop

### Step 2: Install on Admin Computer

1. Extract **MultiControl.Server.zip** to a folder of your choice
2. No installation needed - just run the extracted **MultiControl.Server.exe**

### Step 3: Install on Client Laptops

1. Extract **MultiControl.Client.zip** to a folder on each client laptop
2. No installation needed - just run the extracted **MultiControl.Client.exe**

## 🚀 Quick Start Guide

### 💻 Setting Up the Admin Computer (Server)

1. **Download and Install**:
   - Download the MultiControl.Server application
   - Install it by following the on-screen instructions

2. **Launch the Application**:
   - Find MultiControl.Server in your Start menu or desktop and open it
   - You'll see a simple window like this:

   ![Server Application](https://via.placeholder.com/600x400?text=Server+Application+Screenshot)

3. **Start the Server**:
   - Click the big "Start Server" button
   - The status will change to "Server Running"
   - Note the IP address and port shown (you'll need these for the client computers)

4. **Ready to Connect**:
   - Your admin computer is now ready to accept connections from client laptops
   - The client list will show connected computers

### 💻 Setting Up Client Laptops

1. **Download and Install**:
   - Download the MultiControl.Client application on each laptop you want to control
   - Install by following the on-screen instructions

2. **Launch the Application**:
   - Open MultiControl.Client from the Start menu or desktop
   - You'll see a connection window like this:

   ![Client Application](https://via.placeholder.com/600x400?text=Client+Application+Screenshot)

3. **Connect to Admin**:
   - Enter the admin computer's IP address (shown on the server application)
   - Enter the port number (default is 5000)
   - Type a name for this laptop (like "Laptop1" or "JohnComputer")
   - Click the "Connect" button

4. **Successful Connection**:
   - The status will change to "Connected"
   - The admin computer can now control this laptop

## 🎮 How to Use MultiControl

### On the Admin Computer:

1. **Choose Control Mode**:
   - **Simultaneous**: All connected laptops will mirror your mouse and keyboard
   - **Individual**: Select a specific laptop from the list to control just that one

2. **Start Controlling**:
   - Simply use your mouse and keyboard normally
   - Your actions will be mirrored on the client laptops
   - You can see all connected clients in the list

3. **Stop the Server**:
   - Click "Stop Server" when you're done
   - This will disconnect all clients

### On Client Laptops:

1. **Allow/Block Remote Control**:
   - Check "Allow Remote Control" to let the admin control this laptop
   - Uncheck it to temporarily block remote control

2. **Disconnect**:
   - Click "Disconnect" to stop the connection to the admin
   - You can reconnect anytime by clicking "Connect" again

## ❓ Common Problems & Solutions

### Can't Connect?

1. **Check Network Connection**:
   - Make sure all computers are connected to the same network (WiFi or LAN)
   - Try pinging the admin computer from the client to verify connectivity

2. **Firewall Issues**:
   - Windows Firewall might be blocking the connection
   - Solution: Allow MultiControl through your firewall
     1. Open Windows Security
     2. Go to "Firewall & network protection"
     3. Click "Allow an app through firewall"
     4. Find MultiControl and make sure it's checked for both Private and Public networks

3. **Wrong IP Address**:
   - Double-check the IP address you entered on the client
   - On the admin computer, you can find your IP by:
     1. Opening Command Prompt
     2. Typing `ipconfig` and pressing Enter
     3. Look for "IPv4 Address" under your active network connection

4. **Port Issues**:
   - Make sure you're using the correct port (default is 5000)
   - Try a different port if 5000 is being used by another application

### Mouse/Keyboard Not Working?

1. **Remote Control Not Allowed**:
   - Check that "Allow Remote Control" is enabled on the client

2. **Admin Rights**:
   - Some applications require admin privileges
   - Try running both server and client as administrator:
     1. Right-click the application
     2. Select "Run as administrator"

3. **Cursor Position**:
   - If screens have different resolutions, cursor position might not match exactly
   - Try moving the mouse slowly to get your bearings

## 📝 Frequently Asked Questions

### How many computers can I control at once?
You can control multiple computers simultaneously. The practical limit depends on your network speed and computer performance, but typically 5-10 computers work well.

### Does this work across different networks?
No, all computers must be on the same local network for security and performance reasons.

### Can I use this for gaming?
MultiControl is not designed for gaming as it may introduce slight delays in input. It's best for productivity applications, training, and demonstrations.

### Is my data secure?
Yes, MultiControl only transmits mouse and keyboard events over your local network. No data is sent to external servers.

### Can I customize keyboard shortcuts?
Not in the current version, but this feature may be added in future updates.

### What if a client computer disconnects?
The admin can continue controlling other connected clients. The disconnected client can reconnect at any time by clicking the "Connect" button again.

## 🔧 Technical Details

### How It Works
MultiControl uses a client-server architecture:
- **Server (Admin)**: Captures your mouse and keyboard actions and sends them to connected clients
- **Clients**: Receive these actions and simulate them locally on each computer

### For Developers
Built with:
- C# and .NET 6.0
- Windows Presentation Foundation (WPF) for the user interface
- TCP/IP for network communication

### Building from Source
1. Clone the repository
2. Open the solution in Visual Studio 2022
3. Restore NuGet packages
4. Build the solution
5. Run the server and client applications

## ⌨️ Keyboard Shortcuts Reference

For quick access to common functions:

### Server Application
- **F5**: Start/Stop Server
- **Ctrl+C**: Copy selected client information
- **Ctrl+A**: Select all clients
- **Esc**: Cancel current operation

### Client Application
- **F5**: Connect/Disconnect
- **Ctrl+A**: Toggle Allow Remote Control
- **Esc**: Cancel current operation

## 🔮 Future Updates

We're constantly working to improve MultiControl. Upcoming features may include:

- File transfer between computers
- Screen sharing capabilities
- Custom keyboard shortcut mapping
- Mobile device support
- Cross-platform compatibility (Mac, Linux)

Stay tuned for updates!

## 📄 License
This project is licensed under the MIT License - see the LICENSE file for details.