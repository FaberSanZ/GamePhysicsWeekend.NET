using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;


namespace VulkanRenderer
{
    public unsafe class PhysicalDeviceProperties
    {
		public VkPhysicalDevice m_vkPhysicalDevice;
		public VkPhysicalDeviceProperties m_vkDeviceProperties;
		public VkPhysicalDeviceMemoryProperties m_vkMemoryProperties;
		public VkPhysicalDeviceFeatures m_vkFeatures;
		public VkSurfaceCapabilitiesKHR m_vkSurfaceCapabilities;
		public VkSurfaceFormatKHR[] m_vkSurfaceFormats;
		public VkPresentModeKHR[] m_vkPresentModes;
		public VkQueueFamilyProperties[] m_vkQueueFamilyProperties;
		public VkExtensionProperties[] m_vkExtensionProperties;








		public bool AcquireProperties(VkPhysicalDevice device, VkSurfaceKHR vkSurface)
		{
			VkResult result;

			m_vkPhysicalDevice = device;

			vkGetPhysicalDeviceProperties(m_vkPhysicalDevice, out m_vkDeviceProperties);
			vkGetPhysicalDeviceMemoryProperties(m_vkPhysicalDevice, out m_vkMemoryProperties);
			vkGetPhysicalDeviceFeatures(m_vkPhysicalDevice, out m_vkFeatures);

			// VkSurfaceCapabilitiesKHR
			result = vkGetPhysicalDeviceSurfaceCapabilitiesKHR(m_vkPhysicalDevice, vkSurface, out m_vkSurfaceCapabilities);

			//if (VK_SUCCESS != result)
			//{
			//	printf("ERROR: Failed to vkGetPhysicalDeviceSurfaceCapabilitiesKHR\n");
			//	assert(0);
			//	return false;
			//}

			// VkSurfaceFormatKHR
			{
				uint numFormats;
				result = vkGetPhysicalDeviceSurfaceFormatsKHR(m_vkPhysicalDevice, vkSurface, &numFormats, null);
				//if (VK_SUCCESS != result || 0 == numFormats)
				//{
				//	printf("ERROR: Failed to vkGetPhysicalDeviceSurfaceFormatsKHR\n");
				//	assert(0);
				//	return false;
				//}

				m_vkSurfaceFormats = new VkSurfaceFormatKHR[numFormats];
				fixed(VkSurfaceFormatKHR* ptr = m_vkSurfaceFormats)
					vkGetPhysicalDeviceSurfaceFormatsKHR(m_vkPhysicalDevice, vkSurface, &numFormats, ptr);

				//if (VK_SUCCESS != result || 0 == numFormats)
				//{
				//	printf("ERROR: Failed to vkGetPhysicalDeviceSurfaceFormatsKHR\n");
				//	assert(0);
				//	return false;
				//}
			}

			// VkPresentModeKHR
			{
				uint numPresentModes;
				result = vkGetPhysicalDeviceSurfacePresentModesKHR(m_vkPhysicalDevice, vkSurface, &numPresentModes, null);

				//if (VK_SUCCESS != result || 0 == numPresentModes)
				//{
				//	printf("ERROR: Failed to vkGetPhysicalDeviceSurfacePresentModesKHR\n");
				//	assert(0);
				//	return false;
				//}

				m_vkPresentModes = new VkPresentModeKHR[numPresentModes];
				fixed(VkPresentModeKHR* ptr = m_vkPresentModes)
					vkGetPhysicalDeviceSurfacePresentModesKHR(m_vkPhysicalDevice, vkSurface, &numPresentModes, ptr);

				//if (VK_SUCCESS != result || 0 == numPresentModes)
				//{
				//	printf("ERROR: Failed to vkGetPhysicalDeviceSurfacePresentModesKHR\n");
				//	assert(0);
				//	return false;
				//}
			}

			// VkQueueFamilyProperties
			{
				uint numQueues = 0;
				vkGetPhysicalDeviceQueueFamilyProperties(m_vkPhysicalDevice, &numQueues, null);

				if (0 == numQueues)
				{
					//printf("ERROR: Failed to vkGetPhysicalDeviceQueueFamilyProperties\n");
					//assert(0);
					return false;
				}

				m_vkQueueFamilyProperties = new VkQueueFamilyProperties[numQueues];
				fixed(VkQueueFamilyProperties* ptr = m_vkQueueFamilyProperties)
					vkGetPhysicalDeviceQueueFamilyProperties(m_vkPhysicalDevice, &numQueues, ptr);

				if (0 == numQueues)
				{
					//printf("ERROR: Failed to vkGetPhysicalDeviceQueueFamilyProperties\n");
					//assert(0);
					return false;
				}
			}

			// VkExtensionProperties
			{
				uint numExtensions;
				result = vkEnumerateDeviceExtensionProperties(m_vkPhysicalDevice, null, &numExtensions, null);

				//if (VK_SUCCESS != result || 0 == numExtensions)
				//{
				//	printf("ERROR: Failed to vkEnumerateDeviceExtensionProperties\n");
				//	assert(0);
				//	return false;
				//}

				m_vkExtensionProperties = new VkExtensionProperties[numExtensions];
				fixed(VkExtensionProperties* ptr = m_vkExtensionProperties)
					vkEnumerateDeviceExtensionProperties(m_vkPhysicalDevice, null, &numExtensions, ptr);

				//if (VK_SUCCESS != result || 0 == numExtensions)
				//{
				//	printf("ERROR: Failed to vkEnumerateDeviceExtensionProperties\n");
				//	assert(0);
				//	return false;
				//}
			}

			return true;
		}



	}
}
