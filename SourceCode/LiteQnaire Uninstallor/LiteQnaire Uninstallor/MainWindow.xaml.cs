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
using System.IO;
using Microsoft.Win32;
using System.Collections;
using System.Diagnostics;

namespace LiteQnaire_Uninstallor
{
    /// <summary>
    /// MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            if(MessageBox.Show("Are you sure to uninstall LiteQnaire V1.0?","Uninstall",MessageBoxButton.YesNo,MessageBoxImage.Question)==MessageBoxResult.Yes)
            {
                ArrayList errMessage = new ArrayList();

                try
                {
                    Registry.LocalMachine.DeleteSubKeyTree("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\LiteQnaire");
                }
                catch
                {

                }
                try
                {
                    Registry.ClassesRoot.DeleteSubKeyTree("LiteQnaire.LQN");
                }
                catch
                {

                }

                try
                {
                    Process[] process = Process.GetProcessesByName("liteqnaire.exe");
                    foreach(Process proc in process)
                    {
                        proc.Kill();
                    }
                    Directory.Delete(CurrentDirectory+"\\LiteQnaire",true);
                }
                catch (Exception err)
                {
                    errMessage.Add(err.Message);
                }

                try
                {
                    Process[] process = Process.GetProcessesByName("liteqnaire editor.exe");
                    foreach (Process proc in process)
                    {
                        proc.Kill();
                    }
                    Directory.Delete(CurrentDirectory+"LiteQnaire Editor", true);
                }
                catch (Exception err)
                {
                    errMessage.Add(err.Message);
                }

                try
                {
                    Process[] process = Process.GetProcessesByName("liteqnaire collector.exe");
                    foreach (Process proc in process)
                    {
                        proc.Kill();
                    }
                    Directory.Delete(CurrentDirectory+"LiteQnaire Collector", true);
                }
                catch (Exception err)
                {
                    errMessage.Add(err.Message);
                }
                RefreshDesktop.Refresh();

                try
                {
                    string desktopFolder=System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
                    File.Delete(desktopFolder + "\\LiteQnaire Reader.lnk");
                    File.Delete(desktopFolder + "\\LiteQnaire Editor.lnk");
                    File.Delete(desktopFolder + "\\LiteQnaire Collector.lnk");
                }
                catch
                {

                }

                if (errMessage.Count > 0)
                {
                    string errorMessage = "";
                    foreach (string em in errMessage)
                    {
                        errorMessage += em + "\r\n";
                    }

                    MessageBox.Show(errorMessage + "Some files may not be deleted.You can delete them by yourself.", "Uninstall", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                FileStream batFile = new FileStream(CurrentDirectory+"\\cleanup.bat",FileMode.Create);
                StreamWriter sWriter = new StreamWriter(batFile);
                sWriter.WriteLine("taskkill /pid " + Process.GetCurrentProcess().Id+" /f");
                sWriter.WriteLine("del \"" + CurrentDirectory + "Uninstall.exe\"");
                sWriter.WriteLine("del %0");
                sWriter.Close();
                batFile.Close();

                Process batProcess = new Process();
                batProcess.StartInfo.FileName = CurrentDirectory + "\\cleanup.bat";
                batProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                batProcess.Start();
                this.Close();
            }
            else
            {
                this.Close();
            }
        }
    }
}
