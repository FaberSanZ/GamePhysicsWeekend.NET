using System;
using System.Numerics;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using static Vortice.Vulkan.VkUtils;
using Vortice.Mathematics;

namespace VulkanRenderer
{
    public unsafe class SwapChain
    {
		public VkSemaphore m_vkImageAvailableSemaphore;
		public VkSemaphore m_vkRenderFinishedSemaphore;

		public int m_windowWidth;
		public int m_windowHeight;

		public VkSwapchainKHR m_vkSwapChain;
		public Size m_vkExtent;

		public uint m_currentImageIndex;
		public VkFormat m_vkColorImageFormat;
		public VkImage[] m_vkColorImages;
		public VkImageView[] m_vkImageViews;

		public VkFormat m_vkDepthFormat;
		public VkImage m_vkDepthImage;
		public VkImageView m_vkDepthImageView;
		public VkDeviceMemory m_vkDepthImageMemory;

		public VkFramebuffer[] m_vkFramebuffers;

		public VkRenderPass m_vkRenderPass;
        public SwapChain()
        {

        }


        public uint BeginFrame(DeviceContext device)
        {
			VkResult result;

			m_currentImageIndex = 0;

			result = vkAcquireNextImageKHR(device.m_vkDevice, m_vkSwapChain, ulong.MaxValue, m_vkImageAvailableSemaphore, VkFence.Null, out m_currentImageIndex);
			//if (VK_SUCCESS != result && VK_SUBOPTIMAL_KHR != result)
			//{
			//	printf("ERROR: Failed to acquire swap chain image\n");
			//	assert(0);
			//}

			// Reset the command buffer
			vkResetCommandBuffer(device.m_vkCommandBuffers[m_currentImageIndex], VkCommandBufferResetFlags.ReleaseResources);

			// Begin recording draw commands
			VkCommandBufferBeginInfo beginInfo = new();
			beginInfo.sType = VkStructureType.CommandBufferBeginInfo;
			beginInfo.flags = VkCommandBufferUsageFlags.SimultaneousUse;
			vkBeginCommandBuffer(device.m_vkCommandBuffers[m_currentImageIndex], &beginInfo);

			return m_currentImageIndex;
		}

        public void EndFrame(DeviceContext device)
        {
			VkResult result;

			result = vkEndCommandBuffer(device.m_vkCommandBuffers[m_currentImageIndex]);

			//if (VK_SUCCESS != result)
			//{
			//	printf("ERROR: Failed to record command buffer\n");
			//	assert(0);
			//}

			VkPipelineStageFlags* waitStages = stackalloc VkPipelineStageFlags[1] 
			{
				VkPipelineStageFlags.ColorAttachmentOutput
			};

			VkSubmitInfo submitInfo = new();
			submitInfo.sType = VkStructureType.SubmitInfo;
			submitInfo.waitSemaphoreCount = 1;

			var vkImageAvailableSemaphore = m_vkImageAvailableSemaphore;
			submitInfo.pWaitSemaphores = &vkImageAvailableSemaphore;

			submitInfo.pWaitDstStageMask = waitStages;
			submitInfo.commandBufferCount = 1;

			var cmd = device.m_vkCommandBuffers[m_currentImageIndex];
			submitInfo.pCommandBuffers = &cmd;

			submitInfo.signalSemaphoreCount = 1;

			var vkRenderFinishedSemaphore = m_vkRenderFinishedSemaphore;
			submitInfo.pSignalSemaphores = &vkRenderFinishedSemaphore;

			result = vkQueueSubmit(device.m_vkGraphicsQueue, 1, &submitInfo, VkFence.Null);

			//if (VK_SUCCESS != result)
			//{
			//	printf("ERROR: Failed to submit queue\n");
			//	assert(0);
			//}

			VkPresentInfoKHR presentInfo = new();
			presentInfo.sType = VkStructureType.PresentInfoKHR;
			presentInfo.waitSemaphoreCount = 1;
			presentInfo.pWaitSemaphores = &vkRenderFinishedSemaphore;
			presentInfo.swapchainCount = 1;

			var vkSwapChain = m_vkSwapChain;
			presentInfo.pSwapchains = &vkSwapChain;
			var currentImageIndex = m_currentImageIndex;
			presentInfo.pImageIndices = &currentImageIndex;

			result = vkQueuePresentKHR(device.m_vkPresentQueue, &presentInfo);
			//if (VK_SUCCESS != result && VK_SUBOPTIMAL_KHR != result)
			//{
			//	printf("ERROR: Failed to present\n");
			//	assert(0);
			//}

			vkQueueWaitIdle(device.m_vkPresentQueue);
		}

        public void BeginRenderPass(DeviceContext device)
        {
			//
			//	Set the renderpass
			//
			VkClearValue* clearValues = stackalloc VkClearValue[2];
			clearValues[0].color = new VkClearColorValue(0.0f, 0.0f, 0.0f, 1.0f);
			clearValues[1].depthStencil = new VkClearDepthStencilValue(1.0f, 0);

			VkRenderPassBeginInfo renderPassInfo = new();
			renderPassInfo.sType = VkStructureType.RenderPassBeginInfo;
			renderPassInfo.renderPass = m_vkRenderPass;
			renderPassInfo.framebuffer = m_vkFramebuffers[m_currentImageIndex];

			renderPassInfo.renderArea = new Rectangle(0,0, 1200, 720);
			renderPassInfo.clearValueCount = 2;
			renderPassInfo.pClearValues = clearValues;

			vkCmdBeginRenderPass(device.m_vkCommandBuffers[m_currentImageIndex], &renderPassInfo, VkSubpassContents.Inline);

			//
			//	Set the viewport
			//
			Viewport viewport = new Viewport(0, 0, m_windowWidth, m_windowHeight, 0, 1);

			vkCmdSetViewport(device.m_vkCommandBuffers[m_currentImageIndex], 0, 1, &viewport);

			Rectangle scissor = new Rectangle(0,0, m_windowWidth, m_windowHeight);

			vkCmdSetScissor(device.m_vkCommandBuffers[m_currentImageIndex], 0, 1, &scissor);
		}

        public void EndRenderPass(DeviceContext device)
        {
			vkCmdEndRenderPass(device.m_vkCommandBuffers[m_currentImageIndex]);
		}

        public bool Create(DeviceContext device, int width, int height)
        {
			m_windowWidth = width;
			m_windowHeight = height;

			m_vkExtent.Width = width;
			m_vkExtent.Height = height;

			//
			//	Create Semaphores
			//
			
				VkSemaphoreCreateInfo semaphoreInfo = new ();
				semaphoreInfo.sType = VkStructureType.SemaphoreCreateInfo;

				vkCreateSemaphore(device.m_vkDevice, &semaphoreInfo, null, out m_vkImageAvailableSemaphore);
				//if (VK_SUCCESS != result)
				//{
				//	printf("ERROR: Failed to create semaphores\n");
				//	assert(0);
				//	return false;
				

				vkCreateSemaphore(device.m_vkDevice, &semaphoreInfo, null, out m_vkRenderFinishedSemaphore);

				//if (VK_SUCCESS != result)
				//{
				//	printf("ERROR: Failed to create semaphores\n");
				//	assert(0);
				//	return false;
				//}
			

			//
			//	Create Swapchain
			//
			{
				 int deviceIndex = device.m_deviceIndex;
				PhysicalDeviceProperties physicalDeviceInfo = device.m_physicalDevices[deviceIndex];

				VkSurfaceFormatKHR surfaceFormat;
				surfaceFormat.format = VkFormat.B8G8R8A8UNorm;
				surfaceFormat.colorSpace = VkColorSpaceKHR.SrgbNonLinear;

				VkPresentModeKHR presentMode = VkPresentModeKHR.Fifo;
				for (int i = 0; i < physicalDeviceInfo.m_vkPresentModes.Length; i++)
				{
					if (VkPresentModeKHR.Mailbox == physicalDeviceInfo.m_vkPresentModes[i])
					{
						presentMode = VkPresentModeKHR.SharedContinuousRefresh;
						break;
					}
				}

				uint imageCount = physicalDeviceInfo.m_vkSurfaceCapabilities.minImageCount + 1;
				if (physicalDeviceInfo.m_vkSurfaceCapabilities.maxImageCount > 0 && imageCount > physicalDeviceInfo.m_vkSurfaceCapabilities.maxImageCount)
				{
					imageCount = physicalDeviceInfo.m_vkSurfaceCapabilities.maxImageCount;
				}

				uint* queueFamilyIndices = stackalloc uint[2]
				{
					(uint)device.m_graphicsFamilyIdx,
					(uint)device.m_presentFamilyIdx
				};

				VkSwapchainCreateInfoKHR createInfo = new();
				createInfo.sType = VkStructureType.SwapchainCreateInfoKHR;
				createInfo.surface = device.m_vkSurface;
				createInfo.minImageCount = imageCount;
				createInfo.imageFormat = surfaceFormat.format;
				createInfo.imageColorSpace = surfaceFormat.colorSpace;
				createInfo.imageExtent = m_vkExtent;
				createInfo.imageArrayLayers = 1;
				createInfo.imageUsage = VkImageUsageFlags.ColorAttachment;
				createInfo.queueFamilyIndexCount = 0;
				createInfo.pQueueFamilyIndices = null;
				createInfo.imageSharingMode = VkSharingMode.Exclusive;
				if (device.m_graphicsFamilyIdx != device.m_presentFamilyIdx)
				{
					createInfo.imageSharingMode = VkSharingMode.Concurrent;
					createInfo.queueFamilyIndexCount = 2;
					createInfo.pQueueFamilyIndices = queueFamilyIndices;
				}
				createInfo.preTransform = physicalDeviceInfo.m_vkSurfaceCapabilities.currentTransform;
				createInfo.compositeAlpha = VkCompositeAlphaFlagsKHR.Opaque;
				createInfo.presentMode = presentMode;
				createInfo.clipped = true;

				vkCreateSwapchainKHR(device.m_vkDevice, &createInfo, null, out m_vkSwapChain);

				//if (VK_SUCCESS != result)
				//{
				//	printf("ERROR: Failed to create swap chain\n");
				//	assert(0);
				//	return false;
				//}

				vkGetSwapchainImagesKHR(device.m_vkDevice, m_vkSwapChain, &imageCount, null);
				m_vkColorImages = new VkImage[imageCount];
				fixed(VkImage* ptr = &m_vkColorImages[0])
					vkGetSwapchainImagesKHR(device.m_vkDevice, m_vkSwapChain, &imageCount, ptr);

				m_vkColorImageFormat = surfaceFormat.format;
			}

			//
			//	Create Image Views for swap chain
			//
			{
				m_vkImageViews = new VkImageView[m_vkColorImages.Length];

				for (int i = 0; i < m_vkColorImages.Length; i++)
				{
					VkImageViewCreateInfo viewInfo = new();
					viewInfo.sType = VkStructureType.ImageViewCreateInfo;
					viewInfo.image = m_vkColorImages[i];
					viewInfo.viewType = VkImageViewType.Image2D;
					viewInfo.format = m_vkColorImageFormat;
					viewInfo.subresourceRange.aspectMask = VkImageAspectFlags.Color;
					viewInfo.subresourceRange.baseMipLevel = 0;
					viewInfo.subresourceRange.levelCount = 1;
					viewInfo.subresourceRange.baseArrayLayer = 0;
					viewInfo.subresourceRange.layerCount = 1;

					vkCreateImageView(device.m_vkDevice, &viewInfo, null, out m_vkImageViews[i]);
					//if (VK_SUCCESS != result)
					//{
					//	printf("ERROR: Failed to create texture image view\n");
					//	assert(0);
					//	return false;
					//}
				}
			}

			//
			//	Create Depth Image and Depth Image View for swap chain
			//
			{
				// Choose Depth Image Format
				m_vkDepthFormat = VkFormat.D24UNormS8UInt;
				{
					VkFormatProperties props;
					vkGetPhysicalDeviceFormatProperties(device.m_vkPhysicalDevice, VkFormat.D32SFloat, out props);
					if (0 != (props.optimalTilingFeatures & VkFormatFeatureFlags.DepthStencilAttachment))
					{
						m_vkDepthFormat = VkFormat.D32SFloat;
					}
				}

				VkImageCreateInfo imageInfo = new();
				imageInfo.sType = VkStructureType.ImageCreateInfo;
				imageInfo.imageType = VkImageType.Image2D;
				imageInfo.extent = new Size3(width, height, 1);
				imageInfo.mipLevels = 1;
				imageInfo.arrayLayers = 1;
				imageInfo.format = m_vkDepthFormat;
				imageInfo.tiling = VkImageTiling.Optimal;
				imageInfo.initialLayout = VkImageLayout.Undefined;
				imageInfo.usage = VkImageUsageFlags.DepthStencilAttachment;
				imageInfo.samples = VkSampleCountFlags.Count1;
				imageInfo.sharingMode = VkSharingMode.Exclusive;

				vkCreateImage(device.m_vkDevice, &imageInfo, null, out m_vkDepthImage);
				//if (VK_SUCCESS != result)
				//{
				//	printf("ERROR: Failed to create image\n");
				//	assert(0);
				//	return false;
				//}

				VkMemoryRequirements memRequirements;
				vkGetImageMemoryRequirements(device.m_vkDevice, m_vkDepthImage, out memRequirements);

				VkMemoryAllocateInfo allocInfo = new();
				allocInfo.sType = VkStructureType.MemoryAllocateInfo;
				allocInfo.allocationSize = memRequirements.size;
				allocInfo.memoryTypeIndex = device.FindMemoryTypeIndex(memRequirements.memoryTypeBits, VkMemoryPropertyFlags.DeviceLocal);

				 vkAllocateMemory(device.m_vkDevice, &allocInfo, null, out m_vkDepthImageMemory);
				//if (VK_SUCCESS != result)
				//{
				//	printf("ERROR: Failed to allocate image memory\n");
				//	assert(0);
				//	return false;
				//}

				vkBindImageMemory(device.m_vkDevice, m_vkDepthImage, m_vkDepthImageMemory, 0);

				//
				//	Create depth image view
				//
				VkImageViewCreateInfo viewInfo = new();
				viewInfo.sType = VkStructureType.ImageViewCreateInfo;
				viewInfo.image = m_vkDepthImage;
				viewInfo.viewType = VkImageViewType.Image2D;
				viewInfo.format = m_vkDepthFormat;
				viewInfo.subresourceRange.aspectMask = VkImageAspectFlags.Depth;
				viewInfo.subresourceRange.baseMipLevel = 0;
				viewInfo.subresourceRange.levelCount = 1;
				viewInfo.subresourceRange.baseArrayLayer = 0;
				viewInfo.subresourceRange.layerCount = 1;

				vkCreateImageView(device.m_vkDevice, &viewInfo, null, out m_vkDepthImageView);
				//if (VK_SUCCESS != result)
				//{
				//	printf("ERROR: Failed to create texture image view\n");
				//	assert(0);
				//	return false;
				//}
			}

			//
			//	Create the render pass for the swap chain
			//
			{
				VkAttachmentDescription colorAttachment = new();
				colorAttachment.format = m_vkColorImageFormat;
				colorAttachment.samples = VkSampleCountFlags.Count1;
				colorAttachment.loadOp = VkAttachmentLoadOp.Clear;
				colorAttachment.storeOp = VkAttachmentStoreOp.Store;
				colorAttachment.stencilLoadOp = VkAttachmentLoadOp.DontCare;
				colorAttachment.stencilStoreOp = VkAttachmentStoreOp.DontCare;
				colorAttachment.initialLayout = VkImageLayout.Undefined;
				colorAttachment.finalLayout = VkImageLayout.PresentSrcKHR;

				VkAttachmentDescription depthAttachment = new();
				depthAttachment.format = m_vkDepthFormat;
				depthAttachment.samples = VkSampleCountFlags.Count1;
				depthAttachment.loadOp = VkAttachmentLoadOp.Clear;
				depthAttachment.storeOp = VkAttachmentStoreOp.DontCare;
				depthAttachment.stencilLoadOp = VkAttachmentLoadOp.DontCare;
				depthAttachment.stencilStoreOp = VkAttachmentStoreOp.DontCare;
				depthAttachment.initialLayout = VkImageLayout.Undefined;
				depthAttachment.finalLayout = VkImageLayout.DepthAttachmentOptimal;

				VkAttachmentReference colorAttachmentRef = new();
				colorAttachmentRef.attachment = 0;
				colorAttachmentRef.layout = VkImageLayout.ColorAttachmentOptimal;

				VkAttachmentReference depthAttachmentRef = new();
				depthAttachmentRef.attachment = 1;
				depthAttachmentRef.layout = VkImageLayout.DepthStencilAttachmentOptimal;

				VkSubpassDescription subpass = new();
				subpass.pipelineBindPoint = VkPipelineBindPoint.Graphics;
				subpass.colorAttachmentCount = 1;
				subpass.pColorAttachments = &colorAttachmentRef;
				subpass.pDepthStencilAttachment = &depthAttachmentRef;

				VkSubpassDependency dependency = new();
				dependency.srcSubpass = SubpassExternal;
				dependency.dstSubpass = 0;
				dependency.srcStageMask = VkPipelineStageFlags.ColorAttachmentOutput;
				dependency.srcAccessMask = 0;
				dependency.dstStageMask = VkPipelineStageFlags.ColorAttachmentOutput;
				dependency.dstAccessMask = VkAccessFlags.ColorAttachmentRead | VkAccessFlags.ColorAttachmentWrite;

				VkAttachmentDescription* attachments = stackalloc VkAttachmentDescription[2]
				{
					colorAttachment,
					depthAttachment
				};

				VkRenderPassCreateInfo renderPassInfo = new();
				renderPassInfo.sType = VkStructureType.RenderPassCreateInfo;
				renderPassInfo.attachmentCount = 2;
				renderPassInfo.pAttachments = attachments;
				renderPassInfo.subpassCount = 1;
				renderPassInfo.pSubpasses = &subpass;
				renderPassInfo.dependencyCount = 1;
				renderPassInfo.pDependencies = &dependency;

				vkCreateRenderPass(device.m_vkDevice, &renderPassInfo, null, out m_vkRenderPass);
				//if (VK_SUCCESS != result)
				//{
				//	printf("ERROR: Failed to create the render pass\n");
				//	assert(0);
				//	return false;
				//}
			}

			//
			//	Create Frame Buffers for SwapChain
			//
			{
				m_vkFramebuffers = new VkFramebuffer[m_vkImageViews.Length];

				for (int i = 0; i < m_vkImageViews.Length; i++)
				{
					VkImageView* attachments = stackalloc VkImageView[2]
					{
						m_vkImageViews[i],
						m_vkDepthImageView
					};

					VkFramebufferCreateInfo framebufferInfo = new();
					framebufferInfo.sType = VkStructureType.FramebufferCreateInfo;
					framebufferInfo.renderPass = m_vkRenderPass;
					framebufferInfo.attachmentCount = 2;
					framebufferInfo.pAttachments = attachments;
					framebufferInfo.width = (uint)m_vkExtent.Width;
					framebufferInfo.height = (uint)m_vkExtent.Height;
					framebufferInfo.layers = 1;

					vkCreateFramebuffer(device.m_vkDevice, &framebufferInfo, null, out m_vkFramebuffers[i]);

					//if (VK_SUCCESS != result)
					//{
					//	printf("ERROR: Failed to create the frame buffer\n");
					//	assert(0);
					//	return false;
					//}
				}
			}

			return true;
		}

        public void Resize(DeviceContext deviceContext, int width, int height)
        {
            throw new NotImplementedException();
        }
    }
}