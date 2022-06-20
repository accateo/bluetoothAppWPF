using System.Collections.Generic;
using System.Threading.Tasks;
using Bluetooth.Model;

namespace Bluetooth.Services
{
    /// <summary>
    /// Define the sender Bluetooth service interface.
    /// </summary>
    public interface ISenderBluetoothService
    {
        /// <summary>
        /// Gets the devices.
        /// </summary>
        /// <returns>The list of the devices.</returns>
        Task<IList<Device>> GetDevices();

        /// <summary>
        /// Sends the data to the Receiver.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="content">The content.</param>
        /// <returns>If was sent or not.</returns>
        Task<bool> Send(Device device, string content);

        /// <summary>
        /// Sends the data to the Receiver.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="content">The content.</param>
        /// <returns>If was sent or not.</returns>
        Task<bool> SendAsync(Device device, string content);

        /// <summary>
        /// Get data from buffer.
        /// </summary>
        /// <returns>Buffer data</returns>
        string GetBufferData();

        /// <summary>
        /// Check the device connection status.
        /// </summary>
        /// <param name=""> </param>
        /// <param name=""></param>
        /// <returns>If device is connected or not.</returns>
        //Task<bool> CheckConnection();
        bool CheckConnection();

        /// <summary>
        /// Create a connection for the selected device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <returns>Nothing - since a threar will be checking the status</returns>
        void DeviceConnection(Device device);
        

    }
}