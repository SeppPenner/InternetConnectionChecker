using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

public partial class Main : Form
{
    private BackgroundWorker _backgroundPing;
    private Bitmap _green;
    private Bitmap _red;

    public Main()
    {
        InitializeComponent();
        Initialize();
    }

    private void Initialize()
    {
        InitializeCaptionAndImages();
        InitializeBackgroundPing();
    }

    private void InitializeCaptionAndImages()
    {
        try
        {
            Text = Application.ProductName + @" " + Application.ProductVersion;
            var location = Assembly.GetExecutingAssembly().Location;
            if (location == null)
                throw new ArgumentNullException(nameof(location));
            location = Directory.GetParent(location).FullName;
            _red = (Bitmap) Image.FromFile(Path.Combine(location, "Red.ico"), true);
            _green = (Bitmap) Image.FromFile(Path.Combine(location, "Green.ico"), true);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void InitializeBackgroundPing()
    {
        _backgroundPing = new BackgroundWorker {WorkerSupportsCancellation = true};
        _backgroundPing.DoWork += CheckInternetConnection;
        _backgroundPing.RunWorkerAsync();
    }

    private void CheckInternetConnection(object sender, DoWorkEventArgs doWorkEventArgs)
    {
        while (true)
        {
            this.UiThreadInvoke(() => { pictureBoxConnected.Image = IsConnected() ? _green : _red; });
            Thread.Sleep(1000);
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private bool IsConnected()
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
}