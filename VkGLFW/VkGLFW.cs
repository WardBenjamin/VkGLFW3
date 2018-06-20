using System;
using System.Runtime.InteropServices;
using System.Security;

namespace VkGLFW3
{
    public static class VkGlfw
    {
        /// <summary>
        /// Initializes the VkGLFW library.
        /// </summary>
        /// <remarks>
        /// This function initializes the VkGLFW and native GLFW library. Before most GLFW functions can be used, VkGLFW
        /// must be initialized, and before an application terminates VkGLFW should be terminated in order to free any
        /// resources allocated during or after initialization. If this function fails, it calls <see cref="Terminate"/>
        /// before returning. If it succeeds, you should call <see cref="Terminate"/> before the application exits.
        /// </remarks>
        public static bool Initialize()
        {
            var result = Init();
            return result == (int) State.True;
        }
        
        /// <summary>
        /// Checks if Vulkan is supported on the currently-running platform and device.
        /// </summary>
        public static bool VulkanSupported => Convert.ToBoolean(VulkanSupported_());

        /// <summary>
        /// Representation of the platform-specific Vulkan instance extensions required to interact with a window.
        /// This will always include VK_KHR_surface.
        /// </summary>
        public static unsafe string[] RequiredInstanceExtensions
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

        public static long CreateWindowSurface(IntPtr instance, Window window, IntPtr allocator)
        {
            return CreateWindowSurface(instance, window.Handle, allocator);
        }

        public static long CreateWindowSurface(IntPtr instance, IntPtr window, IntPtr allocator)
        {
            var status = CreateWindowSurface(instance, window, allocator, out long surface);

            if (status != VkResult.VK_SUCCESS)
            {
                throw new InvalidOperationException(
                    $"Creating window surface failed with status code: {(int) status}, {status.ToString()}");
            }

            return surface;
        }

        #region Native Bindings
        
        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl, EntryPoint="glfwInit")]
        public static extern int Init();

        /// <summary>
        /// Terminates the GLFW library. 
        /// </summary>
        /// <remarks>
        ///This function destroys all remaining windows and cursors and frees any other allocated resources. Once this
        /// function is called, you must again call <see cref="Init"/> successfully before you will be able to use most
        /// VkGLFW functions. If VkGLFW has been successfully initialized, this function should be called before the
        /// application exits. If initialization fails, there is no need to call this function, as it is called by
        /// <see cref="Init"/> before it returns failure.
        /// </remarks>
        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl, EntryPoint="glfwTerminate")]
        public static extern void Terminate();

        /// <summary>
        /// Create a Vulkan window surface with the specified handles.
        /// </summary>
        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwCreateWindowSurface")]
        private static extern VkResult CreateWindowSurface(IntPtr instance, IntPtr window, IntPtr allocator,
            out long surface);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwVulkanSupported")]
        private static extern int VulkanSupported_();

        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwGetRequiredInstanceExtensions")]
        private static extern unsafe sbyte** GetRequiredInstanceExtensions(out uint count);

        #endregion
    }
}