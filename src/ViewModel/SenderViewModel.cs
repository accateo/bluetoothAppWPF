// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SenderViewModel.cs" company="saramgsilva">
//   Copyright (c) 2014 saramgsilva. All rights reserved.
// </copyright>
// <summary>
//   The Sender view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Windows.Input;
using Bluetooth.Services;
using Bluetooth.Model;
using bluetoothAppWPF.Shared.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Threading;
using System;
using InTheHand.Net.Sockets;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using InTheHand.Net.Bluetooth;
using System.Net.Sockets;
using System.Text;
using InTheHand.Net;
using System.Windows;
using System.Timers;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace bluetoothAppWPF.ViewModel
{
    /// <summary>
    /// The Sender view model.
    /// </summary>

    public sealed class SenderViewModel : ViewModelBase
    {
        private readonly ISenderBluetoothService _senderBluetoothService;
        private string _data;
        private Device _selectDevice;
        private string _resultValue;
        private string _deviceStatus;
        private string _deviceMac = " --- ";
        private string _alertText;
        private Visibility _isVisible;
        private string _currentCommand;
        private readonly Guid _serviceClassId = new Guid("0e6114d0-8a2e-477a-8502-298d1ff4b4bb");
        //timer visibilità
        System.Timers.Timer visTimer = null;

        private BluetoothListener _listener;



        /// <summary>
        /// Initializes a new instance of the <see cref="SenderViewModel"/> class.
        /// </summary>
        /// <param name="senderBluetoothService">
        /// The Sender bluetooth service.
        /// </param>
        /// 

        public SenderViewModel(ISenderBluetoothService senderBluetoothService)
        {

            _senderBluetoothService = senderBluetoothService;
            AlertVisibility = Visibility.Hidden;
            ResultValue = "N/D";
            //button action
            SendCommand = new RelayCommand(
                SendData,
                () => !string.IsNullOrEmpty(Data) && SelectDevice != null && SelectDevice.DeviceInfo != null);
            ConnectDevice = new RelayCommand(
              ConnectBlue,
              () => SelectDevice != null);
            RefreshBT = new RelayCommand(() => { 
                RefreshDataBT();  
            });
            
            Devices = new ObservableCollection<Device>
            {
                new Device(null) { DeviceName = "Searching..." }
            };
            RefreshData = new RelayCommand(() =>
            {
                Debug.WriteLine("Refresh");
   


            });
            
            //start to search device
            ShowDevice();
            
            //Start thread to keep cheking the connection status
            Thread trDevConnection = new Thread(CheckDeviceConnection) { IsBackground = true }; 
            trDevConnection.Start();
            BluetoothRadio myRadio = BluetoothRadio.PrimaryRadio;
            //bluetooth listener
            _listener = new BluetoothListener(_serviceClassId)
            {
                ServiceName = "MyService"
            };

           

            

        }

        /// <summary>
        /// Gets or sets the devices.
        /// </summary>
        /// <value>
        /// The devices.
        /// </value>
        /// 

        public ObservableCollection<Device> Devices
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the select device.
        /// </summary>
        /// <value>
        /// The select device.
        /// </value>
        public Device SelectDevice
        {
            get { return _selectDevice; }
            set { Set(() => SelectDevice, ref _selectDevice, value);
                ShowAddress();
            }

        }
        //show mac address selected device
        private void ShowAddress()
        {
            DeviceMac = _selectDevice.DeviceInfo.DeviceAddress.ToString();

        }
        
        


        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public string Data
        {
            get { return _data; }
            set { Set(() => Data, ref _data, value); }
        }

        /// <summary>
        /// Gets or sets the result value.
        /// </summary>
        /// <value>
        /// The result value.
        /// </value>
        public string ResultValue
        {
            get { return _resultValue; }
            set { Set(() => ResultValue, ref _resultValue, value); }
        }

        /// <summary>
        /// Gets or sets the result value.
        /// </summary>
        /// <value>
        /// The result value.
        /// </value>
        public string DeviceStatus
        {
            get { return _deviceStatus ; }
            set { Set(() => DeviceStatus, ref _deviceStatus, value); }
        }
        public string DeviceMac
        {
            get { return _deviceMac; }
            set { Set(() => DeviceMac, ref _deviceMac, value); }
        }

        

        
        //BUTTONS
        /// <summary>
        /// Gets the send command.
        /// </summary>
        /// <value>
        /// The send command.
        /// </value>
        //action button
        public ICommand SendCommand { get; private set; }
        public ICommand ConnectDevice { get; private set; }
        public ICommand RefreshBT { get; private set; }
        
        public ICommand RefreshData { get; private set; }
        

        //gestisco visibilità alert avviso
        public Visibility AlertVisibility
        {
            get { return _isVisible; }
            set { Set(() => AlertVisibility, ref _isVisible, value); }
        }
        //mostro alert per x secondi
        private void showAlert()
        {

            AlertVisibility = Visibility.Visible;
            //timer aggiornamento tabella
            if (visTimer == null)
            {
                visTimer = new System.Timers.Timer();
                visTimer.Elapsed += new ElapsedEventHandler(hiddenAlert);
                visTimer.Interval = 3000;
                visTimer.Enabled = true;
            }
        }

        //funzione timer visibilità alert
        private void hiddenAlert(object source, ElapsedEventArgs e)
        {
            AlertVisibility = Visibility.Hidden;
            visTimer.Enabled = false;
            visTimer = null;

        }

        private async void SendData()
        {
            ResultValue = "N/D";
            showAlert();
            //var wasSent = await _senderBluetoothService.Send(SelectDevice, Data);
            var wasSent = await _senderBluetoothService.SendAsync(SelectDevice, Data+"\r");
            Debug.WriteLine("messaggio:" + Data);
            if (wasSent)
            {
                //aspetto
                Thread.Sleep(300);
                ResultValue = " "+_senderBluetoothService.GetBufferData();

            }
            else
            {
                ResultValue = " The data was not sent.";
                
            }
             
            
        }

        /// <summary>
        /// Make the device connection.
        /// </summary>
        /// <value>
        /// The selected device.
        /// </value>
        private async void ConnectBlue()
        {
            DeviceStatus = "Connecting";
            _senderBluetoothService.DeviceConnection(SelectDevice);
            

        }
        //refresh lista device bluetooth
        private async void RefreshDataBT()
        {
            ShowDevice();

        }
        //send command to device
        private async void SendCommandToDevice(string command)
        {
            Debug.WriteLine("Invio comando");
            showAlert();
            //invio comando in modalità async
            var wasSent = await _senderBluetoothService.SendAsync(SelectDevice, command + "\r");
            //_senderBluetoothService.message(Data.ToString());
            if (wasSent)
            {
                //aspetto
                Thread.Sleep(500);
                
                    

            }
            else
            {
                ResultValue = " The data was not sent.";

            }
        }

        /// <summary>
        /// Keeping checking the device connection.
        /// </summary>
        /// <value>
        /// DeviceStatus - if device is connected or not.
        /// </value>
        async private void CheckDeviceConnection()
        {
            Thread.Sleep(TimeSpan.FromSeconds(5));
            var wasConnected =   _senderBluetoothService.CheckConnection();
            if (wasConnected)
            {
                if (!DeviceStatus.Equals("Connected"))
                {
                    Thread.Sleep(300);

                    DeviceStatus = "Connected";
                }
                


            }
            else
            {
                DeviceStatus = "No Connected";
            }

            Thread trDevConnection = new Thread(CheckDeviceConnection) { IsBackground = true };
            trDevConnection.Start();
        }




        /// <summary>
        /// Shows the device.
        /// </summary>
        /// <param name="deviceMessage">The device message.</param>
        private async void ShowDevice()
        {
            Debug.WriteLine("REFRESH DEVICES");
            var items = await _senderBluetoothService.GetDevices();
            Devices.Clear();
            Devices.Add(items);
            Data = string.Empty;
            
        }
        
    }
}
