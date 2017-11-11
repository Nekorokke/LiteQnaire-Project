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
using HeadXMLLib;
using ExtractFileLib;
using FileGlueLib;
using Microsoft.Win32;
using System.IO;


namespace LiteQnaire
{
    /// <summary>
    /// AttachmentWindow.xaml
    /// </summary>
    public partial class AttachmentWindow : Window
    {
        string tag = LiteQnaire.MainWindow.nowTextBlock.Tag.ToString();
        int i = 0;
        int j = 0;
        attachment tempAtt;
        public AttachmentWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            i = ConvertTo.IandJ(tag)[0];//block index
            j = ConvertTo.IandJ(tag)[1];//item index

            closeButton.Source = LiteQnaire.MainWindow.closeButtonImg;

            tempAtt = (attachment)((block)LiteQnaire.MainWindow.hXML.blocks[i]).Contents[j];

            title.Content = tempAtt.filename;
            titleTextBox.Text = tempAtt.filename;
            
            AddFile.Source = new BitmapImage(new Uri(LiteQnaire.MainWindow.CurrentDirectory + @"\Image\addfile.png", UriKind.Absolute));
            ExtractTo.Source = new BitmapImage(new Uri(LiteQnaire.MainWindow.CurrentDirectory + @"\Image\extractto.png", UriKind.Absolute));
            Delete.Source = new BitmapImage(new Uri(LiteQnaire.MainWindow.CurrentDirectory + @"\Image\delete.png", UriKind.Absolute));
            
            labelAddFile.Cursor = System.Windows.Input.Cursors.Hand;
            labelDelete.Cursor = System.Windows.Input.Cursors.Hand;
            title.Cursor = System.Windows.Input.Cursors.Hand;
            
            //If not use DispatcherTimer to load the icon of file,the programme will crash and I don't know why.
            System.Windows.Threading.DispatcherTimer dTimer = new System.Windows.Threading.DispatcherTimer();
            dTimer.Tick += dTimerLoad;
            dTimer.Interval = new TimeSpan(0,0,0,0,10);
            dTimer.Start();
        }

        private void dTimerLoad(object sender, EventArgs e)
        {
            title.ToolTip = title.Content;
            GetIcon getIcon = new GetIcon();

            BitmapSource bSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(getIcon.Get(ConvertTo.FileType(tempAtt.filename), true).ToBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            fileImg.Source = bSource;
            bSource = null;
            getIcon = null;

            ((LiteQnaire.MainWindow.nowTextBlock.Parent as WrapPanel).Children[0] as Image).Source = fileImg.Source;

            if (LiteQnaire.MainWindow.AttachmentFiles[tempAtt.fileIndex - 1] is int)
            {
                ExtractFile eFile = new ExtractFile(LiteQnaire.MainWindow.filePath);
                size.Content = "Size: " + ConvertTo.Size(eFile.GetSize(tempAtt.fileIndex));
            }
            else
            {
                size.Content = "Size: " + ConvertTo.Size(Convert.ToInt32(new FileInfo((string)LiteQnaire.MainWindow.AttachmentFiles[tempAtt.fileIndex - 1]).Length));
            }
            ((System.Windows.Threading.DispatcherTimer)sender).Stop();
        }
  
        //Extract file.
        private void EF_Click(object sender, MouseButtonEventArgs e)
        {         
            SaveFileDialog sfd = new SaveFileDialog();
            string filters="Extension|";
            foreach (string filter in tempAtt.filter)
            {
                filters += filter+";";
            }
            filters = filters.TrimEnd(';');
            sfd.Filter = filters;
         
            bool? save=sfd.ShowDialog();
            if (save == true)
            {
                string path = sfd.FileName;
                if (ConvertTo.FileType(ConvertTo.FileName(path)) == ConvertTo.FileName(path))
                {
                    path += ConvertTo.FileType(tempAtt.filename);
                }

                if (LiteQnaire.MainWindow.AttachmentFiles[tempAtt.fileIndex - 1] is int)
                {
                    ExtractFile eFile = new ExtractFile(LiteQnaire.MainWindow.filePath);
                    eFile.ExtractTo(path, tempAtt.fileIndex);                    
                }
                else
                {
                    File.Copy((string)LiteQnaire.MainWindow.AttachmentFiles[tempAtt.fileIndex - 1],path,true);
                }

                System.Diagnostics.Process.Start(sfd.FileName.Substring(0,sfd.FileName.Count()-sfd.SafeFileName.Count()));//Open it's folder.
                this.Close();
            }         
        }

        private void Delete_Click(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Are you sure to delete this file?", "LiteQnaire", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                LiteQnaire.MainWindow.AttachmentFiles[tempAtt.fileIndex - 1] = LiteQnaire.MainWindow.CurrentDirectory + @"\Bubble";//Bubble is an empty file.
                /* Programme shows there isn't an attachment.
                   Truly there is a /Bubble/.
                   The index without minus sign tells its true index.
                */
                tempAtt.fileIndex = -(tempAtt.fileIndex);
                tempAtt.filename = "Click to Add a File.";
                LiteQnaire.MainWindow.isChanged = true;
                LiteQnaire.MainWindow.nowTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                ((LiteQnaire.MainWindow.nowTextBlock.Parent as WrapPanel).Children[0] as Image).Source = LiteQnaire.MainWindow.attImg;
                this.Close();
            }
        }

        private void AddFile_Click(object sender, MouseButtonEventArgs e)
        {
            if (tempAtt.isLocked == false)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                string filters = "Extension|";
                foreach (string filter in tempAtt.filter)
                {
                    filters += filter + ";";
                }
                filters = filters.TrimEnd(';');
                ofd.Filter = filters;

                bool? open = ofd.ShowDialog();
                if (open == true)
                {
                    if (MessageBox.Show("Are you sure to replace this file?", "LiteQnaire", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {                    
                        LiteQnaire.MainWindow.AttachmentFiles[tempAtt.fileIndex - 1] = ofd.FileName;
                        LiteQnaire.MainWindow.isChanged = true;
                        tempAtt.filename = ConvertTo.FileName(ofd.FileName);

                        GetIcon getIcon = new GetIcon();

                        BitmapSource bSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(getIcon.Get(ConvertTo.FileType(tempAtt.filename), true).ToBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        fileImg.Source = bSource;
                        ((LiteQnaire.MainWindow.nowTextBlock.Parent as WrapPanel).Children[0] as Image).Source = fileImg.Source;

                        bSource = null;
                        getIcon = null;
                        this.Close();

                        MessageBox.Show("Successfully Extracted !", "LiteQnaire", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                }
            }
        }

        //Show TextBox let user edit the file name.
        private void title_Click(object sender, MouseButtonEventArgs e)
        {
           if(tempAtt.isLocked==false){
            titleTextBox.Visibility = System.Windows.Visibility.Visible;
            titleTextBox.Focus();
            titleTextBox.Select(0, titleTextBox.Text.Count() - ConvertTo.FileType(titleTextBox.Text).Count());
           }
        }

        private void this_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FileNameChange();
        }

        //Change the file name by TextBox.
        private void FileNameChange()
        {
            if (titleTextBox.Text.ToString() != title.Content.ToString())
            {
                if (ConvertTo.FileType(titleTextBox.Text) != ConvertTo.FileType(tempAtt.filename))//Check if the extension is right.
                {
                    titleTextBox.Text += ConvertTo.FileType(tempAtt.filename);
                }

                LiteQnaire.MainWindow.isChanged = true;
                tempAtt.filename = titleTextBox.Text;
            
                title.Content = tempAtt.filename;
                title.ToolTip = title.Content;
            }
            titleTextBox.Visibility = Visibility.Hidden;
        }

        private void closeButtonMouseMove(object sender, MouseEventArgs e)
        {
            closeButton.Source = LiteQnaire.MainWindow.closemoveButtonImg;
        }

        private void closeButtonMouseLeave(object sender, MouseEventArgs e)
        {
            closeButton.Source = LiteQnaire.MainWindow.closeButtonImg;
        }

        private void closeButton_Click(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void WindowMouseLeave(object sender, MouseEventArgs e)
        {
            closeButton.Source = LiteQnaire.MainWindow.closeButtonImg;
        }

        private void Label_MouseMove(object sender, MouseEventArgs e)
        {
            ((Label)sender).Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Label)sender).Foreground = new SolidColorBrush(Colors.DimGray);
        }

        private void textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FileNameChange();
            }
        }

        private void this_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LiteQnaire.MainWindow.nowTextBlock.ToolTip = tempAtt.filename;
            ((block)LiteQnaire.MainWindow.hXML.blocks[i]).Contents[j] = tempAtt;
        }
    }
}
