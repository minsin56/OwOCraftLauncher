using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CmlLib.Core.Downloader;
using CmlLib.Core;
using CmlLib.Core.Auth;
using Launcher.Data;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Threading;
using System.IO.Compression;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static CMLauncher CMLauncher;
        public static MainWindow Instance;
        public static bool ForgeInstalled;


        public MSession CurrentSession;

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            Accounts.LoadAccounts();
            CMLauncher = new CMLauncher(new MinecraftPath(Directory.GetCurrentDirectory() + "/Data/Game", Directory.GetCurrentDirectory() + "/Data/Assets"));
            TryAutoLogin();

            JObject Object = JObject.Parse(File.ReadAllText("Config.json"));

            ForgeInstalled = (bool)Object["ForgeInstalled"];

        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            new LoginPage().Show();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            CMLauncher.ProgressChanged += (o, ef) =>
            {
                Dispatcher.Invoke(() =>
                {
                    MainProgressBar.Value = ef.ProgressPercentage;
                });
            };
            CMLauncher.FileChanged += (fu) =>
            {

            };
            CMLauncher.LogOutput += (ee, Log) =>
            {
                Dispatcher.Invoke(() =>
                {
                    Label L = new Label() { Content = Log };
                    LogList.Items.Add(L);
                    LogList.ScrollIntoView(L);
                });
            };


            if (!Directory.Exists("Data/Game"))
            {
                Directory.CreateDirectory("Data/Game");
            }

            if (!Directory.Exists("Data/Game/mods"))
            {
                Directory.CreateDirectory("Data/Game/mods");
            }

            if (!File.Exists("Mods.zip"))
            {

                ModDownloader.DownloadFile((obj, prog) =>
                {
                    Dispatcher.Invoke(() => MainProgressBar.Value = prog.ProgressPercentage);
                },
                (f, ff) =>
                {
                    Directory.Delete("Data/Game/mods", true);
                    Directory.CreateDirectory("Data/Game/mods");
                    ZipFile.ExtractToDirectory("Mods.zip", "Data/Game/mods");
                    LaunchGame();
                });
            }
            else
            {
                Directory.Delete("Data/Game/mods",true);
                Directory.CreateDirectory("Data/Game/mods");
                ZipFile.ExtractToDirectory("Mods.zip", "Data/Game/mods");
                LaunchGame();
            }
        }

        private void LaunchGame()
        {
            ThreadPool.QueueUserWorkItem((Task) =>
            {
                MLaunchOption Options = new MLaunchOption()
                {
                    MaximumRamMb = 2048,
                    Session = CurrentSession,
                    ServerIp = "45.23.44.208"
                };


                Process Proc = CMLauncher.CreateProcess("1.16.3", "34.1.11", Options);
                Proc.Start();
            });
        }

        public void TryAutoLogin()
        {
            MLogin Log = new MLogin();
            MLoginResponse Response = Log.TryAutoLogin();

            if (Response.IsSuccess)
            {
                CurrentSession = Response.Session;
            }
        }

        public bool Login(MinecraftAccount Account)
        {
            MLogin Log = new MLogin();

            MLoginResponse Response = Log.Authenticate(Account.Email, Account.Password);

            if (!Response.IsSuccess)
            {
                return false;
            }

            CurrentSession = Response.Session;
            return true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Accounts.SaveAccounts();
        }

    }
}
