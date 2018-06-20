using System;
using System.Runtime.InteropServices.WindowsRuntime;
using VkGLFW3;

namespace VkGLFW_Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            VkGlfw.Init();
            var window = new Window(800, 600, "VkGLFW3 Demo");

            Console.WriteLine("Window size: {0}", window.GetSize());
            Console.WriteLine("Window title: {0}", window.Title);
            
            Console.WriteLine("Vulkan supported: {0}", VkGlfw.VulkanSupported);
            Console.WriteLine("Required Vulkan instance extensions: {0}", string.Join(", ", VkGlfw.RequiredInstanceExtensions));

            window.Title = "Test";
            
            while (!window.ShouldClose)
            {
                window.PollEvents();
            }

            window.Dispose();
            VkGlfw.Terminate();
        }
    }
}