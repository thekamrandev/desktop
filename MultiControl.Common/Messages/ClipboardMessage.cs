using System;

namespace MultiControl.Common.Messages
{
    /// <summary>
    /// Message for clipboard data transfer
    /// </summary>
    [Serializable]
    public class ClipboardMessage : InputMessage
    {
        /// <summary>
        /// The clipboard data format
        /// </summary>
        public ClipboardFormat Format { get; set; }

        /// <summary>
        /// The clipboard data as a byte array
        /// </summary>
        public byte[] Data { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Creates a new clipboard data message
        /// </summary>
        public ClipboardMessage()
        {
            MessageType = InputMessageType.ClipboardData;
        }
    }

    /// <summary>
    /// Clipboard data formats
    /// </summary>
    public enum ClipboardFormat
    {
        /// <summary>
        /// Text format
        /// </summary>
        Text,

        /// <summary>
        /// Image format
        /// </summary>
        Image,

        /// <summary>
        /// File list format
        /// </summary>
        FileList
    }
}