# VkGLFW3 0.3.0

Vulkan-focused C# 7 (.NET Standard 2.0) object-oriented window/input system based on GLFW3

The aim of this project is to provide minimal, abstracted GLFW3 bindings usable with Vulkan, specifically designed for good interop with [VulkanCore](https://github.com/discosultan/VulkanCore). The focus here is to abstract away as many native GLFW calls as possible while not detracting from the speed advantages and familiarities of the library. As such, most functionality is wrapped by the `Window` class, with Vulkan-specific functionality and `Init` and `Terminate` calls being the only exceptions.

This library makes extensive use of C# 7 features, and targets .NET Standard 2.0, compatible with .NET Core 2.0+, .NET Framework 4.6.1+, and
Mono 5.4+ runtimes as per the [.NET Implementation Support Table](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)

This library only supports x64 platforms! Windows is currently fully supported (Linux and OSX should work but are untested). Make sure to copy the relevent `glfw` library file to your output directory - a common way to do so is by including the following line in your console application CSproj file:

```
<Content Include="glfw3.dll" CopyToOutputDirectory="PreserveNewest" Visible="true" />
```


Documentation is available on [Github Pages](https://wardbenjamin.github.io/VkGLFW3/annotated.html), but here's a small minimal feature example:

```cs
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
```

MIT License (see LICENSE.md)

Copyright (c) 2018 Benjamin Ward
