using System;

namespace MultiControl.Common.Messages
{
    /// <summary>
    /// Message for changing control mode
    /// </summary>
    [Serializable]
    public class ModeChangeMessage : InputMessage
    {
        /// <summary>
        /// The control mode to switch to
        /// </summary>
        public ControlMode Mode { get; set; }

        /// <summary>
        /// Creates a new mode change message
        /// </summary>
        public ModeChangeMessage()
        {
            MessageType = InputMessageType.ModeChange;
        }
    }

    /// <summary>
    /// Message for connection handshake
    /// </summary>
    [Serializable]
    public class ConnectionHandshakeMessage : InputMessage
    {
        /// <summary>
        /// Client ID for identification
        /// </summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// Client name for display
        /// </summary>
        public string ClientName { get; set; } = string.Empty;

        /// <summary>
        /// Screen width of the client
        /// </summary>
        public int ScreenWidth { get; set; }

        /// <summary>
        /// Screen height of the client
        /// </summary>
        public int ScreenHeight { get; set; }

        /// <summary>
        /// Creates a new connection handshake message
        /// </summary>
        public ConnectionHandshakeMessage()
        {
            MessageType = InputMessageType.ConnectionHandshake;
        }
    }

    /// <summary>
    /// Control modes for the application
    /// </summary>
    public enum ControlMode
    {
        /// <summary>
        /// Individual control mode - admin controls one client at a time
        /// </summary>
        Individual,

        /// <summary>
        /// Simultaneous control mode - admin controls all clients at once
        /// </summary>
        Simultaneous
    }
}