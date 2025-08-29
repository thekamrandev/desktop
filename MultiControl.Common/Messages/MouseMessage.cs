using System;

namespace MultiControl.Common.Messages
{
    /// <summary>
    /// Message for mouse movement events
    /// </summary>
    [Serializable]
    public class MouseMoveMessage : InputMessage
    {
        /// <summary>
        /// X position of the cursor as a normalized value between 0.0 and 1.0
        /// </summary>
        public double NormalizedX { get; set; }

        /// <summary>
        /// Y position of the cursor as a normalized value between 0.0 and 1.0
        /// </summary>
        public double NormalizedY { get; set; }

        /// <summary>
        /// Creates a new mouse move message
        /// </summary>
        public MouseMoveMessage()
        {
            MessageType = InputMessageType.MouseMove;
        }
    }

    /// <summary>
    /// Message for mouse button events
    /// </summary>
    [Serializable]
    public class MouseButtonMessage : InputMessage
    {
        /// <summary>
        /// The mouse button that was pressed or released
        /// </summary>
        public MouseButton Button { get; set; }

        /// <summary>
        /// Whether the button was pressed (true) or released (false)
        /// </summary>
        public bool IsPressed { get; set; }

        /// <summary>
        /// X position of the cursor as a normalized value between 0.0 and 1.0
        /// </summary>
        public double NormalizedX { get; set; }

        /// <summary>
        /// Y position of the cursor as a normalized value between 0.0 and 1.0
        /// </summary>
        public double NormalizedY { get; set; }

        /// <summary>
        /// Creates a new mouse button message
        /// </summary>
        public MouseButtonMessage()
        {
            MessageType = InputMessageType.MouseButton;
        }
    }

    /// <summary>
    /// Message for mouse wheel events
    /// </summary>
    [Serializable]
    public class MouseWheelMessage : InputMessage
    {
        /// <summary>
        /// Delta value for the mouse wheel scroll
        /// </summary>
        public int Delta { get; set; }

        /// <summary>
        /// X position of the cursor as a normalized value between 0.0 and 1.0
        /// </summary>
        public double NormalizedX { get; set; }

        /// <summary>
        /// Y position of the cursor as a normalized value between 0.0 and 1.0
        /// </summary>
        public double NormalizedY { get; set; }

        /// <summary>
        /// Creates a new mouse wheel message
        /// </summary>
        public MouseWheelMessage()
        {
            MessageType = InputMessageType.MouseWheel;
        }
    }

    /// <summary>
    /// Enum representing mouse buttons
    /// </summary>
    public enum MouseButton
    {
        /// <summary>
        /// Left mouse button
        /// </summary>
        Left,

        /// <summary>
        /// Right mouse button
        /// </summary>
        Right,

        /// <summary>
        /// Middle mouse button
        /// </summary>
        Middle,

        /// <summary>
        /// X1 mouse button (typically back button)
        /// </summary>
        XButton1,

        /// <summary>
        /// X2 mouse button (typically forward button)
        /// </summary>
        XButton2
    }
}