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
using LQNCollectorLib;

namespace LiteQnaire_Collector
{
    /// <summary>
    /// OutputWindow.xaml
    /// </summary>
    public partial class OutputWindow : Window
    {
        public int blockIndex;
        public int itemIndex;
        public ArrayList sheetsArray;//Sheet names ArrayList.

        public OutputWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            blockIndexTextbox.Text = blockIndex.ToString();
            itemIndexTextbox.Text = itemIndex.ToString();
            sheetsCombo.ItemsSource = sheetsArray;
            sheetsCombo.SelectedIndex = 0;
        }

        private void okButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Click \"OK\" to Output.\r\n" + "Please check the Information again.", "Output", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                ((sender as Label).Parent as Border).Opacity = 0.5;

                outputSheetInfo opsInfo = new outputSheetInfo();
                opsInfo.IsColumn = (bool)columnRadio.IsChecked;
                opsInfo.LineIndex = Convert.ToInt32(lineIndexTextbox.Text) - 1;
                opsInfo.StartFrom = Convert.ToInt32(startFromTextbox.Text) - 1;
                opsInfo.SheetName = sheetsCombo.SelectedItem.ToString();

                fillerInfo fInfo = new fillerInfo(blockIndex, itemIndex);

                (this.Owner as MainWindow).Output(opsInfo, fInfo);
                this.Close();
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
            if ((sender as TextBox).Text == "")
            {
                (sender as TextBox).Text = "1";
            }
        }

        //Input check.
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
    }
}
