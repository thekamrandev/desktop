using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
// Removed System.Windows.Forms to avoid ambiguous references
using MultiControl.Common.Input;
using MultiControl.Common.Messages;
using MultiControl.Common.Network;

namespace MultiControl.Client
{
    public partial class MainWindow : Window
    {
        private TcpNetworkService? _networkService;
        private WindowsInputService? _inputService;
        private bool _isConnected;
        private bool _allowRemoteControl = true;
        private ControlMode _currentMode = ControlMode.Individual;
        private string _clientId = Guid.NewGuid().ToString();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _networkService = new TcpNetworkService();
            _networkService.MessageReceived += NetworkService_MessageReceived;
            _networkService.ClientDisconnected += NetworkService_ClientDisconnected;

            _inputService = new WindowsInputService();
            
            // Set client name to computer name by default
            txtClientName.Text = Environment.MachineName;
            
            LogMessage("Client started. Ready to connect to server.");
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_isConnected)
            {
                DisconnectFromServer();
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (_isConnected)
            {
                DisconnectFromServer();
            }
            else
            {
                ConnectToServer();
            }
        }

        private void chkAllowRemoteControl_CheckedChanged(object sender, RoutedEventArgs e)
        {
            _allowRemoteControl = chkAllowRemoteControl.IsChecked ?? false;
            LogMessage($"Remote control {(_allowRemoteControl ? "enabled" : "disabled")}.");
        }

        private void ConnectToServer()
        {
            try
            {
                if (!IPAddress.TryParse(txtServerIp.Text, out IPAddress serverIp))
                {
                    LogMessage("Invalid server IP address.");
                    return;
                }

                if (!int.TryParse(txtServerPort.Text, out int serverPort))
                {
                    LogMessage("Invalid server port.");
                    return;
                }

                if (_networkService != null)
                {
                    _networkService.StartClient(txtServerIp.Text, serverPort, txtClientName.Text);
                    _isConnected = true;
                    btnConnect.Content = "Disconnect";
                    txtStatus.Text = "Connected";
                    LogMessage($"Connected to server at {serverIp}:{serverPort}");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Failed to connect: {ex.Message}");
            }
        }

        private void DisconnectFromServer()
        {
            try
            {
                if (_networkService != null)
                {
                    _networkService.StopClient();
                    _isConnected = false;
                    btnConnect.Content = "Connect";
                    txtStatus.Text = "Disconnected";
                    LogMessage("Disconnected from server.");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error disconnecting: {ex.Message}");
            }
        }

        private void LogMessage(string message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                txtLog.Text += $"[{DateTime.Now:HH:mm:ss}] {message}\n";
            });
        }

        #region Network Event Handlers

        private void NetworkService_ClientDisconnected(object? sender, ClientConnectionEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (_isConnected)
                {
                    _isConnected = false;
                    btnConnect.Content = "Connect";
                    txtStatus.Text = "Disconnected";
                    LogMessage("Connection to server lost.");
                }
            });
        }

        private void NetworkService_MessageReceived(object? sender, MessageReceivedEventArgs e)
        {
            if (!_allowRemoteControl) return;
            if (_inputService == null) return;

            if (e.Message is MouseMoveMessage mouseMoveMessage)
            {
                HandleMouseMoveMessage(mouseMoveMessage);
            }
            else if (e.Message is MouseButtonMessage mouseButtonMessage)
            {
                HandleMouseButtonMessage(mouseButtonMessage);
            }
            else if (e.Message is MouseWheelMessage mouseWheelMessage)
            {
                HandleMouseWheelMessage(mouseWheelMessage);
            }
            else if (e.Message is KeyboardMessage keyboardMessage)
            {
                HandleKeyboardMessage(keyboardMessage);
            }
            else if (e.Message is ModeChangeMessage modeChangeMessage)
            {
                HandleModeChangeMessage(modeChangeMessage);
            }
        }

        #endregion

        #region Input Message Handlers

        private void HandleMouseMoveMessage(MouseMoveMessage message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (_inputService == null) return;
                
                // Convert normalized coordinates to screen coordinates
                int screenWidth = MultiControl.Common.Input.Screen.PrimaryScreen.Bounds.Width;
                int screenHeight = MultiControl.Common.Input.Screen.PrimaryScreen.Bounds.Height;

                int x = (int)(message.NormalizedX * screenWidth);
                int y = (int)(message.NormalizedY * screenHeight);

                _inputService.SimulateMouseMove(x, y);
            });
        }

        private void HandleMouseButtonMessage(MouseButtonMessage message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (_inputService == null) return;
                
                // Convert normalized coordinates to screen coordinates
                int screenWidth = MultiControl.Common.Input.Screen.PrimaryScreen.Bounds.Width;
                int screenHeight = MultiControl.Common.Input.Screen.PrimaryScreen.Bounds.Height;

                int x = (int)(message.NormalizedX * screenWidth);
                int y = (int)(message.NormalizedY * screenHeight);

                // Move to position first
                _inputService.SimulateMouseMove(x, y);
                
                if (message.IsPressed)
                {
                    _inputService.SimulateMouseButtonDown(message.Button);
                }
                else
                {
                    _inputService.SimulateMouseButtonUp(message.Button);
                }
            });
        }

        private void HandleMouseWheelMessage(MouseWheelMessage message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (_inputService == null) return;
                
                // Convert normalized coordinates to screen coordinates
                int screenWidth = MultiControl.Common.Input.Screen.PrimaryScreen.Bounds.Width;
                int screenHeight = MultiControl.Common.Input.Screen.PrimaryScreen.Bounds.Height;

                int x = (int)(message.NormalizedX * screenWidth);
                int y = (int)(message.NormalizedY * screenHeight);

                _inputService.SimulateMouseWheel(x, y, message.Delta);
            });
        }

        private void HandleKeyboardMessage(KeyboardMessage message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (_inputService == null) return;
                
                if (message.IsPressed)
                {
                    _inputService.SimulateKeyDown((MultiControl.Common.Input.Keys)message.VirtualKeyCode);
                }
                else
                {
                    _inputService.SimulateKeyUp((MultiControl.Common.Input.Keys)message.VirtualKeyCode);
                }
            });
        }

        private void HandleModeChangeMessage(ModeChangeMessage message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                _currentMode = message.Mode;
                txtControlMode.Text = _currentMode == ControlMode.Individual ? "Individual" : "Simultaneous";
                LogMessage($"Control mode changed to {txtControlMode.Text}");
            });
        }

        #endregion
    }
}