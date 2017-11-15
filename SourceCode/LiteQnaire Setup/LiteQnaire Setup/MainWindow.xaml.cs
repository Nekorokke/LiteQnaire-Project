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
using Microsoft.Win32;
using System.IO;
using ExtractFileLib;
using WSH=IWshRuntimeLibrary;


namespace LiteQnaire_Setup
{
    /// <summary>
    /// MainWindow.xaml
    /// </summary>
    public struct setupInfo
    {
        public string path;
        public bool? edit;
        public bool? open;
        public bool? scDestop;

        public setupInfo(bool activate)
        {
            path = "C:\\LiteQnaire\\";
            edit = true;
            open = true;
            scDestop = true;
        }
    }

    public partial class MainWindow : Window
    {
        public setupInfo sInfo=new setupInfo(true);
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Custom_Click(object sender, MouseButtonEventArgs e)
        {
            (sender as Label).Opacity = 0.8;
            Custom customWindow=new Custom();
            customWindow.Owner=this;
            customWindow.ShowDialog();
        }

        private delegate void UpdatePBDelegate(System.Windows.DependencyProperty dp, object value);
        private void PBSetValue(double value)
        {
            UpdatePBDelegate updatePBDelegate = new UpdatePBDelegate(progressBar.SetValue);
            Dispatcher.Invoke(updatePBDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { ProgressBar.ValueProperty, value });
        }

        private void QuickSetup_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Release files.
                ((sender as Label).Parent as Border).Visibility = Visibility.Hidden;
                customLabel.Visibility = Visibility.Hidden;
                progressBar.Maximum = 100;
                PBSetValue(1);

                byte[] package = Properties.Resources.package;
                byte[] uninstall = Properties.Resources.LiteQnaire_Uninstallor;
                PBSetValue(5);

                if (!Directory.Exists(sInfo.path)) { Directory.CreateDirectory(sInfo.path); }
                string pkgPath = sInfo.path + "\\package.pkg";
                PBSetValue(15);

                FileStream fStream = new FileStream(pkgPath, FileMode.Create);
                fStream.Write(package, 0, package.Length);
                fStream.Flush();
                fStream.Close();

                fStream = new FileStream(sInfo.path + "\\Uninstall.exe", FileMode.Create);
                fStream.Write(uninstall, 0, uninstall.Length);
                fStream.Flush();
                fStream.Close();
                PBSetValue(20);

                if (!File.Exists(pkgPath)) { return; }

                ExtractFile eFile = new ExtractFile(pkgPath);
                eFile.ExtractTo(pkgPath + ".info", 0);

                FileStream infoFStream = new FileStream(pkgPath + ".info", FileMode.Open);
                StreamReader sReader = new StreamReader(infoFStream);
                PBSetValue(25);

                double setValue = (double)60 / eFile.GetMaxFileCount();

                string line;
                int i = 1;
                while ((line = sReader.ReadLine()) != null)
                {
                    eFile.ExtractTo(sInfo.path + line, i);
                    PBSetValue(progressBar.Value + setValue);
                    i++;
                }
                sReader.Close();
                fStream.Close();

                File.Delete(pkgPath);
                File.Delete(pkgPath + ".info");

                //Register software information.
                RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall", true);
                RegistryKey software = key.CreateSubKey("LiteQnaire");

                software.SetValue("DisplayIcon", sInfo.path + @"\LiteQnaire\LiteQnaire.exe");
                software.SetValue("DisplayName", "LiteQnaire Set");
                software.SetValue("DisplayVersion", "1.0");
                software.SetValue("Publisher", "Nekorokke");
                software.SetValue("InstallLocation", sInfo.path);
                software.SetValue("InstallSource", sInfo.path);

                software.SetValue("UninstallString", sInfo.path + "\\uninstall.exe");
                software.Close();
                key.Close();
                PBSetValue(90);

                //Register file association.
                if (sInfo.open == true)
                {
                    key = Registry.ClassesRoot.CreateSubKey(".lqn");
                    key.SetValue("", "LiteQnaire.LQN");
                    key = Registry.ClassesRoot.CreateSubKey("LiteQnaire.LQN\\Shell\\Open\\Command");
                    key.SetValue("", "\"" + sInfo.path + "LiteQnaire\\LiteQnaire.exe\"" + "\" %1\"");

                    key = Registry.ClassesRoot.CreateSubKey("LiteQnaire.LQN\\DefaultIcon");
                    key.SetValue("", sInfo.path + "LiteQnaire\\lqnfile.ico");
                    key.Close();
                }
                PBSetValue(93);

                //Register 'Edit with LiteQnaire Editor' command menu.
                if (sInfo.edit == true)
                {
                    key = Registry.ClassesRoot.CreateSubKey("LiteQnaire.LQN\\Shell\\Edit");
                    key.SetValue("", "Edit with LiteQnaire Editor");
                    key = Registry.ClassesRoot.CreateSubKey("LiteQnaire.LQN\\Shell\\Edit\\Command");
                    key.SetValue("", "\"" + sInfo.path + "LiteQnaire Editor\\LiteQnaire Editor.exe\"" + "\" %1\"");
                    key.Close();
                }
                PBSetValue(95);

                //Create shortcut.
                if (sInfo.scDestop == true)
                {
                    string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);

                    WSH.WshShell wshShell = new WSH.WshShell();
                    WSH.IWshShortcut shortcut = (WSH.IWshShortcut)wshShell.CreateShortcut(desktopPath + "\\LiteQnaire Reader.lnk");
                    shortcut.TargetPath = sInfo.path + "LiteQnaire\\LiteQnaire.exe";
                    shortcut.IconLocation = sInfo.path + "LiteQnaire\\LiteQnaire.exe,0";
                    shortcut.Description = "LiteQnaire Reader";
                    shortcut.Save();

                    shortcut = (WSH.IWshShortcut)wshShell.CreateShortcut(desktopPath + "\\LiteQnaire Editor.lnk");
                    shortcut.TargetPath = sInfo.path + "LiteQnaire Editor\\LiteQnaire Editor.exe";
                    shortcut.IconLocation = sInfo.path + "LiteQnaire Editor\\LiteQnaire Editor.exe,0";
                    shortcut.Description = "LiteQnaire Editor";
                    shortcut.Save();

                    shortcut = (WSH.IWshShortcut)wshShell.CreateShortcut(desktopPath + "\\LiteQnaire Collector.lnk");
                    shortcut.TargetPath = sInfo.path + "LiteQnaire Collector\\LiteQnaire Collector.exe";
                    shortcut.IconLocation = sInfo.path + "LiteQnaire Collector\\LiteQnaire Collector.exe,0";
                    shortcut.Description = "LiteQnaire Collector";
                    shortcut.Save();
                }
                PBSetValue(100);

                RefreshDesktop.Refresh();

                startnowBorder.Visibility = Visibility.Visible;
                MessageBox.Show("LiteQnaire Successfully Installed!", "LiteQnaire Setup", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message + "\r\nInstall Failed.Please try again.","LiteQnaire Setup",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
        private void StartNow_Click(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(sInfo.path + "\\LiteQnaire Editor\\LiteQnaire Editor.exe");
            this.Close();
        }

        private void Github_Click(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", "https://github.com/Nekorokke/LiteQnaire-Project");
        }

        private void ButtonMouseMove(object sender, MouseEventArgs e)
        {
            ((sender as Label).Parent as Border).Background = new SolidColorBrush(Colors.LightBlue);
            ((sender as Label).Parent as Border).Opacity = 0.75;
        }

        private void ButtonMouseLeave(object sender, MouseEventArgs e)
        {
            ((sender as Label).Parent as Border).Background = new SolidColorBrush(Colors.WhiteSmoke);
            ((sender as Label).Parent as Border).Opacity = 1;
        }

        private void ButtonMouseUp(object sender, MouseButtonEventArgs e)
        {
            ((sender as Label).Parent as Border).Opacity = 0.8;
        }

        private void ButtonMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((sender as Label).Parent as Border).Opacity = 0.6;
        }

        private void LabelMouseMove(object sender, MouseEventArgs e)
        {
            (sender as Label).Opacity = 0.8;
        }

        private void LabelMouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Label).Opacity = 1;
        }

        private void LabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            (sender as Label).Opacity = 0.5;
        }

        private void LabelMouseUp(object sender, MouseButtonEventArgs e)
        {
            (sender as Label).Opacity = 0.8;
        }
    }
}
