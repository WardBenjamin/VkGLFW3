using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace VkGLFW3
{
    /// <summary>
    /// Event args for the size changed event. 
    /// </summary>
    public struct SizeChangedEventArgs { 
        /// <summary>
        /// The event source.
        /// </summary>
        public Window Source; 
        /// <summary>
        /// The new width.
        /// </summary>
        public int Width;
        /// <summary>
        /// The new height.
        /// </summary>
        public int Height; 
    };

    /// <summary>  This is the event args for the keyboard key event.</summary>
    public struct KeyEventArgs {

        /// <summary>
        /// The window that received the event.
        /// </summary>
        public Window Source;

        /// <summary>
        /// The keyboard key
        /// </summary>
        public Key Key;

        /// <summary>
        /// The system-specific scancode of the key
        /// </summary>
        public int Scancode;

        /// <summary>
        /// `Pressed`, `Released` or `Repeated`.
        /// </summary>
        public KeyAction Action;

        /// <summary>
        /// Bit field describing which modifier keys. Use KeyModifiers for extracting the bits.
        /// </summary>
        public List<KeyModifier> Modifiers;

        internal KeyEventArgs(Window source, Key key, int scancode, KeyAction action, int modifiers)
        {
            Source = source;
            Key = key;
            Scancode = scancode;
            Action = action;
            Modifiers = GetKeyModifiers(modifiers);
        }
        
        private static readonly KeyModifier[] KeyModifiers = Enum.GetValues(typeof(KeyModifier)).Cast<KeyModifier>().ToArray();

        private static List<KeyModifier> GetKeyModifiers(int mods)
        {
            return KeyModifiers.Where(key => (mods & (int) key) == (int) key).ToList();
        }
    }

    public struct ImageDescriptor
    {
        public int Width;
        public int Height;
        public byte[] Pixels;
    }

    internal struct ImageDescriptor_Internal
    {
        internal int Width;
        internal int Height;
        internal IntPtr Pixels;

        internal static ImageDescriptor_Internal MarshalPixels(ImageDescriptor image)
        {
            int size = image.Width * image.Height * 4; // 4 bytes (32 bits) per pixel - R8B8G8A8 or equivalent

            var desc = new ImageDescriptor_Internal()
            {
                Width = image.Width,
                Height = image.Height,
                Pixels = Marshal.AllocHGlobal(size)
            };

            Marshal.Copy(image.Pixels, 0, desc.Pixels, Math.Min(size, image.Pixels.Length));

            return desc;
        }

        internal void FreePixels() => Marshal.FreeHGlobal(Pixels);
    }
}