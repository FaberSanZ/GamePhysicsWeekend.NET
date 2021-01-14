using System;

namespace VulkanRenderer
{
    class Program
    {
        static void Main(string[] args)
        {
            using Application app = new();
            app.Initialize();
            app.MainLoop();
        }
    }
}
