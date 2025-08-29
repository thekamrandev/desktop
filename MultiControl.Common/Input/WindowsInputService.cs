using MultiControl.Common.Messages;
using System;
using System.Runtime.InteropServices;

namespace MultiControl.Common.Input
{
    /// <summary>
    /// Windows implementation of the input service using Windows API
    /// </summary>
    public class WindowsInputService : IInputService, IDisposable
    {
        private bool _isCapturing;
        private readonly MouseHook _mouseHook;
        private readonly KeyboardHook _keyboardHook;

        /// <inheritdoc/>
        public event EventHandler<MouseMoveEventArgs>? MouseMove;

        /// <inheritdoc/>
        public event EventHandler<MouseButtonEventArgs>? MouseButton;

        /// <inheritdoc/>
        public event EventHandler<MouseWheelEventArgs>? MouseWheel;

        /// <inheritdoc/>
        public event EventHandler<KeyboardEventArgs>? KeyboardKey;

        /// <inheritdoc/>
        public int ScreenWidth => 1920; // fallback or platform-specific implementation

        /// <inheritdoc/>
        public int ScreenHeight => 1080; // fallback or platform-specific implementation

        /// <summary>
        /// Creates a new instance of WindowsInputService
        /// </summary>
        public WindowsInputService()
        {
            _mouseHook = new MouseHook();
            _keyboardHook = new KeyboardHook();

            _mouseHook.MouseMove += OnMouseMove;
            _mouseHook.MouseDown += OnMouseDown;
            _mouseHook.MouseUp += OnMouseUp;
            _mouseHook.MouseWheel += OnMouseWheel;

            _keyboardHook.KeyDown += OnKeyDown;
            _keyboardHook.KeyUp += OnKeyUp;
        }

        /// <inheritdoc/>
        public void StartCapture()
        {
            if (_isCapturing)
                return;

            _isCapturing = true;
            _mouseHook.Install();
            _keyboardHook.Install();
        }

        /// <inheritdoc/>
        public void StopCapture()
        {
            if (!_isCapturing)
                return;

            _isCapturing = false;
            _mouseHook.Uninstall();
            _keyboardHook.Uninstall();
        }

        /// <inheritdoc/>
        public void SimulateMouseMove(double x, double y)
        {
            // Convert normalized coordinates to absolute if needed
            int absX = x >= 0 && x <= 1 ? (int)(x * ScreenWidth) : (int)x;
            int absY = y >= 0 && y <= 1 ? (int)(y * ScreenHeight) : (int)y;

            // Set cursor position
            NativeMethods.SetCursorPos(absX, absY);
        }

        /// <inheritdoc/>
        public void SimulateMouseButton(MouseButton button, bool isPressed)
        {
            NativeMethods.MOUSEINPUT input = new NativeMethods.MOUSEINPUT();

            // Set flags based on button and action
            switch (button)
            {
                case MultiControl.Common.Messages.MouseButton.Left:
                    input.dwFlags = isPressed ? (uint)NativeMethods.MOUSEEVENTF_LEFTDOWN : (uint)NativeMethods.MOUSEEVENTF_LEFTUP;
                    break;
                case MultiControl.Common.Messages.MouseButton.Right:
                    input.dwFlags = isPressed ? (uint)NativeMethods.MOUSEEVENTF_RIGHTDOWN : (uint)NativeMethods.MOUSEEVENTF_RIGHTUP;
                    break;
                case MultiControl.Common.Messages.MouseButton.Middle:
                    input.dwFlags = isPressed ? (uint)NativeMethods.MOUSEEVENTF_MIDDLEDOWN : (uint)NativeMethods.MOUSEEVENTF_MIDDLEUP;
                    break;
                case MultiControl.Common.Messages.MouseButton.XButton1:
                    input.dwFlags = isPressed ? (uint)NativeMethods.MOUSEEVENTF_XDOWN : (uint)NativeMethods.MOUSEEVENTF_XUP;
                    input.mouseData = NativeMethods.XBUTTON1;
                    break;
                case MultiControl.Common.Messages.MouseButton.XButton2:
                    input.dwFlags = isPressed ? (uint)NativeMethods.MOUSEEVENTF_XDOWN : (uint)NativeMethods.MOUSEEVENTF_XUP;
                    input.mouseData = NativeMethods.XBUTTON2;
                    break;
            }

            // Create input structure
            NativeMethods.INPUT[] inputs = new NativeMethods.INPUT[1];
            inputs[0].type = NativeMethods.INPUT_MOUSE;
            inputs[0].mi = input;

            // Send input
            NativeMethods.SendInput(1, inputs, Marshal.SizeOf(typeof(NativeMethods.INPUT)));
        }
        
        /// <summary>
        /// Simulates a mouse button down event.
        /// </summary>
        /// <param name="button">The mouse button.</param>
        public void SimulateMouseButtonDown(MouseButton button)
        {
            SimulateMouseButton(button, true);
        }
        
        /// <summary>
        /// Simulates a mouse button up event.
        /// </summary>
        /// <param name="button">The mouse button.</param>
        public void SimulateMouseButtonUp(MouseButton button)
        {
            SimulateMouseButton(button, false);
        }

        /// <summary>
        /// Simulates a mouse button press at the specified coordinates
        /// </summary>
        /// <param name="button">The mouse button</param>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        public void SimulateMouseButtonDown(MouseButton button, int x, int y)
        {
            // First move the mouse to the specified position
            SimulateMouseMove(x, y);
            
            // Then simulate the button press
            SimulateMouseButton(button, true);
        }

        /// <summary>
        /// Simulates a mouse button release at the specified coordinates
        /// </summary>
        /// <param name="button">The mouse button</param>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        public void SimulateMouseButtonUp(MouseButton button, int x, int y)
        {
            // First move the mouse to the specified position
            SimulateMouseMove(x, y);
            
            // Then simulate the button release
            SimulateMouseButton(button, false);
        }

        /// <inheritdoc/>
        public void SimulateMouseWheel(int delta)
        {
            NativeMethods.MOUSEINPUT input = new NativeMethods.MOUSEINPUT();
            input.dwFlags = NativeMethods.MOUSEEVENTF_WHEEL;
            input.mouseData = (uint)delta;

            // Create input structure
            NativeMethods.INPUT[] inputs = new NativeMethods.INPUT[1];
            inputs[0].type = NativeMethods.INPUT_MOUSE;
            inputs[0].mi = input;

            // Send input
            NativeMethods.SendInput(1, inputs, Marshal.SizeOf(typeof(NativeMethods.INPUT)));
        }
        
        /// <summary>
        /// Simulates a mouse wheel event at the specified coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="delta">The wheel delta.</param>
        public void SimulateMouseWheel(int x, int y, int delta)
        {
            // Move to the specified position first
            SimulateMouseMove(x, y);
            
            // Then simulate the wheel event
            SimulateMouseWheel(delta);
        }

        /// <inheritdoc/>
        public void SimulateKeyboardKey(int virtualKeyCode, bool isPressed, bool isExtendedKey = false)
        {
            NativeMethods.KEYBDINPUT input = new NativeMethods.KEYBDINPUT();
            input.wVk = (ushort)virtualKeyCode;

            if (isPressed)
            {
                input.dwFlags = isExtendedKey ? (uint)NativeMethods.KEYEVENTF_EXTENDEDKEY : 0u;
            }
            else
            {
                input.dwFlags = (uint)NativeMethods.KEYEVENTF_KEYUP;
                if (isExtendedKey)
                    input.dwFlags |= (uint)NativeMethods.KEYEVENTF_EXTENDEDKEY;
            }

            // Create input structure
            NativeMethods.INPUT[] inputs = new NativeMethods.INPUT[1];
            inputs[0].type = NativeMethods.INPUT_KEYBOARD;
            inputs[0].ki = input;

            // Send input
            NativeMethods.SendInput(1, inputs, Marshal.SizeOf(typeof(NativeMethods.INPUT)));
        }
        
        /// <summary>
        /// Simulates a key down event.
        /// </summary>
        /// <param name="virtualKeyCode">The virtual key code.</param>
        /// <param name="isExtendedKey">Whether the key is an extended key.</param>
        public void SimulateKeyDown(int virtualKeyCode, bool isExtendedKey = false)
        {
            SimulateKeyboardKey(virtualKeyCode, true, isExtendedKey);
        }
        
        /// <summary>
        /// Simulates a key down event.
        /// </summary>
        /// <param name="key">The key.</param>
        public void SimulateKeyDown(Keys key)
        {
            SimulateKeyboardKey((int)key, true);
        }
        
        /// <summary>
        /// Simulates a key up event.
        /// </summary>
        /// <param name="virtualKeyCode">The virtual key code.</param>
        /// <param name="isExtendedKey">Whether the key is an extended key.</param>
        public void SimulateKeyUp(int virtualKeyCode, bool isExtendedKey = false)
        {
            SimulateKeyboardKey(virtualKeyCode, false, isExtendedKey);
        }
        
        /// <summary>
        /// Simulates a key up event.
        /// </summary>
        /// <param name="key">The key.</param>
        public void SimulateKeyUp(Keys key)
        {
            SimulateKeyboardKey((int)key, false);
        }

        private void OnMouseMove(object? sender, MouseHookEventArgs e)
        {
            MouseMove?.Invoke(this, new MouseMoveEventArgs(e.X, e.Y, ScreenWidth, ScreenHeight));
        }

        private void OnMouseDown(object? sender, MouseHookEventArgs e)
        {
            MouseButton button = GetMouseButton(e.Button);
            MouseButton?.Invoke(this, new MouseButtonEventArgs(button, true, e.X, e.Y, ScreenWidth, ScreenHeight));
        }

        private void OnMouseUp(object? sender, MouseHookEventArgs e)
        {
            MouseButton button = GetMouseButton(e.Button);
            MouseButton?.Invoke(this, new MouseButtonEventArgs(button, false, e.X, e.Y, ScreenWidth, ScreenHeight));
        }

        private void OnMouseWheel(object? sender, MouseHookEventArgs e)
        {
            MouseWheel?.Invoke(this, new MouseWheelEventArgs(e.Delta, e.X, e.Y, ScreenWidth, ScreenHeight));
        }

        private void OnKeyDown(object? sender, KeyboardHookEventArgs e)
        {
            KeyboardKey?.Invoke(this, new KeyboardEventArgs(e.VirtualKeyCode, true, e.IsExtendedKey));
        }

        private void OnKeyUp(object? sender, KeyboardHookEventArgs e)
        {
            KeyboardKey?.Invoke(this, new KeyboardEventArgs(e.VirtualKeyCode, false, e.IsExtendedKey));
        }

        private static MultiControl.Common.Messages.MouseButton GetMouseButton(MouseButtons button)
        {
            return button switch
            {
                MouseButtons.Left => MultiControl.Common.Messages.MouseButton.Left,
                MouseButtons.Right => MultiControl.Common.Messages.MouseButton.Right,
                MouseButtons.Middle => MultiControl.Common.Messages.MouseButton.Middle,
                MouseButtons.XButton1 => MultiControl.Common.Messages.MouseButton.XButton1,
                MouseButtons.XButton2 => MultiControl.Common.Messages.MouseButton.XButton2,
                _ => MultiControl.Common.Messages.MouseButton.Left
            };
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            StopCapture();
            _mouseHook.Dispose();
            _keyboardHook.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Mouse hook for capturing mouse events
    /// </summary>
    internal class MouseHook : IDisposable
    {
        private IntPtr _hookId = IntPtr.Zero;
        private readonly NativeMethods.HookProc _hookProc;

        public event EventHandler<MouseHookEventArgs>? MouseMove;
        public event EventHandler<MouseHookEventArgs>? MouseDown;
        public event EventHandler<MouseHookEventArgs>? MouseUp;
        public event EventHandler<MouseHookEventArgs>? MouseWheel;

        public MouseHook()
        {
            _hookProc = HookCallback;
        }

        public void Install()
        {
            if (_hookId != IntPtr.Zero)
                return;

            _hookId = NativeMethods.SetWindowsHookEx(
                NativeMethods.WH_MOUSE_LL,
                _hookProc,
                IntPtr.Zero,
                0);
        }

        public void Uninstall()
        {
            if (_hookId == IntPtr.Zero)
                return;

            NativeMethods.UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                NativeMethods.MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<NativeMethods.MSLLHOOKSTRUCT>(lParam);

                int x = hookStruct.pt.x;
                int y = hookStruct.pt.y;

                switch ((int)wParam)
                {
                    case NativeMethods.WM_MOUSEMOVE:
                        MouseMove?.Invoke(this, new MouseHookEventArgs(x, y, 0, MouseButtons.None));
                        break;
                    case NativeMethods.WM_LBUTTONDOWN:
                        MouseDown?.Invoke(this, new MouseHookEventArgs(x, y, 0, MouseButtons.Left));
                        break;
                    case NativeMethods.WM_LBUTTONUP:
                        MouseUp?.Invoke(this, new MouseHookEventArgs(x, y, 0, MouseButtons.Left));
                        break;
                    case NativeMethods.WM_RBUTTONDOWN:
                        MouseDown?.Invoke(this, new MouseHookEventArgs(x, y, 0, MouseButtons.Right));
                        break;
                    case NativeMethods.WM_RBUTTONUP:
                        MouseUp?.Invoke(this, new MouseHookEventArgs(x, y, 0, MouseButtons.Right));
                        break;
                    case NativeMethods.WM_MBUTTONDOWN:
                        MouseDown?.Invoke(this, new MouseHookEventArgs(x, y, 0, MouseButtons.Middle));
                        break;
                    case NativeMethods.WM_MBUTTONUP:
                        MouseUp?.Invoke(this, new MouseHookEventArgs(x, y, 0, MouseButtons.Middle));
                        break;
                    case NativeMethods.WM_XBUTTONDOWN:
                        MouseDown?.Invoke(this, new MouseHookEventArgs(x, y, 0, 
                            NativeMethods.HIWORD(hookStruct.mouseData) == 1 ? MouseButtons.XButton1 : MouseButtons.XButton2));
                        break;
                    case NativeMethods.WM_XBUTTONUP:
                        MouseUp?.Invoke(this, new MouseHookEventArgs(x, y, 0, 
                            NativeMethods.HIWORD(hookStruct.mouseData) == 1 ? MouseButtons.XButton1 : MouseButtons.XButton2));
                        break;
                    case NativeMethods.WM_MOUSEWHEEL:
                        MouseWheel?.Invoke(this, new MouseHookEventArgs(x, y, NativeMethods.HIWORD(hookStruct.mouseData), MouseButtons.None));
                        break;
                }
            }

            return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            Uninstall();
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Keyboard hook for capturing keyboard events
    /// </summary>
    internal class KeyboardHook : IDisposable
    {
        private IntPtr _hookId = IntPtr.Zero;
        private readonly NativeMethods.HookProc _hookProc;

        public event EventHandler<KeyboardHookEventArgs>? KeyDown;
        public event EventHandler<KeyboardHookEventArgs>? KeyUp;

        public KeyboardHook()
        {
            _hookProc = HookCallback;
        }

        public void Install()
        {
            if (_hookId != IntPtr.Zero)
                return;

            _hookId = NativeMethods.SetWindowsHookEx(
                NativeMethods.WH_KEYBOARD_LL,
                _hookProc,
                IntPtr.Zero,
                0);
        }

        public void Uninstall()
        {
            if (_hookId == IntPtr.Zero)
                return;

            NativeMethods.UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                NativeMethods.KBDLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<NativeMethods.KBDLLHOOKSTRUCT>(lParam);

                bool isExtendedKey = (hookStruct.flags & NativeMethods.LLKHF_EXTENDED) != 0;

                switch ((int)wParam)
                {
                    case NativeMethods.WM_KEYDOWN:
                    case NativeMethods.WM_SYSKEYDOWN:
                        KeyDown?.Invoke(this, new KeyboardHookEventArgs((int)hookStruct.vkCode, isExtendedKey));
                        break;
                    case NativeMethods.WM_KEYUP:
                    case NativeMethods.WM_SYSKEYUP:
                        KeyUp?.Invoke(this, new KeyboardHookEventArgs((int)hookStruct.vkCode, isExtendedKey));
                        break;
                }
            }

            return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            Uninstall();
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Event arguments for mouse hook events
    /// </summary>
    internal class MouseHookEventArgs : EventArgs
    {
        public int X { get; }
        public int Y { get; }
        public int Delta { get; }
        public MouseButtons Button { get; }

        public MouseHookEventArgs(int x, int y, int delta, MouseButtons button)
        {
            X = x;
            Y = y;
            Delta = delta;
            Button = button;
        }
    }

    /// <summary>
    /// Event arguments for keyboard hook events
    /// </summary>
    internal class KeyboardHookEventArgs : EventArgs
    {
        public int VirtualKeyCode { get; }
        public bool IsExtendedKey { get; }

        public KeyboardHookEventArgs(int virtualKeyCode, bool isExtendedKey)
        {
            VirtualKeyCode = virtualKeyCode;
            IsExtendedKey = isExtendedKey;
        }
    }

    /// <summary>
    /// Native methods for Windows API calls
    /// </summary>
    internal static class NativeMethods
    {
        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        // Hook types
        public const int WH_MOUSE_LL = 14;
        public const int WH_KEYBOARD_LL = 13;

        // Mouse messages
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_MBUTTONDOWN = 0x0207;
        public const int WM_MBUTTONUP = 0x0208;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_XBUTTONDOWN = 0x020B;
        public const int WM_XBUTTONUP = 0x020C;

        // Keyboard messages
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int WM_SYSKEYDOWN = 0x0104;
        public const int WM_SYSKEYUP = 0x0105;

        // Keyboard flags
        public const int LLKHF_EXTENDED = 0x01;

        // Mouse flags
        public const int MOUSEEVENTF_MOVE = 0x0001;
        public const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        public const int MOUSEEVENTF_LEFTUP = 0x0004;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        public const int MOUSEEVENTF_RIGHTUP = 0x0010;
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        public const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        public const int MOUSEEVENTF_XDOWN = 0x0080;
        public const int MOUSEEVENTF_XUP = 0x0100;
        public const int MOUSEEVENTF_WHEEL = 0x0800;
        public const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        // Keyboard flags
        public const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        public const int KEYEVENTF_KEYUP = 0x0002;

        // Input type
        public const int INPUT_MOUSE = 0;
        public const int INPUT_KEYBOARD = 1;

        // XButton values
        public const uint XBUTTON1 = 0x0001;
        public const uint XBUTTON2 = 0x0002;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public int type;
            public MOUSEINPUT mi;
            public KEYBDINPUT ki;
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        public static int HIWORD(uint value)
        {
            return (short)((value >> 16) & 0xFFFF);
        }
    }

    // Define our own MouseButtons enum for use in this file
    internal enum MouseButtons
    {
        None = 0,
        Left = 0x100000,
        Right = 0x200000,
        Middle = 0x400000,
        XButton1 = 0x800000,
        XButton2 = 0x1000000
    }
}