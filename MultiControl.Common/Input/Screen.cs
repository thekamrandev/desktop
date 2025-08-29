using System;

namespace MultiControl.Common.Input
{
    /// <summary>
    /// Represents a display device or multiple display devices on a single system.
    /// </summary>
    public class Screen
    {
        /// <summary>
        /// Gets the bounds of the display.
        /// </summary>
        public Rectangle Bounds { get; private set; }

        /// <summary>
        /// Gets the primary display.
        /// </summary>
        public static Screen PrimaryScreen
        {
            get
            {
                // Get the primary screen dimensions using P/Invoke
                int width = GetSystemMetrics(SM_CXSCREEN);
                int height = GetSystemMetrics(SM_CYSCREEN);
                
                return new Screen
                {
                    Bounds = new Rectangle(0, 0, width, height)
                };
            }
        }

        // System metric constants
        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);
    }

    /// <summary>
    /// Represents a rectangle structure with position and size.
    /// </summary>
    public struct Rectangle
    {
        /// <summary>
        /// Gets the x-coordinate of the left edge of this Rectangle structure.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets the y-coordinate of the top edge of this Rectangle structure.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets the width of this Rectangle structure.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets the height of this Rectangle structure.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Initializes a new instance of the Rectangle structure with the specified location and size.
        /// </summary>
        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}