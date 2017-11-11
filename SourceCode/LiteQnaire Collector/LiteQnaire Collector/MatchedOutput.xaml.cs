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
using LQNCollectorLib;
using System.Collections;

namespace LiteQnaire_Collector
{
    /// <summary>
    /// MatchingOutput.xaml
    /// </summary>
    public partial class MatchedOutput : Window
    {
        public int blockIndex;
        public int itemIndex;
        public ArrayList sheetsArray;
        public MatchedOutput()
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
            if (MessageBox.Show("Click \"OK\" to Output.\r\n" + "Please check the Information again.", "Matched Output", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                ((sender as Label).Parent as Border).Opacity = 0.5;

                matchingSheetInfo msInfo = new matchingSheetInfo();
                msInfo.IsColumn = (bool)columnRadio.IsChecked;
                msInfo.LineIndex = Convert.ToInt32(lineIndexTextbox.Text) - 1;
                msInfo.StartFrom = Convert.ToInt32(startFromTextbox.Text) - 1;
                msInfo.EndBy = Convert.ToInt32(endByTextbox.Text) - 1;
                msInfo.OutputLineIndex = Convert.ToInt32(opLineIndexTextbox.Text) - 1;
                msInfo.indexString = "\"" + mblockIndexTextbox.Text + "," + mitemIndexTextbox.Text + "\"";
                msInfo.SheetName = sheetsCombo.SelectedItem.ToString();

                fillerInfo fInfo = new fillerInfo(blockIndex, itemIndex);

                (this.Owner as MainWindow).Output(msInfo, fInfo);
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
    }
}
