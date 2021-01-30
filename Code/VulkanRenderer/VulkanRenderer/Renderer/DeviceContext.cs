using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;


namespace VulkanRenderer
{
    public unsafe class DeviceContext
    {
		public bool m_enableLayers;
		public VkInstance m_vkInstance;
		public VkDebugReportCallbackEXT m_vkDebugCallback;

		public VkSurfaceKHR m_vkSurface;
		public PhysicalDeviceProperties[] m_physicalDevices;

		//
		//	Device related
		//
		public int m_deviceIndex;
		public VkPhysicalDevice m_vkPhysicalDevice;
		public VkDevice m_vkDevice;

		public int m_graphicsFamilyIdx;
		public int m_presentFamilyIdx;

		public VkQueue m_vkGraphicsQueue;
		public VkQueue m_vkPresentQueue;

		public SwapChain m_swapChain;

		public VkCommandPool m_vkCommandPool;

		public VkCommandBuffer[] m_vkCommandBuffers;


		public bool CreateDevice()
        {
			return true;
        }
		public bool CreatePhysicalDevice()
        {
			return true;

		}
		public bool CreateLogicalDevice()
        {
			return true;

		}




		public uint FindMemoryTypeIndex(uint typeFilter, VkMemoryPropertyFlags properties)
        {
			return 0;
        }

		//static const std::vector< const char* > m_deviceExtensions;
		//m_validationLayers;

		//
		//	Command Buffers
		//
		public bool CreateCommandBuffers()
        {
			return true;

		}



		public VkCommandBuffer CreateCommandBuffer(VkCommandBufferLevel level)
        {
			VkCommandBufferAllocateInfo allocInfo = new ();
			allocInfo.sType = VkStructureType.CommandBufferAllocateInfo;
			allocInfo.commandPool = m_vkCommandPool;
			allocInfo.level = VkCommandBufferLevel.Primary;
			allocInfo.commandBufferCount = 1;

			VkCommandBuffer cmdBuffer;
			vkAllocateCommandBuffers(m_vkDevice, &allocInfo, &cmdBuffer);

			//if (VK_SUCCESS != result)
			//{
			//	printf("ERROR: Failed to create command buffer\n");
			//	assert(0);
			//}

			// Start the command buffer
			VkCommandBufferBeginInfo beginInfo = new();
			beginInfo.sType = VkStructureType.CommandBufferBeginInfo;
			vkBeginCommandBuffer(cmdBuffer, &beginInfo);

			//if (VK_SUCCESS != result)
			//{
			//	printf("ERROR: Failed to begin command buffer\n");
			//	assert(0);
			//}
			return cmdBuffer;
		}

		//
		//	Swap chain related
		//

		public bool CreateSwapChain(int width, int height) { return m_swapChain.Create(this, width, height); }
		public void ResizeWindow(int width, int height) { m_swapChain.Resize(this, width, height); }

		public uint BeginFrame() { return m_swapChain.BeginFrame(this); }

		public void EndFrame() { m_swapChain.EndFrame(this); }

		public void BeginRenderPass() { m_swapChain.BeginRenderPass(this); }
		public void EndRenderPass() { m_swapChain.EndRenderPass(this); }


		//int GetAligendUniformByteOffset( const int offset ) const;
	}
}
