using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using MultiControl.Common.Input;
using MultiControl.Common.Messages;
using MultiControl.Common.Network;

namespace MultiControl.Server
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private TcpNetworkService? _networkService;
        private WindowsInputService? _inputService;
        private bool _isServerRunning;
        private ControlMode _currentMode = ControlMode.Individual;
        private ObservableCollection<ClientViewModel> _clients;
        private ClientViewModel? _selectedClient;

        public event PropertyChangedEventHandler? PropertyChanged;
        
        private void UpdateStatusBar(string message)
        {
            if (txtStatus != null)
            {
                txtStatus.Text = message;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _clients = new ObservableCollection<ClientViewModel>();
            lvConnectedClients.ItemsSource = _clients;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _networkService = new TcpNetworkService();
            _networkService.MessageReceived += NetworkService_MessageReceived;
            _networkService.ClientConnected += NetworkService_ClientConnected;
            _networkService.ClientDisconnected += NetworkService_ClientDisconnected;

            _inputService = new WindowsInputService();
            _inputService.MouseMove += InputService_MouseMove;
            _inputService.MouseButton += InputService_MouseButton;
            _inputService.MouseWheel += InputService_MouseWheel;
            _inputService.KeyboardKey += InputService_KeyboardKey;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_isServerRunning)
            {
                StopServer();
            }
        }

        private void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            if (_isServerRunning)
            {
                StopServer();
            }
            else
            {
                StartServer();
            }
        }

        private void cmbControlMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbControlMode.SelectedIndex == 0)
            {
                _currentMode = ControlMode.Individual;
            }
            else
            {
                _currentMode = ControlMode.Simultaneous;
            }

            // Notify clients about mode change
            if (_isServerRunning)
            {
                var modeMessage = new ModeChangeMessage
                {
                    Mode = _currentMode
                };
                _networkService.BroadcastMessage(modeMessage);
            }
        }

        private void lvConnectedClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedClient = lvConnectedClients.SelectedItem as ClientViewModel;
            UpdateClientDetails();
        }

        private void btnConnectClient_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedClient != null)
            {
                _selectedClient.Status = "Connected";
                UpdateClientDetails();
            }
        }

        private void btnDisconnectClient_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedClient != null)
            {
                _networkService.DisconnectClient(_selectedClient.Id);
            }
        }

        private void StartServer()
        {
            try
            {
                if (_networkService != null && _inputService != null)
                {
                    _networkService.StartServer(IPAddress.Any, 5000);
                    _inputService.StartCapture();
                    _isServerRunning = true;
                    btnStartStop.Content = "Stop Server";
                    txtStatus.Text = "Server running on port 5000";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start server: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StopServer()
        {
            if (_networkService != null && _inputService != null)
            {
                _networkService.StopServer();
                _inputService.StopCapture();
                _isServerRunning = false;
                btnStartStop.Content = "Start Server";
                txtStatus.Text = "Server not running";
                _clients.Clear();
            }
        }

        private void UpdateClientDetails()
        {
            if (_selectedClient != null)
            {
                txtClientName.Text = _selectedClient.Name;
                txtClientIp.Text = _selectedClient.IpAddress;
                txtClientScreenSize.Text = _selectedClient.ScreenSize;
                txtClientStatus.Text = _selectedClient.Status;
                btnConnectClient.IsEnabled = _selectedClient.Status != "Connected";
                btnDisconnectClient.IsEnabled = _selectedClient.Status == "Connected";
            }
            else
            {
                txtClientName.Text = string.Empty;
                txtClientIp.Text = string.Empty;
                txtClientScreenSize.Text = string.Empty;
                txtClientStatus.Text = string.Empty;
                btnConnectClient.IsEnabled = false;
                btnDisconnectClient.IsEnabled = false;
            }
        }

        #region Network Event Handlers

        private void NetworkService_ClientConnected(object? sender, ClientConnectionEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                // Add client to the list
                var client = new ClientViewModel
                {
                    Id = e.ClientId,
                    IpAddress = e.EndPoint.ToString(),
                    Status = "Connected",
                    LastActivity = DateTime.Now
                };
                _clients.Add(client);
                UpdateStatusBar($"Client connected from {e.EndPoint}");
            });
        }

        private void NetworkService_ClientDisconnected(object? sender, ClientConnectionEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                // Remove client from the list
                var client = _clients.FirstOrDefault(c => c.Id == e.ClientId);
                if (client != null)
                {
                    _clients.Remove(client);
                    UpdateStatusBar($"Client {client.Name} disconnected");
                    if (_selectedClient?.Id == e.ClientId)
                    {
                        _selectedClient = null;
                        UpdateClientDetails();
                    }
                }
            });
        }

        private void NetworkService_MessageReceived(object? sender, MessageReceivedEventArgs e)
        {
            if (e.Message is ConnectionHandshakeMessage handshakeMessage)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    var client = _clients.FirstOrDefault(c => c.Id == e.ClientId);
                    if (client != null)
                    {
                        client.Name = handshakeMessage.ClientName;
                        client.ScreenSize = $"{handshakeMessage.ScreenWidth}x{handshakeMessage.ScreenHeight}";
                        
                        if (_selectedClient?.Id == e.ClientId)
                        {
                            UpdateClientDetails();
                        }
                    }
                });

                // Send current mode to the new client
                var modeMessage = new ModeChangeMessage
                {
                    Mode = _currentMode
                };
                _networkService.SendMessage(e.ClientId, modeMessage);
            }
        }

        #endregion

        #region Input Event Handlers

        private void InputService_MouseMove(object sender, MouseMoveEventArgs e)
        {
            if (!_isServerRunning) return;

            var message = new MouseMoveMessage
            {
                NormalizedX = e.NormalizedX,
                NormalizedY = e.NormalizedY
            };

            if (_currentMode == ControlMode.Simultaneous)
            {
                _networkService.BroadcastMessage(message);
            }
            else if (_selectedClient != null)
            {
                _networkService.SendMessage(_selectedClient.Id, message);
            }
        }

        private void InputService_MouseButton(object sender, MouseButtonEventArgs e)
        {
            if (!_isServerRunning) return;

            var message = new MouseButtonMessage
            {
                Button = e.Button,
                IsPressed = e.IsPressed,
                NormalizedX = e.NormalizedX,
                NormalizedY = e.NormalizedY
            };

            if (_currentMode == ControlMode.Simultaneous)
            {
                _networkService.BroadcastMessage(message);
            }
            else if (_selectedClient != null)
            {
                _networkService.SendMessage(_selectedClient.Id, message);
            }
        }

        private void InputService_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!_isServerRunning) return;

            var message = new MouseWheelMessage
            {
                Delta = e.Delta,
                NormalizedX = e.NormalizedX,
                NormalizedY = e.NormalizedY
            };

            if (_currentMode == ControlMode.Simultaneous)
            {
                _networkService.BroadcastMessage(message);
            }
            else if (_selectedClient != null)
            {
                _networkService.SendMessage(_selectedClient.Id, message);
            }
        }

        private void InputService_KeyboardKey(object sender, KeyboardEventArgs e)
        {
            if (!_isServerRunning) return;

            var message = new KeyboardMessage
            {
                VirtualKeyCode = e.VirtualKeyCode,
                IsPressed = e.IsPressed,
                IsExtendedKey = e.IsExtendedKey
            };

            if (_currentMode == ControlMode.Simultaneous)
            {
                _networkService.BroadcastMessage(message);
            }
            else if (_selectedClient != null)
            {
                _networkService.SendMessage(_selectedClient.Id, message);
            }
        }

        #endregion
    }

    public class ClientViewModel : INotifyPropertyChanged
    {
        private string _id = string.Empty;
        private string _name = "Unknown";
        private string _ipAddress = string.Empty;
        private string _screenSize = "Unknown";
        private string _status = "Disconnected";
        private DateTime _lastActivity = DateTime.Now;

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                _ipAddress = value;
                OnPropertyChanged(nameof(IpAddress));
            }
        }

        public string ScreenSize
        {
            get => _screenSize;
            set
            {
                _screenSize = value;
                OnPropertyChanged(nameof(ScreenSize));
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public DateTime LastActivity
        {
            get => _lastActivity;
            set
            {
                _lastActivity = value;
                OnPropertyChanged(nameof(LastActivity));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}