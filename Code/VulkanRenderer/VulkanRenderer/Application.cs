using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Silk.NET.GLFW;

namespace VulkanRenderer
{
    public unsafe class Application : IDisposable
    {
        //GLFWwindow* m_glfwWindow;

        //DeviceContext m_deviceContext;

        ////
        ////	Uniform Buffer
        ////
        //Buffer m_uniformBuffer;

        ////
        ////	Model
        ////
        //Model m_modelFullScreen;
        //std::vector<Model*> m_models;   // models for the bodies

        ////
        ////	Pipeline for copying the offscreen framebuffer to the swapchain
        ////
        //Shader m_copyShader;
        //Descriptors m_copyDescriptors;
        //Pipeline m_copyPipeline;

        // User input
        internal Vector2 m_mousePosition;
        internal Vector3 m_cameraFocusPoint;
        internal float m_cameraPositionTheta;
        internal float m_cameraPositionPhi;
        internal float m_cameraRadius;
        internal bool m_isPaused;
        internal bool m_stepFrame;

        //List<RenderModel> m_renderModels;

        internal int WINDOW_WIDTH = 1200;
        internal int WINDOW_HEIGHT = 720;

        internal bool m_enableLayers = true;


        public void Initialize()
        {
            InitializeGLFW();
        }

        int GetTimeMicroseconds()
        {
            //if (false == gIsInitialized)
            //{
            //    gIsInitialized = true;

            //    // Get the high frequency counter's resolution
            //    QueryPerformanceFrequency((LARGE_INTEGER*)&gTicksPerSecond);

            //    // Get the current time
            //    QueryPerformanceCounter((LARGE_INTEGER*)&gStartTicks);

            //    return 0;
            //}

            //unsigned __int64 tick;
            //QueryPerformanceCounter((LARGE_INTEGER*)&tick);

            //const double ticks_per_micro = (double)(gTicksPerSecond / 1000000);

            //const unsigned __int64 timeMicro = (unsigned __int64)((double)(tick - gStartTicks) / ticks_per_micro);
            //return (int)timeMicro;

            return 1;
        }

        public void MainLoop()
        {
            int timeLastFrame = 0;
            int numSamples = 0;
            float avgTime = 0.0f;
            float maxTime = 0.0f;

            while (!glfw.WindowShouldClose(m_glfwWindow))
            {
                int time = GetTimeMicroseconds();
                float dt_us = (float)time - (float)timeLastFrame;
                //if (dt_us < 16000.0f)
                //{
                //    int x = 16000 - (int)dt_us;
                //    std::this_thread::sleep_for(std::chrono::microseconds(x));
                //    System.Diagnostics.th
                //    dt_us = 16000;
                //    time = GetTimeMicroseconds();
                //}
                timeLastFrame = time;
                Console.WriteLine("\ndt_ms: %.1f    ", dt_us * 0.001f);

                // Get User Input
                glfw.PollEvents();

                // If the time is greater than 33ms (30fps)
                // then force the time difference to smaller
                // to prevent super large simulation steps.
                if (dt_us > 33000.0f)
                {
                    dt_us = 33000.0f;
                }

                bool runPhysics = true;
                if (m_isPaused)
                {
                    dt_us = 0.0f;
                    runPhysics = false;
                    if (m_stepFrame)
                    {
                        dt_us = 16667.0f;
                        m_stepFrame = false;
                        runPhysics = true;
                    }
                    numSamples = 0;
                    maxTime = 0.0f;
                }
                float dt_sec = dt_us * 0.001f * 0.001f;

                // Run Update
                if (runPhysics)
                {
                    int startTime = GetTimeMicroseconds();
                    for (int i = 0; i < 2; i++)
                    {
                        //m_scene.Update(dt_sec * 0.5f);
                    }
                    int endTime = GetTimeMicroseconds();

                    dt_us = (float)endTime - (float)startTime;
                    if (dt_us > maxTime)
                    {
                        maxTime = dt_us;
                    }

                    avgTime = (avgTime * (float)numSamples + dt_us) / (float)numSamples + 1;
                    numSamples++;

                    Console.WriteLine("frame dt_ms: %.2f %.2f %.2f", avgTime * 0.001f, maxTime * 0.001f, dt_us * 0.001f);
                }

                // Draw the Scene
                DrawFrame();
            }
        }

        private readonly Glfw glfw = GlfwProvider.GLFW.Value;
        internal WindowHandle* m_glfwWindow;
        internal void InitializeGLFW()
        {
            glfw.Init();

            glfw.WindowHint(WindowHintBool.Visible, false);


            m_glfwWindow = glfw.CreateWindow(WINDOW_WIDTH, WINDOW_HEIGHT, "GamePhysicsWeekend", (Monitor*)IntPtr.Zero.ToPointer(), null);

            //glfw.SetWindowUserPointer(m_glfwWindow, this);
            //glfw.SetWindowSizeCallback(m_glfwWindow, Application::OnWindowResized);

            //glfwSetInputMode(m_glfwWindow, GLFW_CURSOR, GLFW_CURSOR_DISABLED);
            //glfwSetInputMode(m_glfwWindow, GLFW_STICKY_KEYS, GLFW_TRUE);
            //glfwSetCursorPosCallback(m_glfwWindow, Application::OnMouseMoved);
            //glfwSetScrollCallback(m_glfwWindow, Application::OnMouseWheelScrolled);
            //glfwSetKeyCallback(m_glfwWindow, Application::OnKeyboard);


            glfw.ShowWindow(m_glfwWindow);
        }
        internal bool InitializeVulkan()
        {
            return true;
        }

        internal void UpdateUniforms()
        {
        }
        internal void DrawFrame()
        {
        }
        internal void ResizeWindow(int windowWidth, int windowHeight)
        {
        }
        internal void MouseMoved(float x, float y)
        {
        }
        internal void MouseScrolled(float z)
        {
        }
        internal void Keyboard(int key, int scancode, int action, int modifiers)
        {
        }

        internal void OnWindowResized(IntPtr* window, int width, int height)
        {
        }
        internal void OnMouseMoved(IntPtr* window, double x, double y)
        {
        }
        internal void OnMouseWheelScrolled(IntPtr* window, double x, double y)
        {
        }
        internal void OnKeyboard(IntPtr* window, int key, int scancode, int action, int modifiers)
        {
        }


        public void Dispose()
        {
        }
    }
}
