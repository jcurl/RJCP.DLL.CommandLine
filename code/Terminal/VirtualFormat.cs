namespace RJCP.Core.Terminal
{
    using System;
    using Resources;

    /// <summary>
    /// A formatter class for a virtual console.
    /// </summary>
    internal class VirtualFormat : Format
    {
        private int m_Width = 80;

        /// <summary>
        /// Gets the width of the terminal.
        /// </summary>
        /// <value>The width of the terminal.</value>
        public override int Width
        {
            get { return m_Width; }
        }

        /// <summary>
        /// Sets the width of the terminal.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="width"/> must be positive.</exception>
        public void SetWidth(int width)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), CmdLineStrings.TerminalVirtualWidthRange);
            m_Width = width;
        }

        private int m_Height = 25;

        /// <summary>
        /// Gets the height of the terminal.
        /// </summary>
        /// <value>The height of the terminal.</value>
        public override int Height
        {
            get { return m_Height; }
        }

        /// <summary>
        /// Sets the height.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="height"/> must be positive.</exception>
        public void SetHeight(int height)
        {
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height), CmdLineStrings.TerminalVirtualHeightRange);
            m_Height = height;
        }
    }
}
