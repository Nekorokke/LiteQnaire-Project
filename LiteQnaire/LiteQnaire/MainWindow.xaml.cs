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
using System.Collections;
using HeadXMLLib;
using ExtractFileLib;
using FileGlueLib;
using System.IO;
using Microsoft.Win32;
using System.Windows.Threading;

namespace LiteQnaire
{
    /// <summary>
    /// MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window{
          //Button image sources.
          public static BitmapImage closeButtonImg;
          public static BitmapImage closemoveButtonImg;  
          BitmapImage minimizeButtonImg;
          BitmapImage minimizemoveButtonImg;
          BitmapImage saveButtonImg;
          BitmapImage savemoveButtonImg;
          BitmapImage openButtonImg;
          BitmapImage openmoveButtonImg;
          public static BitmapImage attImg;
          BitmapImage titleIconImg;

          public static string filePath="";//Opened lqn file path.

          public static string CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

          public static ArrayList AttachmentFiles=new ArrayList();//ArrayList that stores an index or a path string.

          public static HeadXMLFile hXML;

          public static bool isChanged=false;//Show whether a file have been changed.

          public static TextBlock nowTextBlock;

          public static string CacheFolder;
          public static string XMLFilepath;//Xml file path which have been extracted to the cache folder.

        public MainWindow()
        {
            InitializeComponent();

            //Loading image sources.
            saveButtonImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\save.png", UriKind.Absolute));
            savemoveButtonImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\save_move.png", UriKind.Absolute));
            openButtonImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\open.png", UriKind.Absolute));
            openmoveButtonImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\open_move.png", UriKind.Absolute));
            closeButtonImg = new BitmapImage(new Uri(CurrentDirectory+@"\Image\close.png", UriKind.Absolute));
            closemoveButtonImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\close_move.png", UriKind.Absolute));
            minimizeButtonImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\minimize.png", UriKind.Absolute));
            minimizemoveButtonImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\minimize_move.png", UriKind.Absolute));
            attImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\attachment.png", UriKind.Absolute));
            titleIconImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\icon.png", UriKind.Absolute));

            closeButton.Source = closeButtonImg;         
            minimizeButton.Source = minimizeButtonImg;
            openButton.Source = openButtonImg;
            saveButton.Source = saveButtonImg;
            titleIcon.Source = titleIconImg;

            RenderOptions.SetBitmapScalingMode(titleIcon, BitmapScalingMode.HighQuality);
        }

        //To receive arguments.
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Count()!=0)
            {
                //Creating cache folder and initialize "filePath" and "XMLFilepath"
                CacheFolder = CurrentDirectory + @"\Cache\Cache" + new Random().Next(9999999).ToString();
                Directory.CreateDirectory(CacheFolder);
                filePath = args[0];
                XMLFilepath = CacheFolder + "\\" + ConvertTo.FileName(filePath) + ".xml";

                ExtractFile eFile = new ExtractFile(args[0]);
                eFile.ExtractTo(XMLFilepath, 0);//Xml file that users can edit is in the end of a lqn file."0" means the last file.

                for (int i = 1; i < eFile.GetNowFileCount(); i++)
                {
                    AttachmentFiles.Add(i);//Initializing the attachment ArrayList,file index begins from "1".
                }
            }      

            App app = new App();
            app.InitializeComponent();
            app.Run();                
        }

        //Fade-in effect.
        private void loadDTimerEvent(object sender, EventArgs e)
        {
            if (mainWindowBorder.Opacity < 1)
            {
                mainWindowBorder.Opacity += 0.1;
                mainStackPanel.Opacity += 0.1;
            }
            else
            {
                ((DispatcherTimer)sender).Stop();            
            }
        }

        //Fade-out effect.
        private void unloadDTimerEvent(object sender, EventArgs e)
        {
            if (mainWindowBorder.Opacity > 0)
            {
                mainWindowBorder.Opacity -= 0.1;
                mainStackPanel.Opacity -= 0.1;
            }
            else
            {
                ((DispatcherTimer)sender).Stop();
                this.Close();
            }
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (filePath != "") { LoadContents(); }

            mainWindowBorder.Opacity = 0;
            mainStackPanel.Opacity = 0;

            DispatcherTimer loadDTimer = new DispatcherTimer();
            loadDTimer.Tick += loadDTimerEvent;
            loadDTimer.Interval = new TimeSpan(0, 0, 0, 0, 25);

            loadDTimer.Start();
        }

        private void openButtonMouseUp(object sender, MouseButtonEventArgs e)
        {
            OpenFile();  
        }

        private void OpenFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "LQN File|*.lqn";
            if (ofd.ShowDialog() == true)
            {
                if (isChanged == true)
                {
                    MessageBoxResult mbr = MessageBox.Show("Do you want to save the changes of this file?", "LiteQnaire", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (mbr == MessageBoxResult.Yes)
                    {
                        Save();

                    }
                    else if (mbr == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }

                LoadNewFile(ofd.FileName);
            }      
        }

        private void LoadNewFile(string filepath)
        {
            try { new DirectoryInfo(CacheFolder).Delete(true); }
            catch { }

            mainStackPanel.Children.Clear();

            CacheFolder = CurrentDirectory + @"\Cache\Cache" + new Random().Next(9999999).ToString();
            Directory.CreateDirectory(CacheFolder);

            filePath = filepath;//Setting the "filePath" to new file path.
            XMLFilepath = CacheFolder + @"\" + ConvertTo.FileName(filePath) + ".xml";

            ExtractFile eFile = new ExtractFile(filePath);
            eFile.ExtractTo(XMLFilepath, 0);

            AttachmentFiles = new ArrayList();
            for (int i = 1; i < eFile.GetNowFileCount(); i++)
            {
                AttachmentFiles.Add(i);
            }

            mainStackPanel.Height = 404;
            LoadContents();
        }

        private void closeButtonMouseUp(object sender, MouseButtonEventArgs e)
        {
            DispatcherTimer unloadDTimer = new DispatcherTimer();
            unloadDTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            unloadDTimer.Tick += unloadDTimerEvent;

            if (isChanged == false)
            {
                unloadDTimer.Start();
            }
            else
            {
                MessageBoxResult mbr = MessageBox.Show("Do you want to save the changes?", "LiteQnaire", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (mbr == MessageBoxResult.Yes)
                {
                    Save();
                    unloadDTimer.Start();
                }
                else if(mbr==MessageBoxResult.No)
                {
                    unloadDTimer.Start();
                }
            }        
        }

        private void saveButtonMouseUp(object sender, MouseButtonEventArgs e)
        {
            Save();
        }

        //To upgrade the progress bar.
        private delegate void UpdatePBDelegate(System.Windows.DependencyProperty dp, object value);      
        private void PBSetValue(double value)
        {
            UpdatePBDelegate updatePBDelegate = new UpdatePBDelegate(savePBar.SetValue);
            Dispatcher.Invoke(updatePBDelegate, System.Windows.Threading.DispatcherPriority.Background,new object[] { ProgressBar.ValueProperty, value });
        }


        private void Save()
        {
            if (isChanged == true)
            {
                savePBar.Visibility = Visibility.Visible;
                PBSetValue(0);

                hXML.Save();//Save the xml file in cache folder.
                PBSetValue(0.15);

                isChanged = false;

                ExtractFile eFile = new ExtractFile(filePath);
                eFile.ExtractTo(CacheFolder + @"\template.xml", 1);//Extracting the template xml file.
                PBSetValue(0.3);

                FileGlue fGlue = new FileGlue(filePath + ".temp");//Create a temp of source lqn file.
                fGlue.Glue(new string[1] { CacheFolder + @"\template.xml" }, eFile.GetMaxFileCount());//Index=1 will always be the template xml file.
                PBSetValue(0.55);
                
                //Gluing the attachment.
                foreach (var item in AttachmentFiles)
                {
                    if (item is int)//Attachments which have not been changed.
                    {
                        if ((int)item != 1) { fGlue.GlueStream(eFile.ExtractStream((int)item)); }
                    }
                    else//String type means the attachment have been changed or newly attached.
                    {
                        fGlue.GlueFile((string)item);
                    }
                }
                PBSetValue(0.8);

                fGlue.GlueFile(XMLFilepath);//Editable xml file will always be the last one.
                PBSetValue(0.9);

                //Recovering the source file.
                File.Delete(filePath);
                File.Move(filePath + ".temp", filePath);
                PBSetValue(1);

                savePBar.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        //Visualize the items.
        private void LoadContents()
        {
            saveButton.ToolTip = "Save " + ConvertTo.FileName(filePath) + "(Ctrl+S)";
            titleLabel.ToolTip = filePath;

            hXML = new HeadXMLFile(XMLFilepath);
            hXML.Read();        

            int i = -1;//block index
            int j = -1;//item index
            double totalHeight=0;
            foreach (block block in hXML.blocks)
            {
                i += 1;
                //The panel for putting items in.
                StackPanel tempStack = new StackPanel();
                tempStack.Height = 0;
                tempStack.Background = new SolidColorBrush(ConvertTo.Color(block.BackgroundColor));
                tempStack.Margin = new Thickness(20, 10, 20, 0);

                mainStackPanel.Children.Add(tempStack);
                
                foreach (var item in block.Contents)
                {
                    if (item is labelText)
                    {
                        j += 1;
                        Label tempLabel = new Label();
                        labelText XMLLabel=((labelText)item);

                        tempLabel.Content = XMLLabel.text;
                        tempLabel.FontSize = XMLLabel.fontsize;
                        tempLabel.Foreground = new SolidColorBrush(ConvertTo.Color(XMLLabel.forecolor));
                        if (XMLLabel.fontstyle.ToLower() == "bold") { tempLabel.FontWeight = FontWeights.Bold; }
                        tempLabel.FontStyle = ConvertTo.FontStyle(XMLLabel.fontstyle);
                        switch(XMLLabel.position){
                            case "left":
                              tempLabel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                              tempLabel.Margin = new Thickness(20, 8, 0, 0);
                              break;
                            case "right":
                              tempLabel.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                              tempLabel.Margin = new Thickness(0, 8, 20, 0);
                              break;
                            case "middle":
                              tempLabel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                              tempLabel.Margin = new Thickness(0, 8, 0, 0);
                              break;
                        }
                        tempLabel.Tag = i.ToString() + "," + j.ToString();

                        tempStack.Children.Add(tempLabel);
                        tempLabel.UpdateLayout();
                        tempStack.Height += (tempLabel.ActualHeight + 8);
                    }
                    else if (item is radioButton)
                    {
                        j += 1;
                        RadioButton tempRadio = new RadioButton();
                        radioButton XMLRadio = ((radioButton)item);

                        tempRadio.Content= XMLRadio.text;
                        tempRadio.FontSize = XMLRadio.fontsize;
                        tempRadio.Foreground = new SolidColorBrush(ConvertTo.Color(XMLRadio.forecolor));
                        if (XMLRadio.fontstyle.ToLower() == "bold") { tempRadio.FontWeight = FontWeights.Bold; }
                        tempRadio.FontStyle = ConvertTo.FontStyle(XMLRadio.fontstyle);
                        switch (XMLRadio.position)
                        {
                            case "left":
                                tempRadio.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                tempRadio.Margin = new Thickness(20, 8, 0, 0);
                                break;
                            case "right":
                                tempRadio.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                                tempRadio.Margin = new Thickness(0, 8, 20, 0);
                                break;
                            case "middle":
                                tempRadio.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                tempRadio.Margin = new Thickness(0, 8, 0, 0);
                                break;
                        }
                        tempRadio.IsChecked = XMLRadio.selected;
                        tempRadio.GroupName = i.ToString();
                        tempRadio.Click += RadioButton_Click;
                        tempRadio.Tag = i.ToString() + "," + j.ToString();

                        tempStack.Children.Add(tempRadio);
                        tempRadio.UpdateLayout();
                        tempStack.Height += (tempRadio.ActualHeight + 8);
                    }
                    else if (item is checkBox)
                    {
                        j += 1;
                        CheckBox tempCheck = new CheckBox();
                        checkBox XMLCheck = ((checkBox)item);

                        tempCheck.Content = XMLCheck.text;
                        tempCheck.FontSize = XMLCheck.fontsize;
                        tempCheck.Foreground = new SolidColorBrush(ConvertTo.Color(XMLCheck.forecolor));
                        if (XMLCheck.fontstyle.ToLower() == "bold") { tempCheck.FontWeight = FontWeights.Bold; }
                        tempCheck.FontStyle = ConvertTo.FontStyle(XMLCheck.fontstyle);
                        switch (XMLCheck.position)
                        {
                            case "left":
                                tempCheck.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                tempCheck.Margin = new Thickness(20, 8, 0, 0);
                                break;
                            case "right":
                                tempCheck.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                                tempCheck.Margin = new Thickness(0, 8, 20, 0);
                                break;
                            case "middle":
                                tempCheck.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                tempCheck.Margin = new Thickness(0, 8, 0, 0);
                                break;
                        }
                        tempCheck.IsChecked = XMLCheck.selected;
                        tempCheck.Click += CheckBox_Click;
                        tempCheck.Tag = i.ToString() + "," + j.ToString();

                        tempStack.Children.Add(tempCheck);
                        tempCheck.UpdateLayout();
                        tempStack.Height += (tempCheck.ActualHeight + 8);
                    }
                    else if (item is textBox)
                    {
                        j += 1;
                        TextBox tempText = new TextBox();
                        textBox XMLText = ((textBox)item);
                        tempText.Width = 350;
                       
                        tempText.FontSize = XMLText.fontsize;
                        tempText.Foreground = new SolidColorBrush(ConvertTo.Color(XMLText.forecolor));
                        if (XMLText.fontstyle.ToLower() == "bold") { tempText.FontWeight = FontWeights.Bold; }
                        tempText.FontStyle = ConvertTo.FontStyle(XMLText.fontstyle);
                        if (XMLText.isLong == true)
                        {
                            tempText.TextWrapping = TextWrapping.Wrap;
                            tempText.UpdateLayout();
                            tempText.Height = tempText.ActualHeight * 3.5;
                            tempText.Text = XMLText.text;
                            tempText.TextWrapping = System.Windows.TextWrapping.WrapWithOverflow;
                            tempText.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;
                            tempText.AcceptsReturn = true;
                        }
                        else
                        {
                            tempText.TextWrapping = TextWrapping.NoWrap;
                            tempText.Text = XMLText.text.Replace("\r\n","");
                        }
                        if (XMLText.isLocked == true) { tempText.IsReadOnly = true; }
                        switch (XMLText.position)
                        {
                            case "left":
                                tempText.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                tempText.Margin = new Thickness(20, 8, 0, 0);
                                break;
                            case "right":
                                tempText.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                                tempText.Margin = new Thickness(0, 8, 20, 0);
                                break;
                            case "middle":
                                tempText.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                tempText.Margin = new Thickness(0, 8, 0, 0);
                                break;
                        }
                        tempText.TextChanged += TextBox_Change;
                        tempText.Tag = i.ToString() + "," + j.ToString();

                        tempStack.Children.Add(tempText);
                        tempText.UpdateLayout();
                        tempStack.Height += (tempText.ActualHeight + 8);
                    }
                    else if (item is comboBox) {
                        j += 1;
                        ComboBox tempCombo = new ComboBox();
                        comboBox XMLCombo = ((comboBox)item);
                        tempCombo.Width = 200;

                        tempCombo.ItemsSource = XMLCombo.texts;
                        tempCombo.FontSize = XMLCombo.fontsize;
                        tempCombo.Foreground = new SolidColorBrush(ConvertTo.Color(XMLCombo.forecolor));
                        if (XMLCombo.fontstyle.ToLower() == "bold") { tempCombo.FontWeight = FontWeights.Bold; }
                        tempCombo.FontStyle = ConvertTo.FontStyle(XMLCombo.fontstyle);
                        switch (XMLCombo.position)
                        {
                            case "left":
                                tempCombo.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                tempCombo.Margin = new Thickness(20, 8, 0, 0);
                                break;
                            case "right":
                                tempCombo.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                                tempCombo.Margin = new Thickness(0, 8, 20, 0);
                                break;
                            case "middle":
                                tempCombo.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                tempCombo.Margin = new Thickness(0, 8, 0, 0);
                                break;
                        }
                        tempCombo.SelectedIndex = XMLCombo.selectedIndex;
                        tempCombo.SelectionChanged += ComboBox_Change;
                        tempCombo.Tag = i.ToString() + "," + j.ToString();

                        tempStack.Children.Add(tempCombo);
                        tempCombo.UpdateLayout();
                        tempStack.Height += (tempCombo.ActualHeight + 8);
                    }
                    else if (item is attachment)
                    {
                        j += 1;
                        WrapPanel tempWrap=new WrapPanel();//WrapPanel for "tempTextBlock" and "tempImage".
                        TextBlock tempTextBlock = new TextBlock();
                        Image tempImage = new Image();//Image for showing file icon.
                        attachment XMLAtt = ((attachment)item);
                        tempImage.Height = 16;
                        tempImage.Width = 16;
                        tempImage.Stretch = Stretch.Uniform;
                        tempTextBlock.TextDecorations = System.Windows.TextDecorations.Underline;
                        tempTextBlock.FontSize = 15;
                        tempTextBlock.Cursor = System.Windows.Input.Cursors.Hand;
                        tempTextBlock.Margin = new Thickness(6, 0, 0, 0);

                        tempTextBlock.Text = XMLAtt.text;
                        tempTextBlock.ToolTip = XMLAtt.filename;
                        /* When "fileIndex"<-1,there is a /Bubble/ at that place.
                           /Bubble/ is how the program delete an attached file.
                           Check how it works: "AttachmentWindow.Delete_Click".
                        */
                        if(XMLAtt.fileIndex<=-1)
                        {
                            tempImage.Source = attImg;
                            tempTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                            tempTextBlock.ToolTip = "Click to Add a File.";
                        }
                        else
                        {
                            GetIcon getIcon = new GetIcon();

                            BitmapSource bSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(getIcon.Get(ConvertTo.FileType(XMLAtt.filename),false).ToBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            tempImage.Source = bSource;
                            bSource = null;
                            getIcon = null;

                            tempTextBlock.Foreground = new SolidColorBrush(Colors.Blue);
                        }
                        switch (XMLAtt.position)
                        {
                            case "left":
                                tempWrap.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                tempWrap.Margin = new Thickness(20, 8, 0, 0);
                                break;
                            case "right":
                                tempWrap.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                                tempWrap.Margin = new Thickness(0, 8, 20, 0);
                                break;
                            case "middle":
                                tempWrap.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                tempWrap.Margin = new Thickness(0, 8, 0, 0);
                                break;
                        }
                        tempTextBlock.MouseLeftButtonUp += Attachment_Click;
                        tempTextBlock.MouseMove += Label_MouseMove;
                        tempTextBlock.MouseLeave += Label_MouseLeave;
                        tempTextBlock.Tag = i.ToString() + "," + j.ToString();
                        
                        tempWrap.Children.Add(tempImage);
                        tempWrap.Children.Add(tempTextBlock);
                        tempStack.Children.Add(tempWrap);               
                        tempWrap.UpdateLayout();
                        tempStack.Height += (tempWrap.ActualHeight + 8);
                    }

                    Border split = new Border();
                    split.Height = 0.5;
                    split.BorderThickness = new Thickness(1, 1, 1, 1);
                    split.Margin = new Thickness(0, 8, 0, 0);
                    split.BorderBrush = new SolidColorBrush(ConvertTo.Color(block.SplitColor));
                    tempStack.Children.Add(split);
                    tempStack.Height += (split.Height + 9.5);
                }

                tempStack.UpdateLayout();
                totalHeight += (tempStack.ActualHeight + 10);
                if (totalHeight >= mainStackPanel.Height) { mainStackPanel.Height = totalHeight; }//If total height of blocks > 404,set the "mainStackPanel" height to "totalHeight".
                j = -1;
            }          
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            isChanged = true;

            string tag = ((RadioButton)sender).Tag.ToString();
            int i= (ConvertTo.IandJ(tag))[0];
            int j = (ConvertTo.IandJ(tag))[1];

            //Update other radio buttons in the same block.
            for(int k=0;k<((block)hXML.blocks[i]).Contents.Count;k++)
            {
                if (((block)hXML.blocks[i]).Contents[k] is radioButton)
                {
                    radioButton otherRadio;
                    otherRadio = ((radioButton)((block)hXML.blocks[i]).Contents[k]);
                    otherRadio.selected = false;
                    ((block)hXML.blocks[i]).Contents[k] = otherRadio;
                }               
            }

            radioButton tempRadio;
            tempRadio = ((radioButton)((block)hXML.blocks[i]).Contents[j]);
            tempRadio.selected = Convert.ToBoolean(((RadioButton)sender).IsChecked);

            ((block)hXML.blocks[i]).Contents[j] = tempRadio;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            isChanged = true;
            string tag = ((CheckBox)sender).Tag.ToString();
            int i = (ConvertTo.IandJ(tag))[0];
            int j = (ConvertTo.IandJ(tag))[1];
            checkBox tempCheck;
            tempCheck = ((checkBox)((block)hXML.blocks[i]).Contents[j]);
            tempCheck.selected = Convert.ToBoolean(((CheckBox)sender).IsChecked);

            ((block)hXML.blocks[i]).Contents[j] = tempCheck;
        }

        private void Attachment_Click(object sender, MouseButtonEventArgs e)
        {
            int i = ConvertTo.IandJ(((TextBlock)sender).Tag.ToString())[0];
            int j = ConvertTo.IandJ(((TextBlock)sender).Tag.ToString())[1];
            attachment tempAtt = ((attachment)((block)hXML.blocks[i]).Contents[j]);

            if (tempAtt.fileIndex <= -1)//No attachment.
            {
                OpenFileDialog ofd = new OpenFileDialog();
                string filters = "Extension|";
                foreach (string filter in tempAtt.filter)
                {
                    filters += filter + ";";
                }
                filters = filters.TrimEnd(';');
                ofd.Filter = filters;
                ofd.Title = "Select a File as Attachment.";

                bool? open = ofd.ShowDialog();
                if (open == true)
                {
                    isChanged = true;

                    if (tempAtt.fileIndex < -1)
                    {
                        AttachmentFiles[-(tempAtt.fileIndex) - 1] = ofd.FileName;
                        tempAtt.fileIndex = -(tempAtt.fileIndex);
                    }
                    else
                    {
                        AttachmentFiles.Add(ofd.FileName);
                        tempAtt.fileIndex = AttachmentFiles.Count;
                    }
                    tempAtt.filename = ConvertTo.FileName(ofd.FileName);

                    ((TextBlock)sender).ToolTip = tempAtt.filename;
                    ((TextBlock)sender).Foreground = new SolidColorBrush(Colors.Blue);
                    ((block)hXML.blocks[i]).Contents[j] = tempAtt;
               
                    nowTextBlock = (TextBlock)sender;//Let "AttachmentWindow" know which TextBlock is under editing.
                    nowTextBlock.Foreground = new SolidColorBrush(Colors.Blue);
                    
                    AttachmentWindow attWindow = new AttachmentWindow();
                    attWindow.Owner = this;
                    attWindow.ShowDialog();
                }               
            }
            else//Directly open "AttachmentWindow" when there is an attachment.
            {
                nowTextBlock = (TextBlock)sender;
                AttachmentWindow attWindow = new AttachmentWindow();
                attWindow.Owner = this;
                attWindow.ShowDialog();
            }
        }

        private void ComboBox_Change(object sender, SelectionChangedEventArgs e)
        {
            isChanged = true;

            string tag = ((ComboBox)sender).Tag.ToString();
            int i = (ConvertTo.IandJ(tag))[0];
            int j = (ConvertTo.IandJ(tag))[1];

            comboBox tempCombo;
            tempCombo = ((comboBox)((block)hXML.blocks[i]).Contents[j]);
            tempCombo.selectedIndex = ((ComboBox)sender).SelectedIndex;

            ((block)hXML.blocks[i]).Contents[j] = tempCombo;
        }

        private void TextBox_Change(object sender, TextChangedEventArgs e)
        {
            isChanged = true;

            string tag = ((TextBox)sender).Tag.ToString();
            int i = (ConvertTo.IandJ(tag))[0];
            int j = (ConvertTo.IandJ(tag))[1];

            textBox tempText;
            tempText = ((textBox)((block)hXML.blocks[i]).Contents[j]);
            tempText.text = ((TextBox)sender).Text;

            ((block)hXML.blocks[i]).Contents[j] = tempText;
        }

        private void dragMove_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void minimizeButtonMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void minimizeButtonMouseMove(object sender, MouseEventArgs e)
        {          
            minimizeButton.Source = minimizemoveButtonImg;           
        }

        private void minimizeButtonMouseLeave(object sender, MouseEventArgs e)
        {           
            minimizeButton.Source = minimizeButtonImg;
        }

        private void openButtonMouseMove(object sender, MouseEventArgs e)
        {
            openButton.Source = openmoveButtonImg;
        }

        private void openButtonMouseLeave(object sender, MouseEventArgs e)
        {
            openButton.Source = openButtonImg;
        }

        private void closeButtonMouseMove(object sender, MouseEventArgs e)
        {
            closeButton.Source = closemoveButtonImg;
        }

        private void closeButtonMouseLeave(object sender, MouseEventArgs e)
        {
            closeButton.Source = closeButtonImg;
        }

        private void saveButtonMouseMove(object sender, MouseEventArgs e)
        {
            saveButton.Source = savemoveButtonImg;
        }

        private void saveButtonMouseLeave(object sender, MouseEventArgs e)
        {
            saveButton.Source = saveButtonImg;
        }

        private void Label_MouseMove(object sender, MouseEventArgs e)
        {
            int i = ConvertTo.IandJ(((TextBlock)sender).Tag.ToString())[0];
            int j = ConvertTo.IandJ(((TextBlock)sender).Tag.ToString())[1];

            if (((attachment)((block)hXML.blocks[i]).Contents[j]).fileIndex > -1)
            {
                ((TextBlock)sender).Foreground = new SolidColorBrush(Colors.SkyBlue);
                
            }
            else
            {
                ((TextBlock)sender).Foreground = new SolidColorBrush(Colors.IndianRed);
            }
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            int i = ConvertTo.IandJ(((TextBlock)sender).Tag.ToString())[0];
            int j = ConvertTo.IandJ(((TextBlock)sender).Tag.ToString())[1];

            if (((attachment)((block)hXML.blocks[i]).Contents[j]).fileIndex > -1)
            {
                ((TextBlock)sender).Foreground = new SolidColorBrush(Colors.Blue);
            }
            else
            {
                ((TextBlock)sender).Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Keyboard.IsKeyDown(Key.S))
            {
                Save();
            }
            else if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Keyboard.IsKeyDown(Key.O))
            {
                OpenFile();
            }
        }

        //When a file dropping to the window.
        private void File_Drop(object sender, DragEventArgs e)
        {
            string filename = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            if (ConvertTo.FileType(filename).ToLower() == ".lqn")
            {
                if (isChanged == true)
                {
                    MessageBoxResult mbr = MessageBox.Show("Do you want to save the changes?", "LiteQnaire", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                    if (mbr == MessageBoxResult.Yes)
                    {
                        Save();
                    }
                    else if (mbr == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }
                LoadNewFile(filename);
            }
        }

        private void Window_Unloaded(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                new DirectoryInfo(CacheFolder).Delete(true);
            }
            catch
            {

            }
        }
    }
}
