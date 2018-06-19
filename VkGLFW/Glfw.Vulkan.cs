using System;
using System.Runtime.InteropServices;
using System.Security;

namespace VkGLFW3
{
    public partial class Glfw
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "glfwGetInstanceProcAddress")]
        public static extern System.IntPtr GetInstanceProcAddress(IntPtr instance, [MarshalAs(UnmanagedType.LPStr)] string procname);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "glfwGetPhysicalDevicePresentationSupport")]
        public static extern int GetPhysicalDevicePresentationSupport(IntPtr instance, IntPtr device, uint queuefamily);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "glfwCreateWindowSurface")]
        /// Creates a window surface with the specified handles        
        public static extern VkResult CreateWindowSurface(IntPtr instance, IntPtr window, IntPtr allocator, Int64 surface);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "glfwCreateWindowSurface")]
        /// Creates a window surface with the specified handles
        public static extern VkResult CreateWindowSurface(UIntPtr instance, UIntPtr window, UIntPtr allocator, UInt64 surface);
    }
}