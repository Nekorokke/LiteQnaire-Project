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
using HeadXMLLib;
using FileGlueLib;
using ExtractFileLib;
using System.IO;
using System.Collections;
using Microsoft.Win32;

namespace LiteQnaireEditor
{
    /// <summary>
    /// MainWindow.xaml
    /// Some functions are same to their in "LiteQnaire",no more comments.
    /// </summary>
    public partial class MainWindow : Window
    {
        public static BitmapImage attImg;
        public static BitmapImage labelImg;
        public static BitmapImage textImg;
        public static BitmapImage radioImg;
        public static BitmapImage checkImg;
        public static BitmapImage comboImg;

        public static BitmapImage deleteImg;
        public static BitmapImage deletemoveImg;

        BitmapImage upImg;
        BitmapImage upmoveImg;

        BitmapImage downImg;
        BitmapImage downmoveImg;

        BitmapImage editImg;
        BitmapImage editmoveImg;

        BitmapImage newfileImg;
        BitmapImage newfilemoveImg;

        BitmapImage openImg;
        BitmapImage openmoveImg;

        BitmapImage additemImg;
        BitmapImage additemmoveImg;

        BitmapImage saveImg;
        BitmapImage savemoveImg;

        BitmapImage saveasImg;
        BitmapImage saveasmoveImg;

        BitmapImage addImg;

        ContextMenu blockPopMenu = new ContextMenu();
        ContextMenu itemPopMenu = new ContextMenu();
        ContextMenu pasteItemMenu = new ContextMenu();
        ContextMenu pasteBlockMenu = new ContextMenu();

        public static string CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
        public static string filePath = "";
        static string CacheFolder="";
        static string XMLFilepath="";

        static ExtractFile eFile;
        public static HeadXMLFile hXML;

        public static Boolean isChanged = false;

        int nowSelectedIndex = -1;//Show which block is now selected.

        double totalHeight = 0;

        public MainWindow()
        {
            InitializeComponent();

            attImg=new BitmapImage(new Uri(CurrentDirectory+@"\Image\attachment.png",UriKind.Absolute));
            radioImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\radiobutton.png", UriKind.Absolute));
            textImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\textbox.png", UriKind.Absolute));
            comboImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\combobox.png", UriKind.Absolute));
            checkImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\checkbox.png", UriKind.Absolute));
            labelImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\labeltext.png", UriKind.Absolute));

            upImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\up.png", UriKind.Absolute));
            upmoveImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\up_move.png", UriKind.Absolute));
            itemupButton.Source = upImg;
            upButton.Source = upImg;

            downImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\down.png", UriKind.Absolute));
            downmoveImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\down_move.png", UriKind.Absolute));
            itemdownButton.Source = downImg;
            downButton.Source = downImg;

            editImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\edit.png", UriKind.Absolute));
            editmoveImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\edit_move.png", UriKind.Absolute));
            itemeditButton.Source = editImg;
            editButton.Source = editImg;

            deleteImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\delete.png", UriKind.Absolute));
            deletemoveImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\delete_move.png", UriKind.Absolute));
            itemdeleteButton.Source = deleteImg;
            deleteButton.Source = deleteImg;

            newfileImg=new BitmapImage(new Uri(CurrentDirectory+@"\Image\newfile.png",UriKind.Absolute));
            newfilemoveImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\newfile_move.png", UriKind.Absolute));
            newfileButton.Source = newfileImg;

            openImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\open.png", UriKind.Absolute));
            openmoveImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\open_move.png", UriKind.Absolute));
            openButton.Source = openImg;

            saveImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\save.png", UriKind.Absolute));
            savemoveImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\save_move.png", UriKind.Absolute));
            saveButton.Source = saveImg;

            saveasImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\saveas.png", UriKind.Absolute));
            saveasmoveImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\saveas_move.png", UriKind.Absolute));
            saveasButton.Source = saveasImg;

            additemImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\additem.png", UriKind.Absolute));
            additemmoveImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\additem_move.png", UriKind.Absolute));
            additemButton.Source = additemImg;

            addImg=new BitmapImage(new Uri(CurrentDirectory + @"\Image\AddBlock.png", UriKind.Absolute));

            nowSelectedText.Text = " " + ConvertTo.FileName(filePath);
            saveButton.ToolTip = "Save " + ConvertTo.FileName(filePath) + " (Ctrl+S)";
            BlockButtonHide();
            
            MenuItem blockEditMenu = new MenuItem();
            blockEditMenu.Header = "Edit";
            blockEditMenu.Click += BlockEditButtonClick;
            blockPopMenu.Items.Add(blockEditMenu);

            blockPopMenu.Items.Add(new Separator());

            MenuItem blockCopyMenu = new MenuItem();
            blockCopyMenu.Header = "Copy";
            blockCopyMenu.Click += CopyBlock_Click;
            blockPopMenu.Items.Add(blockCopyMenu);

            blockPopMenu.Items.Add(new Separator());

            MenuItem blockUpMenu = new MenuItem();
            blockUpMenu.Header = "Move Up";
            blockUpMenu.Click += blockupButtonClick;
            blockPopMenu.Items.Add(blockUpMenu);

            MenuItem blockDownMenu = new MenuItem();
            blockDownMenu.Header = "Move Down";
            blockDownMenu.Click += blockdownButtonClick;
            blockPopMenu.Items.Add(blockDownMenu);

            blockPopMenu.Items.Add(new Separator());

            MenuItem blockDeleteMenu = new MenuItem();
            blockDeleteMenu.Header = "Delete";
            blockDeleteMenu.Click += BlockDeleteButtonClick;
            blockPopMenu.Items.Add(blockDeleteMenu);

            //

            MenuItem itemEditMenu = new MenuItem();
            itemEditMenu.Header = "Edit";
            itemEditMenu.Click += itemEditButtonClick;
            itemPopMenu.Items.Add(itemEditMenu);

            itemPopMenu.Items.Add(new Separator());

            MenuItem itemCopyMenu = new MenuItem();
            itemCopyMenu.Header = "Copy (Ctrl+C)";
            itemCopyMenu.Click += CopyItem_Click;
            itemPopMenu.Items.Add(itemCopyMenu);

            MenuItem itemCutMenu = new MenuItem();
            itemCutMenu.Header = "Cut (Ctrl+X)";
            itemCutMenu.Click += CutItem_Click;
            itemPopMenu.Items.Add(itemCutMenu);

            itemPopMenu.Items.Add(new Separator());

            MenuItem itemUpMenu = new MenuItem();
            itemUpMenu.Header = "Move Up";
            itemUpMenu.Click += itemupButtonClick;
            itemPopMenu.Items.Add(itemUpMenu);

            MenuItem itemDownMenu = new MenuItem();
            itemDownMenu.Header = "Move Down";
            itemDownMenu.Click += itemdownButtonClick;
            itemPopMenu.Items.Add(itemDownMenu);

            itemPopMenu.Items.Add(new Separator());

            MenuItem itemDeleteMenu = new MenuItem();
            itemDeleteMenu.Header = "Delete";
            itemDeleteMenu.Click += itemdeleteButtonClick;
            itemPopMenu.Items.Add(itemDeleteMenu);

            //

            MenuItem itemPasteMenu = new MenuItem();
            itemPasteMenu.Header = "Paste (Ctrl+V)";
            itemPasteMenu.Click += PasteItem_Click;
            pasteItemMenu.Items.Add(itemPasteMenu);

            MenuItem blockPasteMenu = new MenuItem();
            blockPasteMenu.Header = "Paste";
            blockPasteMenu.Click += PasteBlock_Click;
            pasteBlockMenu.Items.Add(blockPasteMenu);
        }

        private void BlockButtonVisible()
        {
            editButton.Visibility = Visibility.Visible;
            upButton.Visibility = Visibility.Visible;
            downButton.Visibility = Visibility.Visible;
            deleteButton.Visibility = Visibility.Visible;
        }

        private void BlockButtonHide()
        {
            editButton.Visibility = Visibility.Hidden;
            upButton.Visibility = Visibility.Hidden;
            downButton.Visibility = Visibility.Hidden;
            deleteButton.Visibility = Visibility.Hidden;
        }


        [STAThread]
        public static void Main(string[] args)
        {
             if (args.Count() != 0)
             {

                 loadFile(args[0]);
             }
             else
             {
                 newFile();
             }

            App app = new App();
            app.InitializeComponent();
            app.Run();
        }

        private static void loadFile(string filepath)
        {
            CacheFolder = CurrentDirectory + @"\Cache\Cache" + new Random().Next(9999999).ToString();
            Directory.CreateDirectory(CacheFolder);

            filePath = filepath;
            XMLFilepath = CacheFolder + @"\" + ConvertTo.FileName(filePath) + ".xml";

            eFile = new ExtractFile(filepath);
            eFile.ExtractTo(XMLFilepath, 1);
            eFile.ExtractTo(XMLFilepath + "_chk", 0);//Extract editable xml file to check if it has an attachment.
        }

        private static void newFile()
        {
            CacheFolder = CurrentDirectory + @"\Cache\Cache" + new Random().Next(9999999).ToString();
            Directory.CreateDirectory(CacheFolder);

            XMLFilepath = CacheFolder + @"\Unnamed.xml";
            filePath = CacheFolder + @"\Unnamed.lqn";

            File.Copy(CurrentDirectory + @"\Templates\NewFile.xml", XMLFilepath);//NewFile.xml has been prepared in advance.
            File.Copy(CurrentDirectory + @"\Templates\NewFile.xml", XMLFilepath + "_chk");
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            LoadContents();
            LoadItemCombo();

            ItemList.ContextMenu = pasteItemMenu;
            scroll.ContextMenu = pasteBlockMenu;

            isChanged = false;
        }

        private void LoadContents()
        {
            hXML = new HeadXMLFile(XMLFilepath + "_chk");
            if (hXML.Read(true) == true)//Check if it has an attachment.
            {
                System.Threading.Thread mbThread = new System.Threading.Thread
                (
                    () =>
                    {
                        this.Dispatcher.Invoke
                           (
                            new Action(
                            () =>
                            {
                                MessageBox.Show("Edit and save this file will lose it's attachments.", "LiteQnaire Editor", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            }
                                      )
                           );
                    }
                );
                mbThread.Start();
            }

            hXML = new HeadXMLFile(XMLFilepath);
            hXML.Read();//Read the template xml file.

            totalHeight = 0;
            foreach (block block in hXML.blocks)
            {
                LoadBlock(block, -1);
            }

            AddButtonLoad();
        }

        private void LoadBlock(block block, int insertTo)
        {
            //The Border for marking out which block users have selected.
            Border tempBorder = new Border();
            tempBorder.BorderBrush = null;
            tempBorder.BorderThickness = new Thickness(2);
            tempBorder.Cursor = Cursors.Hand;

            StackPanel tempStack = new StackPanel();
            tempStack.Height = 0;

            tempBorder.MouseUp += blockPanel_Click;
            tempBorder.MouseMove += blockPanel_MouseMove;
            tempBorder.MouseLeave += blockPanel_MouseLeave;

            tempBorder.Child = tempStack;

            if (insertTo == -1) { mainStackPanel.Children.Add(tempBorder); }//When "insertTo" = -1,it means to add the block to the end.
            else { mainStackPanel.Children.Insert(insertTo, tempBorder); }

            tempStack.Background = new SolidColorBrush(ConvertTo.Color(block.BackgroundColor));

            tempBorder.Margin = new Thickness(20, 8, 20, 0);

            foreach (var item in block.Contents)
            {
                string random = new Random().Next(1000000).ToString();//RadioButton group name.

                if (item is labelText)
                {
                    Label tempLabel = new Label();
                    labelText XMLLabel = ((labelText)item);

                    tempLabel.Content = XMLLabel.text;
                    tempLabel.FontSize = XMLLabel.fontsize;
                    tempLabel.Foreground = new SolidColorBrush(ConvertTo.Color(XMLLabel.forecolor));
                    if (XMLLabel.fontstyle.ToLower() == "bold") { tempLabel.FontWeight = FontWeights.Bold; }
                    tempLabel.FontStyle = ConvertTo.FontStyle(XMLLabel.fontstyle);

                    tempStack.Children.Add(tempLabel);
                    tempStack.ContextMenu = blockPopMenu;

                    switch (XMLLabel.position)
                    {
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
                    tempLabel.UpdateLayout();
                    tempStack.Height += (tempLabel.ActualHeight + 8);
                }
                else if (item is radioButton)
                {
                    RadioButton tempRadio = new RadioButton();
                    radioButton XMLRadio = ((radioButton)item);

                    tempRadio.Content = XMLRadio.text;
                    tempRadio.FontSize = XMLRadio.fontsize;
                    tempRadio.Foreground = new SolidColorBrush(ConvertTo.Color(XMLRadio.forecolor));
                    if (XMLRadio.fontstyle.ToLower() == "bold") { tempRadio.FontWeight = FontWeights.Bold; }
                    tempRadio.FontStyle = ConvertTo.FontStyle(XMLRadio.fontstyle);

                    tempStack.Children.Add(tempRadio);
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
                    tempRadio.UpdateLayout();
                    tempStack.Height += (tempRadio.ActualHeight + 8);
                    tempRadio.IsChecked = XMLRadio.selected;
                    tempRadio.GroupName = random;
                    tempRadio.IsEnabled = false;
                }
                else if (item is checkBox)
                {
                    CheckBox tempCheck = new CheckBox();
                    checkBox XMLCheck = ((checkBox)item);

                    tempCheck.Content = XMLCheck.text;
                    tempCheck.FontSize = XMLCheck.fontsize;
                    tempCheck.Foreground = new SolidColorBrush(ConvertTo.Color(XMLCheck.forecolor));
                    if (XMLCheck.fontstyle.ToLower() == "bold") { tempCheck.FontWeight = FontWeights.Bold; }
                    tempCheck.FontStyle = ConvertTo.FontStyle(XMLCheck.fontstyle);

                    tempStack.Children.Add(tempCheck);
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
                    tempCheck.UpdateLayout();
                    tempStack.Height += (tempCheck.ActualHeight + 8);
                    tempCheck.IsChecked = XMLCheck.selected;
                    tempCheck.IsEnabled = false;
                }
                else if (item is textBox)
                {
                    TextBox tempText = new TextBox();
                    textBox XMLText = ((textBox)item);

                    tempText.Width = 360;

                    tempText.FontSize = XMLText.fontsize;
                    tempText.Foreground = new SolidColorBrush(ConvertTo.Color(XMLText.forecolor));
                    if (XMLText.fontstyle.ToLower() == "bold") { tempText.FontWeight = FontWeights.Bold; }
                    tempText.FontStyle = ConvertTo.FontStyle(XMLText.fontstyle);

                    tempStack.Children.Add(tempText);

                    if (XMLText.isLong == true)
                    {
                        tempText.TextWrapping = System.Windows.TextWrapping.Wrap;
                        tempText.UpdateLayout();
                        tempText.Height = tempText.ActualHeight * 3.5;
                        tempText.TextWrapping = System.Windows.TextWrapping.WrapWithOverflow;
                        tempText.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;
                        tempText.AcceptsReturn = true;
                        tempText.Text = XMLText.text;
                    }
                    else
                    {
                        tempText.TextWrapping = System.Windows.TextWrapping.NoWrap;
                        tempText.Text = XMLText.text.Replace("\r\n", "");
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
                    tempText.UpdateLayout();
                    tempStack.Height += (tempText.ActualHeight + 8);
                    tempText.IsEnabled = false;
                }
                else if (item is comboBox)
                {
                    ComboBox tempCombo = new ComboBox();
                    comboBox XMLCombo = ((comboBox)item);

                    tempCombo.ItemsSource = XMLCombo.texts;
                    tempCombo.FontSize = XMLCombo.fontsize;
                    tempCombo.Foreground = new SolidColorBrush(ConvertTo.Color(XMLCombo.forecolor));
                    if (XMLCombo.fontstyle.ToLower() == "bold") { tempCombo.FontWeight = FontWeights.Bold; }
                    tempCombo.FontStyle = ConvertTo.FontStyle(XMLCombo.fontstyle);

                    tempStack.Children.Add(tempCombo);

                    tempCombo.Width = 200;

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
                    tempCombo.UpdateLayout();
                    tempStack.Height += (tempCombo.ActualHeight + 8);
                    tempCombo.SelectedIndex = XMLCombo.selectedIndex;
                    tempCombo.IsEnabled = false;
                }
                else if (item is attachment)
                {
                    WrapPanel tempWrap = new WrapPanel();
                    TextBlock tempTextBlock = new TextBlock();
                    Image tempImage = new Image();
                    attachment XMLAtt = ((attachment)item);

                    tempImage.Height = 16;
                    tempImage.Width = 16;
                    tempImage.Stretch = Stretch.Uniform;

                    tempTextBlock.Text = XMLAtt.text;

                    if (XMLAtt.fileIndex <= -1)
                    {
                        tempImage.Source = attImg;
                        tempTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        GetIcon getIcon = new GetIcon();

                        BitmapSource bSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(getIcon.Get(ConvertTo.FileType(XMLAtt.filename), false).ToBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        tempImage.Source = bSource;
                        bSource = null;
                        getIcon = null;

                        tempTextBlock.Foreground = new SolidColorBrush(Colors.Blue);
                    }
                    tempTextBlock.TextDecorations = System.Windows.TextDecorations.Underline;
                    tempTextBlock.FontSize = 15;
                    tempTextBlock.Cursor = System.Windows.Input.Cursors.Hand;

                    tempWrap.Children.Add(tempImage);
                    tempWrap.Children.Add(tempTextBlock);

                    tempStack.Children.Add(tempWrap);

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

            tempBorder.UpdateLayout();
            totalHeight += (tempBorder.ActualHeight + 10);
            UpdateMainStackHeight();
        }
     
        private void UpdateMainStackHeight()
        {
            isChanged = true;

            if (totalHeight < 401) { mainStackPanel.Height = 401; }
            else { mainStackPanel.Height = totalHeight; }
        }

        //Load the add button.
        private void AddButtonLoad()
        {
            Image AddBlock = new Image();
            AddBlock.Height = 32;
            AddBlock.Width = 32;
            AddBlock.Stretch = Stretch.Uniform;
            AddBlock.Margin = new Thickness(0, 8, 0, 0);
            AddBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            AddBlock.Cursor = Cursors.Hand;
            AddBlock.Source = addImg;
            AddBlock.MouseLeftButtonUp += AddBlockButtonClick;

            mainStackPanel.Children.Add(AddBlock);

            totalHeight += 32 + 8;

            UpdateMainStackHeight();
        }

        private void AddBlockButtonClick(object sender, MouseButtonEventArgs e)
        {
            AddBlock(-1);
        }

        private void AddBlock(int index)
        {
            AddButtonUnload();

            //Create a new block with labelText.
            block tempBlock = new block(true);
            labelText tempLabel = new labelText(true);
            tempLabel.text = "New Block";
            tempLabel.position = "middle";
            tempBlock.Contents.Add(tempLabel);
            hXML.blocks.Add(tempBlock);

            LoadBlock((block)hXML.blocks[hXML.blocks.Count - 1], index);

            Block_Select(hXML.blocks.Count - 1);

            AddButtonLoad();

            scroll.ScrollToEnd();
        }

        //While a block is selected.
        private void Block_Select(int index)
        {
            //Hide the border selected before.
            if (nowSelectedIndex != -1 && nowSelectedIndex != mainStackPanel.Children.Count - 1)
            {
                (mainStackPanel.Children[nowSelectedIndex] as Border).BorderBrush = null;
                (mainStackPanel.Children[nowSelectedIndex] as Border).Effect = null;
            }

            System.Windows.Media.Effects.DropShadowEffect dsEffect = new System.Windows.Media.Effects.DropShadowEffect();
            dsEffect.BlurRadius = 1.5;
            dsEffect.Color = Colors.LightBlue;

            (mainStackPanel.Children[index] as Border).Effect = dsEffect;
            (mainStackPanel.Children[index] as Border).BorderBrush = new SolidColorBrush(Colors.LightBlue);

            nowSelectedIndex = index;

            nowSelectedText.Text = " Now Selected: Block (" + (nowSelectedIndex + 1) + ")";

            LoadItemList(nowSelectedIndex);

            BlockButtonVisible();
        }

        //Load the "ListBox ItemList" in the left hand of window.
        private void LoadItemList(int index)
        {
            ItemList.Items.Clear();

            foreach (var item in ((block)hXML.blocks[index]).Contents)
            {
                if (item is labelText)
                {
                    LoadItem(labelImg, "Text", "ItemList");
                }
                else if (item is radioButton)
                {
                    LoadItem(radioImg, "RadioButton", "ItemList");
                }
                else if (item is checkBox)
                {
                    LoadItem(checkImg, "CheckBox", "ItemList");
                }
                else if (item is comboBox)
                {
                    LoadItem(comboImg, "ComboBox", "ItemList");
                }
                else if (item is attachment)
                {
                    LoadItem(attImg, "Attachment", "ItemList");
                }
                else if (item is textBox)
                {
                    LoadItem(textImg, "TextBox", "ItemList");
                }

                ItemList.SelectedIndex = 0;
            }
        }

        //Load the "ComboBox ItemCombo" over the "ItemList".
        private void LoadItemCombo()
        {
            LoadItem(labelImg, "Text", "ItemCombo");

            LoadItem(radioImg, "RadioButton", "ItemCombo");

            LoadItem(checkImg, "CheckBox", "ItemCombo");

            LoadItem(comboImg, "ComboBox", "ItemCombo");

            LoadItem(attImg, "Attachment", "ItemCombo");


            LoadItem(textImg, "TextBox", "ItemCombo");

            ItemCombo.SelectedIndex = 0;
        }

        //Load options to the "ItemList" or "ItemCombo".
        private void LoadItem(BitmapImage icon, string text, string Parent)
        {
            WrapPanel tempDock = new WrapPanel();

            Image tempImg = new Image();
            tempImg.Stretch = Stretch.Fill;
            tempImg.Height = 12;
            tempImg.Width = 12;

            tempImg.Source = icon;
            tempDock.Children.Add(tempImg);

            Label tempLabel = new Label();
            tempLabel.Content = text;
            tempLabel.FontWeight = FontWeights.Bold;
            tempLabel.FontSize = 11;
            tempLabel.Foreground = new SolidColorBrush(Colors.Gray);
            tempLabel.Width = 100;
            tempLabel.Margin = new Thickness(0, 0, 0, 0);
            tempLabel.Cursor = Cursors.Hand;

            tempDock.Children.Add(tempLabel);
            tempLabel.MouseDoubleClick += itemEditButtonClick;

            if (Parent == "ItemList")
            {
                tempDock.ContextMenu = itemPopMenu;
                ItemList.Items.Add(tempDock);
            }
            else if (Parent == "ItemCombo")
            {
                tempLabel.Margin = new Thickness(0, -5, 0, 0);
                tempImg.Margin = new Thickness(0, -5, 0, 0);
                ItemCombo.Items.Add(tempDock);
            }
        }

        private void AddButtonUnload()
        {
            mainStackPanel.Children.RemoveAt(mainStackPanel.Children.Count - 1);

            totalHeight -= (32 + 8);
            UpdateMainStackHeight();
        }

        private void newfileButton_Click(object sender, MouseButtonEventArgs e)
        {
            NewFile_Click();
        }

        private void NewFile_Click()
        {
            if (SaveMessage() == null) { return; }

            CleanCache();
            mainStackPanel.Children.Clear();
            ItemList.Items.Clear();

            newFile();
            LoadContents();

            NothingSelected();
            saveButton.ToolTip = "Save " + ConvertTo.FileName(filePath) + " (Ctrl+S)";
        }

        //No block is selected.
        private void NothingSelected()
        {
            nowSelectedText.Text = " " + ConvertTo.FileName(filePath);
            nowSelectedIndex = -1;
            BlockButtonHide();
            ItemList.Items.Clear();
        }

        //Exchange two blocks.
        private void ExchangeBlock(int indexNow, int indexTarget)
        {
            if (indexNow > indexTarget) { int temp; temp = indexTarget; indexTarget = indexNow; indexNow = temp; }//Always let indexNow < indexTarget.

            (mainStackPanel.Children[indexNow] as Border).UpdateLayout();
            totalHeight -= ((mainStackPanel.Children[indexNow] as Border).ActualHeight + 10);
            mainStackPanel.Children.RemoveAt(indexNow);

            (mainStackPanel.Children[indexTarget - 1] as Border).UpdateLayout();
            totalHeight -= ((mainStackPanel.Children[indexTarget - 1] as Border).ActualHeight + 10);
            mainStackPanel.Children.RemoveAt(indexTarget - 1);

            UpdateMainStackHeight();

            block tempBlock = (block)hXML.blocks[indexNow];
            hXML.blocks[indexNow] = (block)hXML.blocks[indexTarget];
            hXML.blocks[indexTarget] = tempBlock;

            LoadBlock((block)hXML.blocks[indexNow], indexNow);
            LoadBlock((block)hXML.blocks[indexTarget], indexTarget);
        }

        //MouseDown Event version.
        private void blockUuButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (nowSelectedIndex != 0)//Not the first block.
            {
                ExchangeBlock(nowSelectedIndex, nowSelectedIndex - 1);
                Block_Select(nowSelectedIndex - 1);
                scroll.ScrollToVerticalOffset(scroll.ContentVerticalOffset - (mainStackPanel.Children[nowSelectedIndex] as Border).ActualHeight);
            }
        }

        //Click Event version.
        private void blockupButtonClick(object sender, RoutedEventArgs e)
        {
            if (nowSelectedIndex != 0)
            {
                ExchangeBlock(nowSelectedIndex, nowSelectedIndex - 1);
                Block_Select(nowSelectedIndex - 1);
                scroll.ScrollToVerticalOffset(scroll.ContentVerticalOffset - (mainStackPanel.Children[nowSelectedIndex] as Border).ActualHeight);
            }
        }     

        private void blockdownButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (nowSelectedIndex != mainStackPanel.Children.Count - 2)//Not the last block.
            {
                ExchangeBlock(nowSelectedIndex, nowSelectedIndex + 1);
                Block_Select(nowSelectedIndex + 1);
                scroll.ScrollToVerticalOffset(scroll.ContentVerticalOffset + (mainStackPanel.Children[nowSelectedIndex] as Border).ActualHeight);
            }
        }

        private void blockdownButtonClick(object sender, RoutedEventArgs e)
        {
            if (nowSelectedIndex != mainStackPanel.Children.Count - 1)
            {
                ExchangeBlock(nowSelectedIndex, nowSelectedIndex + 1);
                Block_Select(nowSelectedIndex + 1);
                scroll.ScrollToVerticalOffset(scroll.ContentVerticalOffset + (mainStackPanel.Children[nowSelectedIndex] as Border).ActualHeight);
            }
        }

        private void BlockDeleteButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Are you sure to delete this block?", "LiteQnarie Editor", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                DeleteBlock(nowSelectedIndex);

                //Select a new block.
                if (nowSelectedIndex == mainStackPanel.Children.Count-1 && mainStackPanel.Children.Count>=2)
                { Block_Select(nowSelectedIndex - 1); }
                else if(nowSelectedIndex < mainStackPanel.Children.Count-1 && mainStackPanel.Children.Count>=2)
                { Block_Select(nowSelectedIndex); }
                else
                {
                    NothingSelected();
                }
            }
        }

        private void BlockDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure to delete this block?", "LiteQnarie Editor", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                DeleteBlock(nowSelectedIndex);

                if (nowSelectedIndex == mainStackPanel.Children.Count - 1 && mainStackPanel.Children.Count >= 2)
                { Block_Select(nowSelectedIndex - 1); }
                else if (nowSelectedIndex < mainStackPanel.Children.Count - 1 && mainStackPanel.Children.Count >= 2)
                { Block_Select(nowSelectedIndex); }
                else
                {
                    NothingSelected();
                }
            }
        }

        //While "DeleteDate" = false,only remove the StackPanel at that place.
        private void DeleteBlock(int index,bool DeleteData=true)
        {
            (mainStackPanel.Children[index] as Border).UpdateLayout();
            totalHeight -= ((mainStackPanel.Children[index] as Border).ActualHeight+10);

            UpdateMainStackHeight();

            mainStackPanel.Children.RemoveAt(index);
            if (DeleteData == true) { hXML.blocks.RemoveAt(index); }
        }

        private void ExchangeItem(int nowIndex,int targetIndex)
        {
            var temp = ((block)hXML.blocks[nowSelectedIndex]).Contents[nowIndex];
            ((block)hXML.blocks[nowSelectedIndex]).Contents[nowIndex] = ((block)hXML.blocks[nowSelectedIndex]).Contents[targetIndex];
            ((block)hXML.blocks[nowSelectedIndex]).Contents[targetIndex] = temp;

            //Refresh the block.
            DeleteBlock(nowSelectedIndex,false);
            LoadBlock((block)hXML.blocks[nowSelectedIndex], nowSelectedIndex);        
            Block_Select(nowSelectedIndex);
        }

        private void itemupButtonClick(object sender, MouseButtonEventArgs e)
        {
           if (nowSelectedIndex != -1 && ItemList.SelectedIndex > 0)
            {
                int tempSelectedIndex=ItemList.SelectedIndex;
                ExchangeItem(ItemList.SelectedIndex, ItemList.SelectedIndex - 1);
                ItemList.SelectedIndex = tempSelectedIndex - 1;             
            }
        }

        private void itemupButtonClick(object sender, RoutedEventArgs e)
        {
            if (nowSelectedIndex != -1 && ItemList.SelectedIndex > 0)
            {
                int tempSelectedIndex = ItemList.SelectedIndex;
                ExchangeItem(ItemList.SelectedIndex, ItemList.SelectedIndex - 1);
                ItemList.SelectedIndex = tempSelectedIndex - 1;
            }
        }

        private void itemdownButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (nowSelectedIndex != -1 && ItemList.SelectedIndex != ItemList.Items.Count-1)
            {
                int tempSelectedIndex = ItemList.SelectedIndex;
                ExchangeItem(ItemList.SelectedIndex, ItemList.SelectedIndex + 1);
                ItemList.SelectedIndex = tempSelectedIndex + 1; 
            }
        }

        private void itemdownButtonClick(object sender, RoutedEventArgs e)
        {
            if (nowSelectedIndex != -1 && ItemList.SelectedIndex != ItemList.Items.Count - 1)
            {
                int tempSelectedIndex = ItemList.SelectedIndex;
                ExchangeItem(ItemList.SelectedIndex, ItemList.SelectedIndex + 1);
                ItemList.SelectedIndex = tempSelectedIndex + 1;
            }
        }

        //Remove an item by index.
        private void removeItem(int index)
        {
            ((block)hXML.blocks[nowSelectedIndex]).Contents.RemoveAt(index);
            DeleteBlock(nowSelectedIndex,false);
            LoadBlock((block)hXML.blocks[nowSelectedIndex], nowSelectedIndex);
            LoadItemList(nowSelectedIndex);
        }

        private void itemdeleteButtonClick(object sender, MouseButtonEventArgs e)
        {
            deleteItem();          
        }

        private void itemdeleteButtonClick(object sender, RoutedEventArgs e)
        {
            deleteItem();
        }

        private void deleteItem(bool question=true)
        {
            if (ItemList.SelectedIndex != -1)
            {
                if (question == true)//Whether to show a message.
                {
                    if (MessageBox.Show("Are you sure to delete this item?", "LiteQnaire Editor", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
                    { return; }
                }

                int tempSelectedIndex = ItemList.SelectedIndex;

                removeItem(tempSelectedIndex);

                if (ItemList.Items.Count != 0)
                {
                    //New item will be selected in "ItemList".
                    if (tempSelectedIndex == ItemList.Items.Count)
                    { Block_Select(nowSelectedIndex); ItemList.SelectedIndex = tempSelectedIndex - 1; }
                    else if (tempSelectedIndex < ItemList.Items.Count)
                    { Block_Select(nowSelectedIndex); ItemList.SelectedIndex = tempSelectedIndex; }
                }
                else//When a block no longer has an item,remove it.
                {
                    DeleteBlock(nowSelectedIndex);
                    if (mainStackPanel.Children.Count >= 2) { Block_Select(0); }
                    else { NothingSelected(); }
                    scroll.ScrollToTop();
                }
            }
        }

        private void additemButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (nowSelectedIndex != -1)
            {
                addItem(ItemCombo.SelectedIndex);
            }
        }
        
        //Same as the index in "ItemCombo".
        private void addItem(int itemTypeIndex)
        {
            switch (itemTypeIndex)
            {
                case -1:
                    return;
                case 0:
                    labelText tempLabel = new labelText(true);
                    tempLabel.text = "New Text";
                    ((block)hXML.blocks[nowSelectedIndex]).Contents.Add(tempLabel);
                    break;
                case 1:
                    radioButton tempRadio= new radioButton(true);
                    tempRadio.text = "New RadioButton";
                    ((block)hXML.blocks[nowSelectedIndex]).Contents.Add(tempRadio);
                    break;
                case 2:
                    checkBox tempCheck = new checkBox(true);
                    tempCheck.text = "New CheckBox";
                    ((block)hXML.blocks[nowSelectedIndex]).Contents.Add(tempCheck);
                    break;
                case 3:
                    comboBox tempCombo = new comboBox(true);
                    tempCombo.texts.Add("Item 1");
                    ((block)hXML.blocks[nowSelectedIndex]).Contents.Add(tempCombo);
                    break;
                case 4:
                    attachment tempAtt = new attachment(true);
                    tempAtt.text="New Attachment";
                    tempAtt.filter.Add("*.*");
                    ((block)hXML.blocks[nowSelectedIndex]).Contents.Add(tempAtt);
                    break;
                case 5:
                    textBox tempText = new textBox(true);
                    ((block)hXML.blocks[nowSelectedIndex]).Contents.Add(tempText);
                    break;
            }

            DeleteBlock(nowSelectedIndex,false);
            LoadBlock((block)hXML.blocks[nowSelectedIndex], nowSelectedIndex);
            Block_Select(nowSelectedIndex);

            ItemList.SelectedIndex = ItemList.Items.Count - 1;
        }

        //Show the "EditWindow".
        private void itemEditButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (ItemList.SelectedIndex != -1)
            {
                LiteQnaireEditor.EditWindow.EditType = ((ItemList.SelectedItem as WrapPanel).Children[1] as Label).Content.ToString();
                LiteQnaireEditor.EditWindow.blockIndex = nowSelectedIndex;
                LiteQnaireEditor.EditWindow.itemIndex = ItemList.SelectedIndex;

                EditWindow newEditWindow = new EditWindow();
                newEditWindow.Owner = this;
                newEditWindow.ShowDialog();
            }          
        }

        private void itemEditButtonClick(object sender, RoutedEventArgs e)
        {
            if (ItemList.SelectedIndex != -1)
            {
                LiteQnaireEditor.EditWindow.EditType = ((ItemList.SelectedItem as WrapPanel).Children[1] as Label).Content.ToString();
                LiteQnaireEditor.EditWindow.blockIndex = nowSelectedIndex;
                LiteQnaireEditor.EditWindow.itemIndex = ItemList.SelectedIndex;

                EditWindow newEditWindow = new EditWindow();
                newEditWindow.Owner = this;
                newEditWindow.ShowDialog();
            }
        }

        //Let the "EditWindow" refreshs the blocks in "MainWindow".
        public void RefreshOwner(int blockIndex,int itemIndex)
        {
            DeleteBlock(blockIndex,false);
            LoadBlock((block)hXML.blocks[blockIndex], blockIndex);
            LoadItemList(blockIndex);
            Block_Select(blockIndex);
            ItemList.SelectedIndex = itemIndex;
        }
        
        private void BlockEditButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (nowSelectedIndex != -1)
            {
                LiteQnaireEditor.EditWindow.EditType = "Block";
                LiteQnaireEditor.EditWindow.blockIndex = nowSelectedIndex;
                LiteQnaireEditor.EditWindow.itemIndex = -1;

                EditWindow newEditWindow = new EditWindow();
                newEditWindow.Owner = this;
                newEditWindow.ShowDialog();
            }
        }

        private void BlockEditButtonClick(object sender, RoutedEventArgs e)
        {
            if (nowSelectedIndex != -1)
            {
                LiteQnaireEditor.EditWindow.EditType = "Block";
                LiteQnaireEditor.EditWindow.blockIndex = nowSelectedIndex;
                LiteQnaireEditor.EditWindow.itemIndex = -1;

                EditWindow newEditWindow = new EditWindow();
                newEditWindow.Owner = this;
                newEditWindow.ShowDialog();
            }
        }

        private delegate void UpdatePBDelegate(System.Windows.DependencyProperty dp, object value);
        private void PBSetValue(double value)
        {
            UpdatePBDelegate updatePBDelegate = new UpdatePBDelegate(savePBar.SetValue);
            Dispatcher.Invoke(updatePBDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { ProgressBar.ValueProperty, value });
        }

        private void saveButton_Click(object sender, MouseButtonEventArgs e)
        {
            Save();
        }

        private bool Save(bool SaveAsMode=false)
        {
            if (isChanged == true || SaveAsMode == true)
            {
                string tempFilePath = filePath;
                if (filePath == CacheFolder + @"\Unnamed.lqn" || SaveAsMode==true)//Path hasn't been settled or save as.
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "LQN File|*.lqn";
                    if (sfd.ShowDialog() == true)
                    {
                        filePath = sfd.FileName;
                    }
                    else { return false;/*Canceled*/ }
                }

                savePBar.Visibility = Visibility.Visible;
                savePBar.Maximum = 1;
                PBSetValue(0.1);

                hXML.Save();//Save the xml in "XMLFilepath".
                PBSetValue(0.4);

                FileGlue fg = new FileGlue(filePath);//Replace the lqn file.
                PBSetValue(0.6);

                int attCount = 0;
                foreach (block block in hXML.blocks)
                {
                    foreach (var item in block.Contents) { if (item is attachment) { attCount++; } }
                }
                fg.Glue(new string[2] { XMLFilepath, XMLFilepath }, 2 + attCount/* ←↑ Calculate the "maxFileCount".*/);
                PBSetValue(0.8);

                if (SaveAsMode == true) { filePath = tempFilePath; }
                isChanged = false;
                PBSetValue(1);
                savePBar.Visibility = Visibility.Hidden;
            }
            return true;//Successfully saved.
        }

        private void saveasButton_Click(object sender, MouseButtonEventArgs e)
        {
            Save(true);
        }

        private bool? SaveMessage()
        {
            if (isChanged == true)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save the changes of the editing file?", "LiteQnaire Editor", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if(Save()==false){return null;/*Canceled*/}
                    return true;
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return null;/*Canceled*/
                }
            }
            return false;//Do not save.
        }

        private void openButton_Click(object sender, MouseButtonEventArgs e)
        {
            OpenFile();
        }

        private void OpenFile()
        {
            if (SaveMessage() == null) { return; }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "LQN File|*.lqn";
            if (ofd.ShowDialog() == true)
            {
                mainStackPanel.Children.Clear();
                CleanCache();

                loadFile(ofd.FileName);
                LoadContents();

                NothingSelected();
                isChanged = false;
                saveButton.ToolTip = "Save " + ConvertTo.FileName(filePath) + " (Ctrl+S)";
            }
        }

        private void CopyItem_Click(object sender, RoutedEventArgs e)
        {
            Copy_Item(((block)hXML.blocks[nowSelectedIndex]).Contents[ItemList.SelectedIndex]);
        }

        private void CopyBlock_Click(object sender, RoutedEventArgs e)
        {
            Copy_Block(((block)hXML.blocks[nowSelectedIndex]));
        }

        //Copy an item to the clipboard.
        private void Copy_Item(object item)
        {
            HeadXMLFile tempHXML = new HeadXMLFile();
            string itemXML = "<LQN_ITEM>" + "\n" + "<Block BackgroundColor=\"black\" SplitColor=\"black\">" + "\n";//A temp block node.

            foreach (string line in tempHXML.GetItemXML(item))
            {
                itemXML += (line) + "\n";
            }

            itemXML += "</Block>" + "\n" + "</LQN_ITEM>";

            Clipboard.SetDataObject(itemXML);
        }

        private void Copy_Block(block GetBlock)
        {
            HeadXMLFile tempHXML = new HeadXMLFile();
            string blockXML = "<LQN_BLOCK>" + "\n" + "<Block BackgroundColor=\"" + GetBlock.BackgroundColor + "\" SplitColor=\"" + GetBlock.SplitColor + "\">\n";

            foreach (object item in GetBlock.Contents)
            {
                foreach (string line in tempHXML.GetItemXML(item))
                {
                    blockXML += (line) + "\n";
                }
            }

            blockXML += "</Block>" + "\n" + "</LQN_BLOCK>";

            Clipboard.SetDataObject(blockXML);
        }

        private void CutItem_Click(object sender, RoutedEventArgs e)
        {
            Copy_Item(((block)hXML.blocks[nowSelectedIndex]).Contents[ItemList.SelectedIndex]);
            deleteItem(false);
        }

        private void PasteItem_Click(object sender, RoutedEventArgs e)
        {
            Paste_Item();
        }

        private void Paste_Item()
        {
            if (nowSelectedIndex != -1)
            {
                try
                {
                    HeadXMLFile tempHXML = new HeadXMLFile();

                    StringReader sReader = new StringReader(Clipboard.GetText());

                    tempHXML.ReadStream(sReader, true);

                    if (((block)tempHXML.blocks[0]).Contents.Count == 1)
                    {
                        ((block)hXML.blocks[nowSelectedIndex]).Contents.Add(((block)tempHXML.blocks[0]).Contents[0]);
                    }

                    DeleteBlock(nowSelectedIndex, false);
                    LoadBlock((block)hXML.blocks[nowSelectedIndex], nowSelectedIndex);
                    Block_Select(nowSelectedIndex);
                }
                catch
                {

                }

            }
        }

        private void PasteBlock_Click(object sender, RoutedEventArgs e)
        {
            Paste_Block();
        }

        private void Paste_Block()
        {
            if (nowSelectedIndex != -1)
            {
                try
                {
                    HeadXMLFile tempHXML = new HeadXMLFile();

                    StringReader sReader = new StringReader(Clipboard.GetText());

                    tempHXML.ReadStream(sReader, true);

                    hXML.blocks.Add((block)tempHXML.blocks[0]);

                    AddButtonUnload();
                    LoadBlock((block)tempHXML.blocks[0], -1);
                    Block_Select(mainStackPanel.Children.Count - 1);
                    AddButtonLoad();
                }
                catch
                {

                }
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
            else if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Keyboard.IsKeyDown(Key.N))
            {
                NewFile_Click();
            }
            else if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Keyboard.IsKeyDown(Key.C))
            {
                if (ItemList.SelectedIndex != -1 && nowSelectedIndex != -1)
                {
                    Copy_Item(((block)hXML.blocks[nowSelectedIndex]).Contents[ItemList.SelectedIndex]);
                }
            }
            else if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Keyboard.IsKeyDown(Key.X))
            {
                if (ItemList.SelectedIndex != -1 && nowSelectedIndex != -1)
                {
                    Copy_Item(((block)hXML.blocks[nowSelectedIndex]).Contents[ItemList.SelectedIndex]);
                    deleteItem(false);
                }
            }
            else if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Keyboard.IsKeyDown(Key.V))
            {
                Paste_Item();
            }
        }

        private void File_Drop(object sender, DragEventArgs e)
        {
            string filename = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();

            if (ConvertTo.FileType(filename).ToLower() == ".lqn")
            {
                if (SaveMessage() == null) { return; }
                mainStackPanel.Children.Clear();
                CleanCache();

                loadFile(filename);
                LoadContents();

                NothingSelected();
                isChanged = false;
                saveButton.ToolTip = "Save " + ConvertTo.FileName(filePath) + " (Ctrl+S)";
            }
        }

        private void newfileButtonMouseMove(object sender, MouseEventArgs e)
        {
            newfileButton.Source = newfilemoveImg;
        }

        private void newfileButtonMouseLeave(object sender, MouseEventArgs e)
        {
            newfileButton.Source = newfileImg;
        }

        private void additemButtonMouseMove(object sender, MouseEventArgs e)
        {
            additemButton.Source = additemmoveImg;
        }

        private void additemButtonMouseLeave(object sender, MouseEventArgs e)
        {
            additemButton.Source = additemImg;
        }

        private void saveButtonMouseMove(object sender, MouseEventArgs e)
        {
            saveButton.Source = savemoveImg;
        }

        private void saveButtonMouseLeave(object sender, MouseEventArgs e)
        {
            saveButton.Source = saveImg;
        }

        private void saveasButtonMouseMove(object sender, MouseEventArgs e)
        {
            saveasButton.Source = saveasmoveImg;
        }

        private void saveasButtonMouseLeave(object sender, MouseEventArgs e)
        {
            saveasButton.Source = saveasImg;
        }

        private void openButtonMouseMove(object sender, MouseEventArgs e)
        {

            openButton.Source = openmoveImg;
        }

        private void openButtonMouseLeave(object sender, MouseEventArgs e)
        {
            openButton.Source = openImg;
        }

        private void editButtonMouseMove(object sender, MouseEventArgs e)
        {
            (sender as Image).Source = editmoveImg;
        }

        private void editButtonMouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Image).Source = editImg;
        }
        private void upButtonMouseMove(object sender, MouseEventArgs e)
        {
            (sender as Image).Source = upmoveImg;
        }

        private void upButtonMouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Image).Source = upImg;
        }

        private void deleteButtonMouseMove(object sender, MouseEventArgs e)
        {
            (sender as Image).Source = deletemoveImg;
        }

        private void deleteButtonMouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Image).Source = deleteImg;
        }
        private void downButtonMouseMove(object sender, MouseEventArgs e)
        {
            (sender as Image).Source = downmoveImg;
        }

        private void downButtonMouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Image).Source = downImg;
        }

        private void blockPanel_MouseMove(object sender, MouseEventArgs e)
        {
            (sender as Border).BorderBrush = new SolidColorBrush(Colors.LightBlue);
        }

        private void blockPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            if (nowSelectedIndex != mainStackPanel.Children.IndexOf(sender as Border)) { (sender as Border).BorderBrush = null; }
        }

        private void blockPanel_Click(object sender, MouseButtonEventArgs e)
        {
            Block_Select(mainStackPanel.Children.IndexOf(sender as Border));
        }

        private void WindowsClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool? result = SaveMessage();
            if (result == null)
            {
                e.Cancel = true;
                return;
            }

            CleanCache();
        }

        private void CleanCache()
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
