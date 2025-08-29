using MultiControl.Common.Messages;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MultiControl.Common.Network
{
    /// <summary>
    /// Interface for network communication service
    /// </summary>
    public interface INetworkService
    {
        /// <summary>
        /// Event raised when a message is received
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Event raised when a client connects
        /// </summary>
        event EventHandler<ClientConnectionEventArgs> ClientConnected;

        /// <summary>
        /// Event raised when a client disconnects
        /// </summary>
        event EventHandler<ClientConnectionEventArgs> ClientDisconnected;

        /// <summary>
        /// Starts the network service
        /// </summary>
        /// <param name="port">Port to listen on (for server) or connect to (for client)</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task StartAsync(int port);

        /// <summary>
        /// Stops the network service
        /// </summary>
        /// <returns>Task representing the asynchronous operation</returns>
        Task StopAsync();

        /// <summary>
        /// Sends a message to a specific client
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="clientId">ID of the client to send to (null for broadcast to all clients)</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task SendMessageAsync(InputMessage message, string? clientId = null);

        /// <summary>
        /// Broadcasts a message to all connected clients
        /// </summary>
        /// <param name="message">Message to broadcast</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task BroadcastMessageAsync(InputMessage message);
    }

    /// <summary>
    /// Event arguments for message received events
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// The received message
        /// </summary>
        public InputMessage Message { get; }

        /// <summary>
        /// ID of the client that sent the message
        /// </summary>
        public string ClientId { get; }

        /// <summary>
        /// Creates a new instance of MessageReceivedEventArgs
        /// </summary>
        /// <param name="message">The received message</param>
        /// <param name="clientId">ID of the client that sent the message</param>
        public MessageReceivedEventArgs(InputMessage message, string clientId)
        {
            Message = message;
            ClientId = clientId;
        }
    }

    /// <summary>
    /// Event arguments for client connection events
    /// </summary>
    public class ClientConnectionEventArgs : EventArgs
    {
        /// <summary>
        /// ID of the client
        /// </summary>
        public string ClientId { get; }

        /// <summary>
        /// Name of the client
        /// </summary>
        public string ClientName { get; }

        /// <summary>
        /// Endpoint of the client
        /// </summary>
        public EndPoint? EndPoint { get; }

        /// <summary>
        /// Creates a new instance of ClientConnectionEventArgs
        /// </summary>
        /// <param name="clientId">ID of the client</param>
        /// <param name="clientName">Name of the client</param>
        public ClientConnectionEventArgs(string clientId, string clientName)
        {
            ClientId = clientId;
            ClientName = clientName;
        }

        /// <summary>
        /// Creates a new instance of ClientConnectionEventArgs
        /// </summary>
        /// <param name="clientId">ID of the client</param>
        /// <param name="clientName">Name of the client</param>
        /// <param name="endPoint">Endpoint of the client</param>
        public ClientConnectionEventArgs(string clientId, string clientName, EndPoint endPoint)
        {
            ClientId = clientId;
            ClientName = clientName;
            EndPoint = endPoint;
        }
    }

    /// <summary>
    /// Event arguments for client disconnection events
    /// </summary>
    public class ClientDisconnectedEventArgs : EventArgs
    {
        /// <summary>
        /// ID of the client
        /// </summary>
        public string ClientId { get; }

        /// <summary>
        /// Creates a new instance of ClientDisconnectedEventArgs
        /// </summary>
        /// <param name="clientId">ID of the client</param>
        public ClientDisconnectedEventArgs(string clientId)
        {
            ClientId = clientId;
        }
    }
}