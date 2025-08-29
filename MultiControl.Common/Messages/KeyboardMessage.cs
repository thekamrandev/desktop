using System;

namespace MultiControl.Common.Messages
{
    /// <summary>
    /// Message for keyboard key events
    /// </summary>
    [Serializable]
    public class KeyboardMessage : InputMessage
    {
        /// <summary>
        /// Virtual key code of the key that was pressed or released
        /// </summary>
        public int VirtualKeyCode { get; set; }

        /// <summary>
        /// Whether the key was pressed (true) or released (false)
        /// </summary>
        public bool IsPressed { get; set; }

        /// <summary>
        /// Whether the key is an extended key
        /// </summary>
        public bool IsExtendedKey { get; set; }

        /// <summary>
        /// Creates a new keyboard key message
        /// </summary>
        public KeyboardMessage()
        {
            MessageType = InputMessageType.KeyboardKey;
        }
    }
}