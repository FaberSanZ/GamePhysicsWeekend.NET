using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulkanRenderer
{
    public unsafe sealed class Application : IDisposable
    {
        int F = 0;



        public void Initialize()
        {
        }

        public void MainLoop()
        {
        }


        void InitializeGLFW()
        {
        }
        bool InitializeVulkan()
        {
            return 0 == this.F;
        }
        void Cleanup()
        {
        }
        void UpdateUniforms()
        {
        }
        void DrawFrame()
        {
        }
        void ResizeWindow(int windowWidth, int windowHeight)
        {
        }
        void MouseMoved(float x, float y)
        {
        }
        void MouseScrolled(float z)
        {
        }
        void Keyboard(int key, int scancode, int action, int modifiers)
        {
        }

        static void OnWindowResized(IntPtr* window, int width, int height)
        {
        }
        static void OnMouseMoved(IntPtr* window, double x, double y)
        {
        }
        static void OnMouseWheelScrolled(IntPtr* window, double x, double y)
        {
        }
        static void OnKeyboard(IntPtr* window, int key, int scancode, int action, int modifiers)
        {
        }


        public void Dispose()
        {
        }
    }
}
