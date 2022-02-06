// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Main.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The main form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace InternetConnectionChecker
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Net.NetworkInformation;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// The main form.
    /// </summary>
    public partial class Main : Form
    {
        /// <summary>
        /// The background ping worker.
        /// </summary>
        private BackgroundWorker backgroundPing = new();

        /// <summary>
        /// The green color.
        /// </summary>
        private Bitmap green = new Bitmap(1, 1);

        /// <summary>
        /// The red color.
        /// </summary>
        private Bitmap red = new Bitmap(1, 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="Main"/> class.
        /// </summary>
        public Main()
        {
            this.InitializeComponent();
            this.Initialize();
        }

        /// <summary>
        /// Checks whether the service is connected or not.
        /// </summary>
        /// <returns>A <see cref="bool"/> value indicating whether the service is connected or not.</returns>
        private static bool IsConnected()
        {
            try
            {
                new Ping().SendPingAsync("8.8.8.8");
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Initializes the data.
        /// </summary>
        private void Initialize()
        {
            this.InitializeCaptionAndImages();
            this.InitializeBackgroundPing();
        }

        /// <summary>
        /// Initializes the caption and images.
        /// </summary>
        private void InitializeCaptionAndImages()
        {
            try
            {
                this.Text = Application.ProductName + @" " + Application.ProductVersion;
                var location = Assembly.GetExecutingAssembly().Location;
                location = Directory.GetParent(location)?.FullName;
                this.red = (Bitmap)Image.FromFile(Path.Combine(location ?? string.Empty, "Red.ico"), true);
                this.green = (Bitmap)Image.FromFile(Path.Combine(location ?? string.Empty, "Green.ico"), true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Initializes the background ping.
        /// </summary>
        private void InitializeBackgroundPing()
        {
            this.backgroundPing = new BackgroundWorker { WorkerSupportsCancellation = true };
            this.backgroundPing.DoWork += this.CheckInternetConnection;
            this.backgroundPing.RunWorkerAsync();
        }

        /// <summary>
        /// Checks the internet connection.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void CheckInternetConnection(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                this.UiThreadInvoke(
                    () => { this.pictureBoxConnected.Image = IsConnected() ? this.green : this.red; });
                Thread.Sleep(1000);
            }
        }
    }
}