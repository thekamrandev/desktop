using System;
using System.Text.Json.Serialization;

namespace MultiControl.Common.Messages
{
    /// <summary>
    /// Base class for all input messages sent between server and clients
    /// </summary>
    [Serializable]
    public abstract class InputMessage
    {
        /// <summary>
        /// Type of input message
        /// </summary>
        [JsonInclude]
        public InputMessageType MessageType { get; protected set; }

        /// <summary>
        /// Timestamp when the message was created
        /// </summary>
        [JsonInclude]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Types of input messages that can be sent between server and clients
    /// </summary>
    public enum InputMessageType
    {
        /// <summary>
        /// Mouse movement message
        /// </summary>
        MouseMove,

        /// <summary>
        /// Mouse button press/release message
        /// </summary>
        MouseButton,

        /// <summary>
        /// Mouse wheel scroll message
        /// </summary>
        MouseWheel,

        /// <summary>
        /// Keyboard key press/release message
        /// </summary>
        KeyboardKey,

        /// <summary>
        /// Connection handshake message
        /// </summary>
        ConnectionHandshake,

        /// <summary>
        /// Mode change message (individual vs simultaneous control)
        /// </summary>
        ModeChange,

        /// <summary>
        /// Clipboard data message
        /// </summary>
        ClipboardData
    }
}