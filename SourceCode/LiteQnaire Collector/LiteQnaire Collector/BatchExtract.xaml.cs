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
using System.Windows.Shapes;
using System.Collections;
using Winform = System.Windows.Forms;

namespace LiteQnaire_Collector
{
    /// <summary>
    /// BatchExtract.xaml 的交互逻辑
    /// </summary>
    public partial class BatchExtract : Window
    {

        public int blockIndex;
        public int itemIndex;
        string folderPath="";

        public BatchExtract()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            blockIndexTextbox.Text = blockIndex.ToString();
            itemIndexTextbox.Text = itemIndex.ToString();
            indexRadio.IsChecked = true;
        }

        private void okButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (folderPath != "")
            {
                if (MessageBox.Show("Click \"OK\" to Extract.\r\n" + "Please check the Information again.", "BatchExtract", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                {
                    ((sender as Label).Parent as Border).Opacity = 0.5;

                    NamingInfo namingInfo = new NamingInfo();
                    if (enumerateRadio.IsChecked == true)
                    {
                        namingInfo = new NamingInfo(NamingMethod.Enumerate);
                    }
                    else if (originRadio.IsChecked == true)
                    {
                        namingInfo = new NamingInfo(NamingMethod.Origin);
                    }
                    else if (attRadio.IsChecked == true)
                    {
                        namingInfo = new NamingInfo(NamingMethod.Att);
                    }
                    else if (indexRadio.IsChecked == true)
                    {
                        if (nblockIndexTextbox.Text != "" && nitemIndexTextbox.Text != "")
                        {
                            namingInfo = new NamingInfo("\"" + nblockIndexTextbox.Text + "," + nitemIndexTextbox.Text + "\"");
                        }
                        else
                        {
                            MessageBox.Show("Please input an Index.", "Batch Extract", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                    }

                    (this.Owner as MainWindow).BatchExtract("\"" + blockIndexTextbox.Text + "," + itemIndexTextbox.Text + "\"", folderPath, namingInfo);
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Please select a Folder.", "Batch Extract", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
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

        private void ButtonMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((sender as Label).Parent as Border).Opacity = 0.5;
        }

        private void ButtonMouseUp(object sender, MouseButtonEventArgs e)
        {
            ((sender as Label).Parent as Border).Opacity = 0.75;
        }

        private void numTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            /*if ((sender as TextBox).Text == "")
            {
                (sender as TextBox).Text = "1";
            }*/
        }

        private void numTextbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int integer;

            if (int.TryParse(e.Text, out integer) == false)
            {
                if ((sender as TextBox).Text != "")
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        private void folderTextbox_Click(object sender, MouseButtonEventArgs e)
        {
            Winform.FolderBrowserDialog fbd = new Winform.FolderBrowserDialog();
            if (fbd.ShowDialog() == Winform.DialogResult.OK)
            {
                (sender as TextBox).Text = fbd.SelectedPath+"\\";
                (sender as TextBox).ToolTip = fbd.SelectedPath + "\\";
                folderPath = fbd.SelectedPath+"\\";
            }
        }



        private void otherRadio_Checked(object sender, RoutedEventArgs e)
        {
            namedByWrap.IsEnabled = false;
        }

        private void indexRadio_Checked(object sender, RoutedEventArgs e)
        {
            namedByWrap.IsEnabled = true;
        }
    }
    
}
