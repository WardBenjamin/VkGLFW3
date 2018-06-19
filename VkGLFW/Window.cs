using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace VkGLFW3
{
    /// <summary>
    /// Represents a native window handle and acts as the main entry point for all GLFW
    /// functionality, excluding Init and Terminate.
    /// </summary>
    public class Window : IDisposable
    {
        public IntPtr Handle;
        
        /// <summary>
        /// Indicated whether the window should be closed.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool ShouldClose => Convert.ToBoolean(WindowShouldClose(Handle));

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                SetWindowTitle(Handle, value);
            }
        }
        
        private string _title;
        
        private readonly GlfwWindowSizeFun _sizeChangedCallback;
        private readonly GlfwKeyFun _keyPressedCallback;

        /// <summary>
        /// Will be called if the Size of the window has been changed
        /// </summary>
        public event EventHandler<SizeChangedEventArgs> SizeChanged;

        /// <summary>
        /// Will be called if a key has been pressed
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyChanged;
        
        /// <summary>
        /// Creates a window meant to be used with Vulkan (aka there is no associated OpenGL context) and
        /// initialize event callbacks.
        /// </summary>
        /// <param name="initialWidth">Initial window width</param>
        /// <param name="initialHeight">Initial window height</param>
        /// <param name="title">Window title</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Window (int initialWidth, int initialHeight, string title)
        {
            WindowHint((int) State.ClientApi, (int) State.NoApi);

            Handle = CreateWindow(initialWidth, initialHeight, title, IntPtr.Zero, IntPtr.Zero);
            _title = title;
            
            if (Handle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Window creation failed.");
            }

            _sizeChangedCallback = (_, width, height) =>
            {
                SizeChanged?.Invoke(this, new SizeChangedEventArgs {Source = this, Width = width, Height = height});
            };
            SetWindowSizeCallback(_sizeChangedCallback);

            _keyPressedCallback = (_, key, scancode, action, mods) =>
            {
                var args = new KeyEventArgs(
                    this,
                    (Key) Enum.Parse(typeof(Key), key.ToString()),
                    scancode,
                    (KeyAction) Enum.Parse(typeof(KeyAction), action.ToString()),
                    mods
                );
                KeyChanged?.Invoke(this, args);
            };
            SetKeyCallback(_keyPressedCallback);

            // Add dummy handlers to prevent any null reference exceptions
            SizeChanged = (__, _) => { };
            KeyChanged = (__, _) => { };
        }

        /// <summary>
        /// This function makes the specified window visible if it was previously hidden. If the window is already
        /// visible or is in full screen mode, this function does nothing.
        /// </summary>
        public void Show() => ShowWindow(Handle);

        /// <summary>
        /// This function hides the specified window if it was previously visible. If the window is already hidden or
        /// is in full screen mode, this function does nothing.
        /// </summary>
        public void Hide() => HideWindow(Handle);

        /// <summary>
        /// Not implemented. Indicates whether the specified window is visible. 
        /// </summary>
        public bool Visible() => false;

        /// <summary>
        /// Not implemented. Indicates whether the specified window has decorations such as a border, a close widget, etc.
        /// </summary>
        public bool Decorated() => false;

        /// <summary>
        /// Not implemented. Indicates whether the specified window is resizable by the user.
        /// </summary>
        /// <returns></returns>
        public bool Resizable() => false;

        /// <summary>
        /// Not implemented. Indicates whether the specified window has input focus.
        /// </summary>
        public bool Focused() => false;

        /// <summary>
        /// Not implemented. Indicates whether the specified window is iconified (either manually by the user or via window.Minimize()).
        /// </summary>
        public bool Minimized() => false; // "Iconified"

        /// <summary>
        /// Processes all pending events.
        /// </summary>
        /// <remarks>
        /// This function processes only those events that are already in the event queue and then returns immediately.
        /// Processing events will cause the window and input callbacks associated with those events to be called.
        /// On some platforms, a window move, resize or menu operation will cause event processing to block.
        /// On some platforms, certain events are sent directly to the application without going through the event
        /// queue, causing callbacks to be called outside of a call to one of the event processing functions.
        /// </remarks>
        public void PollEvents()
        {
            PollEvents_();
        }

        /// <summary>
        /// Retrieves the size of the client area of the specified window.
        /// </summary>
        /// <remarks>
        /// This function retrieves the size, in screen coordinates, of the client area of the specified window.
        /// </remarks>
        public (int width, int height) GetSize()
        {
            int width = 0, height = 0;
            unsafe
            {
                GetWindowSize(Handle, &width, &height);
            }

            return (width, height);
        }

        /// <summary>
        /// Not implemented. Sets the size of the client area of the specified window. 
        /// </summary>
        /// <remarks>
        /// This function sets the size, in screen coordinates, of the client area of the specified window.
        /// For full screen windows, this function updates the resolution of its desired video mode and switches to
        /// the video mode closest to it, without affecting the window's context. As the context is unaffected, the
        /// bit depths of the framebuffer remain unchanged.
        /// </remarks>
        /// <param name="width">Width, in screen coordinates</param>
        /// <param name="height">Height, in screen coordinates</param>
        public void SetSize(int width, int height)
        {
        }

        /// <summary>
        /// Set the icon for the window.
        /// </summary>
        /// <remarks>
        /// This function sets the icon of the specified window. If no image (null) is specified, the window reverts
        /// to its default icon.
        /// </remarks>
        public unsafe void SetIcon(ImageDescriptor? image)
        {
            if (image == null)
            {
                SetWindowIcon(Handle, 1, IntPtr.Zero);
                return;
            }

            var images = new[] {ImageDescriptor_Internal.MarshalPixels(image.Value)};

            IntPtr ptr;
            fixed (ImageDescriptor_Internal* array = images)
                ptr = new IntPtr((void*) array);

            SetWindowIcon(Handle, 1, ptr);

            images[0].FreePixels();
        }

        private GlfwWindowSizeFun SetWindowSizeCallback(GlfwWindowSizeFun func)
        {
            ValidateHandle();
            var funcPtr = Marshal.GetFunctionPointerForDelegate(func);
            var result = SetWindowSizeCallback_(Handle, funcPtr);
            if (result == IntPtr.Zero)
                return null;
            return (GlfwWindowSizeFun) Marshal.GetDelegateForFunctionPointer(result, typeof(GlfwWindowSizeFun));
        }

        private GlfwKeyFun SetKeyCallback(GlfwKeyFun func)
        {
            ValidateHandle();
            var funcPtr = Marshal.GetFunctionPointerForDelegate(func);
            var result = SetKeyCallback_(Handle, funcPtr);
            if (result == IntPtr.Zero)
                return null;
            return (GlfwKeyFun) Marshal.GetDelegateForFunctionPointer(result, typeof(GlfwKeyFun));
        }

        private void ValidateHandle()
        {
            if (Handle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Window handle is invalid.");
            }
        }

        #region Vulkan Support

        public bool VulkanSupported => Convert.ToBoolean(VulkanSupported_());

        public unsafe string[] RequiredInstanceExtensions
        {
            get
            {
                // We get to do some fun unsafe memory manipulation to manually marshal the returned strings
                sbyte** nativeArr = GetRequiredInstanceExtensions(out uint count);

                string[] extensions = new string[count];

                for (int i = 0; i < count; i++)
                {
                    sbyte* value = nativeArr[i];
                    extensions[i] = new string(value);
                }

                return extensions;
            }
        }

        public VkResult CreateWindowSurface(IntPtr instance, IntPtr allocator)
        {
            return VkResult.VK_NOT_READY;
        }

        #endregion

        #region Native Bindings

        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwWindowHint")]
        internal static extern void WindowHint(int hint, int value);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwCreateWindow")]
        private static extern IntPtr CreateWindow(int width, int height, [MarshalAs(UnmanagedType.LPStr)] string title,
            IntPtr monitor, IntPtr share);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwDestroyWindow")]
        private static extern void DestroyWindow(IntPtr window);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwWindowShouldClose")]
        private static extern int WindowShouldClose(IntPtr window);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwShowWindow")]
        private static extern void ShowWindow(IntPtr window);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwHideWindow")]
        private static extern void HideWindow(IntPtr window);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwGetWindowSize")]
        private static extern unsafe void GetWindowSize(IntPtr window, int* width, int* height);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwSetWindowIcon")]
        private static extern void SetWindowIcon(IntPtr window, int count, IntPtr images);
        
        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl, EntryPoint="glfwSetWindowTitle")]
        private static extern void SetWindowTitle(IntPtr window, [MarshalAs(UnmanagedType.LPStr)] string title);


        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwPollEvents")]
        private static extern void PollEvents_();

        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwSetWindowSizeCallback")]
        private static extern IntPtr SetWindowSizeCallback_(IntPtr window, IntPtr cbfun);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwSetKeyCallback")]
        private static extern IntPtr SetKeyCallback_(IntPtr window, IntPtr cbfun);

        /// <summary>  This is the function signature for window position callback functions.</summary>
        /// <param name="window">The window that was moved.</param>
        /// <param name="x"> The new x-coordinate, in screen coordinates, of the upper-left corner of the client area of the window. </param>
        /// <param name="y"> The new y-coordinate, in screen coordinates, of the upper-left corner of the client area of the window. </param>
        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void GlfwWindowPosFun(IntPtr window, int x, int y);

        /// <summary>  This is the function signature for window size callback functions.</summary>
        /// <param name="window">The window that was resized.</param>
        /// <param name="width">The new width, in screen coordinates, of the window.</param>
        /// <param name="height">The new height, in screen coordinates, of the window.</param>
        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void GlfwWindowSizeFun(IntPtr window, int width, int height);

        /// <summary>  This is the function signature for keyboard key callback functions.</summary>
        /// <param name="window">The window that received the event.</param>
        /// <param name="key">The [keyboard key](</param>
        /// <param name="scancode">The system-specific scancode of the key.</param>
        /// <param name="action">`Pressed`, `Released` or `Repeated`.</param>
        /// <param name="mods"> Bit field describing which modifier keys are held down. </param>
        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void GlfwKeyFun(IntPtr window, int key, int scancode, int action, int mods);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "glfwGetPhysicalDevicePresentationSupport")]
        public static extern int GetPhysicalDevicePresentationSupport(IntPtr instance, IntPtr device, uint queuefamily);

        /// <summary>
        /// Create a Vulkan window surface with the specified handles.
        /// </summary>
        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "glfwCreateWindowSurface")]
        private static extern VkResult CreateWindowSurface(IntPtr instance, IntPtr window, IntPtr allocator,
            Int64 surface);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwVulkanSupported")]
        private static extern int VulkanSupported_();

        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "glfwGetRequiredInstanceExtensions", CharSet = CharSet.Ansi)]
        private static extern unsafe sbyte** GetRequiredInstanceExtensions(out uint count);

        #endregion

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                }

                DestroyWindow(Handle);
                _disposedValue = true;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Destroys the specified window and its context. 
        /// </summary>
        /// <remarks>
        /// This function destroys the specified window and its context. On calling this function, no further callbacks
        /// will be called for that window.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion // Disposable
    }
}