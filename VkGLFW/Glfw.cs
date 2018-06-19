using System.Runtime.InteropServices;
using System.Security;

namespace VkGLFW3
{
    public partial class Glfw
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl, EntryPoint="glfwInit")]
        public static extern int Init();

        [SuppressUnmanagedCodeSecurity]
        [DllImport("glfw3", CallingConvention = CallingConvention.Cdecl, EntryPoint="glfwTerminate")]
        public static extern void Terminate();
    }
}