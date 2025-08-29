using MultiControl.Common.Messages;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using MultiControl.Common.Input;

namespace MultiControl.Common.Network
{
    /// <summary>
    /// TCP implementation of the network service
    /// </summary>
    public class TcpNetworkService : INetworkService
    {
        private TcpListener? _listener;
        private TcpClient? _serverConnection;
        private readonly ConcurrentDictionary<string, TcpClient> _clients = new();
        private readonly ConcurrentDictionary<string, string> _clientNames = new();
        private bool _isServer;
        private bool _isRunning;
        private int _port = 5000;
        private string _serverIp = "127.0.0.1";
        private string _clientName;

        /// <inheritdoc/>
        public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

        /// <inheritdoc/>
        public event EventHandler<ClientConnectionEventArgs>? ClientConnected;

        /// <inheritdoc/>
        public event EventHandler<ClientConnectionEventArgs>? ClientDisconnected;

        /// <summary>
        /// Creates a new instance of TcpNetworkService
        /// </summary>
        /// <param name="isServer">Whether this instance is a server or client</param>
        public TcpNetworkService(bool isServer)
        {
            _isServer = isServer;
            _clientName = Environment.MachineName;
        }

        /// <summary>
        /// Creates a new instance of TcpNetworkService with default server mode
        /// </summary>
        public TcpNetworkService()
        {
            _isServer = true;
            _clientName = Environment.MachineName;
        }

        /// <summary>
        /// Starts the server on the specified IP address and port
        /// </summary>
        /// <param name="ipAddress">IP address to listen on</param>
        /// <param name="port">Port to listen on</param>
        public void StartServer(IPAddress ipAddress, int port)
        {
            _isServer = true;
            StartAsync(port).Wait();
        }

        /// <summary>
        /// Starts the client and connects to the specified server
        /// </summary>
        /// <param name="ipAddress">Server IP address</param>
        /// <param name="port">Server port</param>
        public void StartClient(IPAddress ipAddress, int port)
        {
            _isServer = false;
            _serverConnection = new TcpClient();
            _serverConnection.Connect(ipAddress, port);
            _isRunning = true;
            _ = ReceiveMessagesAsync(_serverConnection, "server");
        }
        
        /// <summary>
        /// Starts the client and connects to the specified server
        /// </summary>
        /// <param name="serverIp">Server IP address as string</param>
        /// <param name="port">Server port</param>
        /// <param name="clientName">Name of this client</param>
        public void StartClient(string serverIp, int port, string clientName)
        {
            _clientName = clientName;
            IPAddress ipAddress = IPAddress.Parse(serverIp);
            StartClient(ipAddress, port);
            
            // Send handshake message with client information
            var handshakeMessage = new ConnectionHandshakeMessage
            {
                ClientId = Guid.NewGuid().ToString(),
                ClientName = clientName,
                ScreenWidth = Screen.PrimaryScreen.Bounds.Width,
                ScreenHeight = Screen.PrimaryScreen.Bounds.Height
            };
            
            SendMessage(string.Empty, handshakeMessage);
        }

        /// <summary>
        /// Stops the server and disconnects all clients
        /// </summary>
        public void StopServer()
        {
            StopAsync().Wait();
        }

        /// <summary>
        /// Stops the client and disconnects from the server
        /// </summary>
        public void StopClient()
        {
            StopAsync().Wait();
        }

        /// <summary>
        /// Disconnects a specific client
        /// </summary>
        /// <param name="clientId">ID of the client to disconnect</param>
        public void DisconnectClient(string clientId)
        {
            if (_clients.TryRemove(clientId, out var client))
            {
                client.Close();
                _clientNames.TryRemove(clientId, out var clientName);
                ClientDisconnected?.Invoke(this, new ClientConnectionEventArgs(clientId, clientName ?? clientId));
            }
        }

        /// <summary>
        /// Sends a message to a specific client or server
        /// </summary>
        /// <param name="clientId">ID of the client to send to (empty for server)</param>
        /// <param name="message">Message to send</param>
        public void SendMessage(string clientId, InputMessage message)
        {
            SendMessageAsync(message, clientId).Wait();
        }
        
        /// <summary>
        /// Sends a message to the server (client-side only)
        /// </summary>
        /// <param name="message">Message to send</param>
        public void SendMessage(InputMessage message)
        {
            SendMessageAsync(message, string.Empty).Wait();
        }

        /// <summary>
        /// Broadcasts a message to all connected clients
        /// </summary>
        /// <param name="message">Message to broadcast</param>
        public void BroadcastMessage(InputMessage message)
        {
            BroadcastMessageAsync(message).Wait();
        }

        /// <inheritdoc/>
        public async Task StartAsync(int port)
        {
            _port = port;
            await StartAsync();
        }
        
        /// <summary>
        /// Starts the network service asynchronously.
        /// </summary>
        public async Task StartAsync()
        {
            if (_isRunning)
                return;

            _isRunning = true;

            if (_isServer)
            {
                await StartServerAsync();
            }
            else
            {
                await StartClientAsync();
            }
        }
        
        /// <summary>
        /// Starts the server asynchronously.
        /// </summary>
        private async Task StartServerAsync()
        {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            
            // Accept clients in a loop
            _ = Task.Run(async () =>
            {
                while (_isRunning)
                {
                    try
                    {
                        var client = await _listener.AcceptTcpClientAsync();
                        _ = HandleClientConnectionAsync(client);
                    }
                    catch (Exception) when (!_isRunning)
                    {
                        // Ignore exceptions during shutdown
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error accepting client: {ex.Message}");
                    }
                }
            });
            
            await Task.CompletedTask;
        }
        
        /// <summary>
        /// Starts the client connection asynchronously.
        /// </summary>
        private async Task StartClientAsync()
        {
            _serverConnection = new TcpClient();
            await _serverConnection.ConnectAsync(_serverIp, _port);
            
            // Send handshake message
            var clientId = Guid.NewGuid().ToString();
            var handshake = new ConnectionHandshakeMessage
            {
                ClientId = clientId,
                ClientName = _clientName
            };
            
            await SendMessageToClientAsync(handshake, _serverConnection);
            
            // Start receiving messages
            _ = ReceiveMessagesAsync(_serverConnection, "server");
        }

        /// <inheritdoc/>
        public async Task StopAsync()
        {
            if (!_isRunning)
                return;

            _isRunning = false;

            if (_isServer)
            {
                _listener?.Stop();

                // Disconnect all clients
                foreach (var client in _clients.Values)
                {
                    client.Close();
                }

                _clients.Clear();
                _clientNames.Clear();
            }
            else
            {
                _serverConnection?.Close();
                _serverConnection = null;
            }

            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task SendMessageAsync(InputMessage message, string? clientId = null)
        {
            if (!_isRunning)
                return;

            if (_isServer)
            {
                if (clientId != null && _clients.TryGetValue(clientId, out var client))
                {
                    await SendMessageToClientAsync(message, client);
                }
            }
            else
            {
                if (_serverConnection != null)
                {
                    await SendMessageToClientAsync(message, _serverConnection);
                }
            }
        }

        /// <inheritdoc/>
        public async Task BroadcastMessageAsync(InputMessage message)
        {
            if (!_isRunning || !_isServer)
                return;

            foreach (var client in _clients.Values)
            {
                await SendMessageToClientAsync(message, client);
            }
        }

        private async Task HandleClientConnectionAsync(TcpClient client)
        {
            string clientId = Guid.NewGuid().ToString();
            string clientName = clientId; // Default name until handshake

            try
            {
                // Wait for handshake message
                var stream = client.GetStream();
                byte[] buffer = new byte[4096];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    var message = DeserializeMessage(buffer, bytesRead);

                    if (message is ConnectionHandshakeMessage handshake)
                    {
                        clientId = handshake.ClientId;
                        clientName = handshake.ClientName;

                        // Add client to dictionary
                        _clients[clientId] = client;
                        _clientNames[clientId] = clientName;

                        // Notify about new connection
                        var endpoint = client.Client.RemoteEndPoint;
                        ClientConnected?.Invoke(this, endpoint != null 
                            ? new ClientConnectionEventArgs(clientId, clientName, endpoint)
                            : new ClientConnectionEventArgs(clientId, clientName));

                        // Start receiving messages
                        await ReceiveMessagesAsync(client, clientId);
                    }
                    else
                    {
                        // Invalid handshake, close connection
                        client.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client connection: {ex.Message}");
                client.Close();
            }
        }

        // This method is duplicated below with a nullable return type, removing this version

        // This method is duplicated below, removing this version

        private async Task ReceiveMessagesAsync(TcpClient client, string clientId)
        {
            try
            {
                var stream = client.GetStream();
                byte[] buffer = new byte[4096];

                while (_isRunning && client.Connected)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                    {
                        // Client disconnected
                        break;
                    }

                    var message = DeserializeMessage(buffer, bytesRead);
                    if (message != null)
                    {
                        MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message, clientId));
                    }
                }
            }
            catch (Exception) when (!_isRunning)
            {
                // Ignore exceptions during shutdown
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving messages: {ex.Message}");
            }
            finally
            {
                // Handle disconnection
                if (_isServer)
                {
                    _clients.TryRemove(clientId, out _);
                    _clientNames.TryRemove(clientId, out var name);
                    ClientDisconnected?.Invoke(this, new ClientConnectionEventArgs(clientId, name ?? clientId));
                }
                else
                {
                    // Client disconnected from server
                    _isRunning = false;
                    ClientDisconnected?.Invoke(this, new ClientConnectionEventArgs(clientId, clientId));
                }
            }
        }

        private async Task SendMessageToClientAsync(InputMessage message, TcpClient client)
        {
            try
            {
                if (!client.Connected)
                    return;

                var json = JsonSerializer.Serialize(message);
                var data = System.Text.Encoding.UTF8.GetBytes(json);

                var stream = client.GetStream();
                await stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        private InputMessage? DeserializeMessage(byte[] buffer, int bytesRead)
        {
            try
            {
                var json = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                var messageType = JsonDocument.Parse(json).RootElement.GetProperty("MessageType").GetInt32();

                return messageType switch
                {
                    (int)InputMessageType.MouseMove => JsonSerializer.Deserialize<MouseMoveMessage>(json),
                    (int)InputMessageType.MouseButton => JsonSerializer.Deserialize<MouseButtonMessage>(json),
                    (int)InputMessageType.MouseWheel => JsonSerializer.Deserialize<MouseWheelMessage>(json),
                    (int)InputMessageType.KeyboardKey => JsonSerializer.Deserialize<KeyboardMessage>(json),
                    (int)InputMessageType.ConnectionHandshake => JsonSerializer.Deserialize<ConnectionHandshakeMessage>(json),
                    (int)InputMessageType.ModeChange => JsonSerializer.Deserialize<ModeChangeMessage>(json),
                    (int)InputMessageType.ClipboardData => JsonSerializer.Deserialize<ClipboardMessage>(json),
                    _ => null
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing message: {ex.Message}");
                return null;
            }
        }
    }
}