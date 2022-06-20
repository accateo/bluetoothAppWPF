// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="saramgsilva">
//   Copyright (c) 2014 saramgsilva. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Media;
using FirstFloor.ModernUI.Presentation;
using System.Threading;
using System;
using System.Windows;
using System.Diagnostics;
using System.ComponentModel;
using Microsoft.Win32;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Threading;

namespace bluetoothAppWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow
    {
        public static MainWindow wi;
        public List<string> pattern_commands = new List<string>();
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            wi = this;
            //gestore chiusura applicazione
            Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);
            //colore sfondo
            AppearanceManager.Current.AccentColor = Colors.Blue;
            //larghezza finestra
            this.Width = SystemParameters.WorkArea.Width;
            this.Height = SystemParameters.WorkArea.Height;
            this.Left = 0;
            this.Top = 0;
            this.WindowState = WindowState.Normal;


        }
        

        //gestione chiusura finestra
        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            //Your code to handle the event
            Debug.WriteLine("Closed");
            // The user wants to exit the application. Close everything down.
            Environment.Exit(-1);
        }


    }
}
