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
using Winform = System.Windows.Forms;

namespace LiteQnaire_Setup
{
    /// <summary>
    /// Custom.xaml
    /// </summary>
    public partial class Custom : Window
    {
        public Custom()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            setupInfo sInfo = (this.Owner as MainWindow).sInfo;
            pathTextBox.Text = sInfo.path;
            pathTextBox.ToolTip = sInfo.path;
            openwithCheck.IsChecked = sInfo.open;
            editwithCheck.IsChecked = sInfo.edit;
            scdesktopCheck.IsChecked = sInfo.scDestop;
        }

        private void OK_Click(object sender, MouseButtonEventArgs e)
        {
            ((sender as Label).Parent as Border).Opacity = 0.75;
            setupInfo sInfo = new setupInfo();
            sInfo.path=pathTextBox.Text;
            sInfo.open=openwithCheck.IsChecked;
            sInfo.edit=editwithCheck.IsChecked ;
            sInfo.scDestop=scdesktopCheck.IsChecked ;
            (this.Owner as MainWindow).sInfo = sInfo;
            this.Close();
        }

        private void TextBox_Click(object sender, MouseButtonEventArgs e)
        {
            Winform.FolderBrowserDialog fbd = new Winform.FolderBrowserDialog();
            if (fbd.ShowDialog() == Winform.DialogResult.OK)
            {
                pathTextBox.Text = fbd.SelectedPath+"LiteQniare\\";
                pathTextBox.ToolTip = fbd.SelectedPath + "LiteQnaire\\";
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

        private void ButtonMouseUp(object sender, MouseButtonEventArgs e)
        {
            ((sender as Label).Parent as Border).Opacity = 0.75;
        }

        private void ButtonMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((sender as Label).Parent as Border).Opacity = 0.5;
        }
    }
}
