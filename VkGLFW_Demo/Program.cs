using System;
using System.Runtime.InteropServices.WindowsRuntime;
using VkGLFW3;

namespace VkGLFW_Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Glfw.Init();
            var window = Window.CreateVulkan(800, 600, "VkGLFW3 Demo");

            Console.WriteLine(window.GetSize());
            
            while (!window.ShouldClose)
            {
                window.PollEvents();
            }

            window.Dispose();
            Glfw.Terminate();
        }
    }
}