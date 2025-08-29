using MultiControl.Common.Messages;
using System;

namespace MultiControl.Common.Input
{
    /// <summary>
    /// Interface for input service that captures and simulates input events
    /// </summary>
    public interface IInputService
    {
        /// <summary>
        /// Event raised when mouse movement is detected
        /// </summary>
        event EventHandler<MouseMoveEventArgs> MouseMove;

        /// <summary>
        /// Event raised when a mouse button is pressed or released
        /// </summary>
        event EventHandler<MouseButtonEventArgs> MouseButton;

        /// <summary>
        /// Event raised when the mouse wheel is scrolled
        /// </summary>
        event EventHandler<MouseWheelEventArgs> MouseWheel;

        /// <summary>
        /// Event raised when a keyboard key is pressed or released
        /// </summary>
        event EventHandler<KeyboardEventArgs> KeyboardKey;

        /// <summary>
        /// Starts capturing input events
        /// </summary>
        void StartCapture();

        /// <summary>
        /// Stops capturing input events
        /// </summary>
        void StopCapture();

        /// <summary>
        /// Simulates a mouse movement
        /// </summary>
        /// <param name="x">X coordinate (absolute or normalized based on implementation)</param>
        /// <param name="y">Y coordinate (absolute or normalized based on implementation)</param>
        void SimulateMouseMove(double x, double y);

        /// <summary>
        /// Simulates a mouse button press or release
        /// </summary>
        /// <param name="button">The mouse button</param>
        /// <param name="isPressed">Whether the button is pressed (true) or released (false)</param>
        void SimulateMouseButton(MouseButton button, bool isPressed);

        /// <summary>
        /// Simulates a mouse wheel scroll
        /// </summary>
        /// <param name="delta">The scroll delta</param>
        void SimulateMouseWheel(int delta);

        /// <summary>
        /// Simulates a keyboard key press or release
        /// </summary>
        /// <param name="virtualKeyCode">The virtual key code</param>
        /// <param name="isPressed">Whether the key is pressed (true) or released (false)</param>
        /// <param name="isExtendedKey">Whether the key is an extended key</param>
        void SimulateKeyboardKey(int virtualKeyCode, bool isPressed, bool isExtendedKey = false);

        /// <summary>
        /// Gets the current screen width
        /// </summary>
        int ScreenWidth { get; }

        /// <summary>
        /// Gets the current screen height
        /// </summary>
        int ScreenHeight { get; }
    }

    /// <summary>
    /// Event arguments for mouse move events
    /// </summary>
    public class MouseMoveEventArgs : EventArgs
    {
        /// <summary>
        /// X position of the cursor
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Y position of the cursor
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// X position of the cursor as a normalized value between 0.0 and 1.0
        /// </summary>
        public double NormalizedX { get; }

        /// <summary>
        /// Y position of the cursor as a normalized value between 0.0 and 1.0
        /// </summary>
        public double NormalizedY { get; }

        /// <summary>
        /// Creates a new instance of MouseMoveEventArgs
        /// </summary>
        /// <param name="x">X position of the cursor</param>
        /// <param name="y">Y position of the cursor</param>
        /// <param name="screenWidth">Screen width for normalization</param>
        /// <param name="screenHeight">Screen height for normalization</param>
        public MouseMoveEventArgs(int x, int y, int screenWidth, int screenHeight)
        {
            X = x;
            Y = y;
            NormalizedX = (double)x / screenWidth;
            NormalizedY = (double)y / screenHeight;
        }
    }

    /// <summary>
    /// Event arguments for mouse button events
    /// </summary>
    public class MouseButtonEventArgs : EventArgs
    {
        /// <summary>
        /// The mouse button
        /// </summary>
        public MouseButton Button { get; }

        /// <summary>
        /// Whether the button is pressed (true) or released (false)
        /// </summary>
        public bool IsPressed { get; }

        /// <summary>
        /// X position of the cursor
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Y position of the cursor
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// X position of the cursor as a normalized value between 0.0 and 1.0
        /// </summary>
        public double NormalizedX { get; }

        /// <summary>
        /// Y position of the cursor as a normalized value between 0.0 and 1.0
        /// </summary>
        public double NormalizedY { get; }

        /// <summary>
        /// Creates a new instance of MouseButtonEventArgs
        /// </summary>
        /// <param name="button">The mouse button</param>
        /// <param name="isPressed">Whether the button is pressed (true) or released (false)</param>
        /// <param name="x">X position of the cursor</param>
        /// <param name="y">Y position of the cursor</param>
        /// <param name="screenWidth">Screen width for normalization</param>
        /// <param name="screenHeight">Screen height for normalization</param>
        public MouseButtonEventArgs(MouseButton button, bool isPressed, int x, int y, int screenWidth, int screenHeight)
        {
            Button = button;
            IsPressed = isPressed;
            X = x;
            Y = y;
            NormalizedX = (double)x / screenWidth;
            NormalizedY = (double)y / screenHeight;
        }
    }

    /// <summary>
    /// Event arguments for mouse wheel events
    /// </summary>
    public class MouseWheelEventArgs : EventArgs
    {
        /// <summary>
        /// The scroll delta
        /// </summary>
        public int Delta { get; }

        /// <summary>
        /// X position of the cursor
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Y position of the cursor
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// X position of the cursor as a normalized value between 0.0 and 1.0
        /// </summary>
        public double NormalizedX { get; }

        /// <summary>
        /// Y position of the cursor as a normalized value between 0.0 and 1.0
        /// </summary>
        public double NormalizedY { get; }

        /// <summary>
        /// Creates a new instance of MouseWheelEventArgs
        /// </summary>
        /// <param name="delta">The scroll delta</param>
        /// <param name="x">X position of the cursor</param>
        /// <param name="y">Y position of the cursor</param>
        /// <param name="screenWidth">Screen width for normalization</param>
        /// <param name="screenHeight">Screen height for normalization</param>
        public MouseWheelEventArgs(int delta, int x, int y, int screenWidth, int screenHeight)
        {
            Delta = delta;
            X = x;
            Y = y;
            NormalizedX = (double)x / screenWidth;
            NormalizedY = (double)y / screenHeight;
        }
    }

    /// <summary>
    /// Event arguments for keyboard events
    /// </summary>
    public class KeyboardEventArgs : EventArgs
    {
        /// <summary>
        /// The virtual key code
        /// </summary>
        public int VirtualKeyCode { get; }

        /// <summary>
        /// Whether the key is pressed (true) or released (false)
        /// </summary>
        public bool IsPressed { get; }

        /// <summary>
        /// Whether the key is an extended key
        /// </summary>
        public bool IsExtendedKey { get; }

        /// <summary>
        /// Creates a new instance of KeyboardEventArgs
        /// </summary>
        /// <param name="virtualKeyCode">The virtual key code</param>
        /// <param name="isPressed">Whether the key is pressed (true) or released (false)</param>
        /// <param name="isExtendedKey">Whether the key is an extended key</param>
        public KeyboardEventArgs(int virtualKeyCode, bool isPressed, bool isExtendedKey = false)
        {
            VirtualKeyCode = virtualKeyCode;
            IsPressed = isPressed;
            IsExtendedKey = isExtendedKey;
        }
    }
}