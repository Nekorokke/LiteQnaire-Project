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
using System.Threading;
using System.Collections;
using System.Xml;
using Winform=System.Windows.Forms;
using Microsoft.Win32;
using HeadXMLLib;
using LQNCollectorLib;
using ExtractFileLib;

namespace LiteQnaire_Collector
{
    public enum NamingMethod
    {
        Enumerate=1,
        Origin,
        Att,
        Index
    }

    public struct NamingInfo
    {
        public NamingMethod namingMethod;
        public string indexString;

        public NamingInfo(NamingMethod namingMethod)
        {
            this.namingMethod=namingMethod;
            indexString="";
        }

        public NamingInfo(string indexString)
        {
            this.indexString=indexString;
            this.namingMethod=NamingMethod.Index;
        }
    }
    /// <summary>
    /// MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
        static string CacheFolder = "";
        string WorkFolder="";

        public HeadXMLFile templateHXML;

        static WorkbookOperator wbOperator = new WorkbookOperator();
        static SummaryManagement smryMng = new SummaryManagement();
        
        BitmapImage resetImg;
        BitmapImage checkImg;

        public MainWindow()
        {
            InitializeComponent();
            CacheFolder = CurrentDirectory + @"\Cache\Cache" + new Random().Next(9999999).ToString();
            Directory.CreateDirectory(CacheFolder);

            resetImg = new BitmapImage(new Uri(CurrentDirectory+@"\Image\reset.png", UriKind.Absolute));
            resetImage.Source=resetImg;

            checkImg = new BitmapImage(new Uri(CurrentDirectory + @"\Image\check.png", UriKind.Absolute));
            checkImage.Source = checkImg;
        }

        private delegate void UpdatePBDelegate(System.Windows.DependencyProperty dp, object value);
        private void PBSetValue(double value)
        {
            UpdatePBDelegate updatePBDelegate = new UpdatePBDelegate(progressBar.SetValue);
            Dispatcher.Invoke(updatePBDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { ProgressBar.ValueProperty, value });
        }

        private void SelectTemplate_Click(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "LQN File|*.lqn";

            if (ofd.ShowDialog() == true)
            {
                listBox.Items.Clear();
                try
                {
                    ExtractFile eFile = new ExtractFile(ofd.FileName);
                    eFile.ExtractTo(CacheFolder + "\\template.xml", 1);
                    templateHXML = new HeadXMLFile(CacheFolder + "\\template.xml");
                    templateHXML.Read();
                    loadBlockCombo();
                }
                catch
                {
                    errorMessage("Failed to open :\r\n" + ofd.FileName);
                    return;
                }

                (sender as TextBox).Text = ofd.FileName;

                workfolderTextbox.IsEnabled = true;
            }
        }

        private void SelectWorkbook_Click(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel File|*.xls;*.xlsx";

            if (ofd.ShowDialog() == true)
            {
                progressBar.Visibility = Visibility.Visible;
                progressBar.Value = 0;

                PBSetValue(15);
                try
                {                 
                    wbOperator.targetWorkbookPath = ofd.FileName;
                }
                catch (IOException err)
                {
                    errorMessage(err.Message);
                    progressBar.Visibility = Visibility.Hidden;
                    return;
                }
                PBSetValue(85);

                (sender as TextBox).Text = ofd.FileName;
                PBSetValue(100);
                progressBar.Visibility = Visibility.Hidden;
            }           
        }

        private void WorkFolder_Click(object sender, MouseButtonEventArgs e)
        {
            Winform.FolderBrowserDialog fbd = new Winform.FolderBrowserDialog();
            if (fbd.ShowDialog() == Winform.DialogResult.OK)
            {
                (sender as TextBox).Text = fbd.SelectedPath;
                WorkFolder = fbd.SelectedPath;
            }
        }

        private void errorMessage(string message)
        {
            MessageBox.Show(message, "LiteQnaire Collector", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void tipMessage(string message) 
        {
            MessageBox.Show(message, "LiteQnaire Collector", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void loadBlockCombo()
        {
            blockCombo.Items.Clear();
            int i = 1;
            foreach(block blk in templateHXML.blocks)
            {
                blockCombo.Items.Add("Block (" + i.ToString() + ")");               
                i++;
            }
            blockCombo.SelectedIndex = 0;
        }

        private void loadItemCombo(int index)
        {
            if (index != -1)
            {
                itemCombo.Items.Clear();

                int j = 1;
                foreach (var item in (((block)templateHXML.blocks[index])).Contents)
                {
                    string indexString = "Index=["+(index + 1).ToString() + "-" + j.ToString() + "] , Item=";
                    if (item is textBox)
                    {
                        itemCombo.Items.Add(indexString + "TextBox - \"" + ((textBox)item).text + "\"");
                    }
                    else if (item is labelText)
                    {
                        itemCombo.Items.Add(indexString + "Text - \"" + ((labelText)item).text + "\"");
                    }
                    else if (item is radioButton)
                    {
                        itemCombo.Items.Add(indexString + "RadioButton - \"" + ((radioButton)item).text + "\"");
                    }
                    else if (item is checkBox)
                    {
                        itemCombo.Items.Add(indexString + "CheckBox - \"" + ((checkBox)item).text + "\"");
                    }
                    else if (item is attachment)
                    {

                        itemCombo.Items.Add(indexString + "Attachment - \"" + ((attachment)item).text + "\"");
                    }
                    else if (item is comboBox)
                    {
                        itemCombo.Items.Add(indexString + "ComboBox- \"" + ((comboBox)item).texts[0].ToString() + "\"...");
                    }

                    j++;
                }

                itemCombo.SelectedIndex = 0;
            }          
         }

        private void showCorrectButton()
        {
            if ((((block)templateHXML.blocks[blockCombo.SelectedIndex]).Contents[itemCombo.SelectedIndex]) is attachment)
            {
                extractButton.Visibility = Visibility.Visible;
                outputButton.Visibility = Visibility.Hidden;
                matchedOutputButton.Visibility = Visibility.Hidden;
            }
            else
            {
                extractButton.Visibility = Visibility.Hidden;
                outputButton.Visibility = Visibility.Visible;
                matchedOutputButton.Visibility = Visibility.Visible;
            }
        }

        private void StartCollecting_Click(object sender, MouseButtonEventArgs e)
        {
            ((sender as Label).Parent as Border).Opacity = 0.75;
            if (workfolderTextbox.Text != "" && WorkFolder != "")
            {
                progressBar.Visibility = Visibility.Visible;
                progressBar.Value = 0;

                PBSetValue(5);

                Thread thread = new Thread(() =>
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        listBox.Items.Clear();

                        string[] files = Directory.GetFiles(WorkFolder, "*.lqn");

                        ArrayList fileArray = new ArrayList();

                        HeadXMLFile tempHXML;

                        PBSetValue(35);

                        foreach (string path in files)
                        {
                            try
                            {
                                ExtractFile eFile = new ExtractFile(path);
                                eFile.ExtractTo(CacheFolder + "\\temp.xml", 1);//Extract the editable xml.

                                tempHXML = new HeadXMLFile(CacheFolder + "\\temp.xml");
                                tempHXML.Read();

                                if (templateHXML.CompareTo(ref tempHXML) == true)//Check if they are in the same template.
                                {
                                    //Ensure the editable xml is able to be loaded.
                                    eFile.ExtractTo(CacheFolder + "\\temp.xml", 0);
                                    tempHXML = new HeadXMLFile(CacheFolder + "\\temp.xml");
                                    tempHXML.Read();

                                    fileArray.Add(path);
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        PBSetValue(75);

                        smryMng.TemplateHXML = templateHXML;
                        smryMng.SummarizeFromFiles(fileArray);

                        addToList(fileArray);
                        loadItemContents(smryMng.GetInnerTexts(blockCombo.SelectedIndex + 1, itemCombo.SelectedIndex + 1));
                        PBSetValue(100);

                        tipMessage(smryMng.Files.Count.ToString() + " Files found.");
                        showCorrectButton();
                        progressBar.Visibility = Visibility.Hidden;
                    }));
                });
                thread.Start();
            }
        }
            
        //Add paths to "ListBox" in the window.
        private void addToList(ArrayList paths)
        {
            foreach (string path in paths)
            {
                WrapPanel tempWrap = new WrapPanel();
                CheckBox tempCheck = new CheckBox();
                tempCheck.IsChecked = true;
                tempCheck.Content = path;

                TextBlock tempText = new TextBlock();
                tempText.Margin = new Thickness(8, 0, 0, 0);
                tempText.Foreground=new SolidColorBrush(Colors.DimGray);

                tempWrap.Children.Add(tempCheck);
                tempWrap.Children.Add(tempText);
                
                tempWrap.Margin = new Thickness(3, 2.5, 0, 0);

                listBox.Items.Add(tempWrap);
            }
        }

        //Load inner texts of items in summary xml.
        private void loadItemContents(ArrayList contents)
        {
            for (int i = 0; i < listBox.Items.Count; i++)
            {
                ((listBox.Items[i] as WrapPanel).Children[1] as TextBlock).Text = "Content= \""+contents[i].ToString()+"\"";
            }
        }

        private void blockCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            loadItemCombo(blockCombo.SelectedIndex);
        }

        private void itemCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (workfolderTextbox.Text != "" && itemCombo.SelectedIndex!=-1)
            {
                showCorrectButton();
                Reset();
                loadItemContents(smryMng.GetInnerTexts(blockCombo.SelectedIndex + 1, itemCombo.SelectedIndex + 1));
            }
        }

        private void outputButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (listBox.Items.Count>0 && workbookTextbox.Text!="")
            {
                OutputWindow outputWindow = new OutputWindow();
                outputWindow.Owner = this;
                outputWindow.blockIndex = blockCombo.SelectedIndex + 1;
                outputWindow.itemIndex = itemCombo.SelectedIndex + 1;
                outputWindow.sheetsArray = wbOperator.GetSheets();
                outputWindow.ShowDialog();
            }
        }

        private void matchedOutputButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (listBox.Items.Count > 0 && workbookTextbox.Text != "")
            {
                MatchedOutput MatchedOutput = new MatchedOutput();
                MatchedOutput.Owner = this;
                MatchedOutput.blockIndex = blockCombo.SelectedIndex + 1;
                MatchedOutput.itemIndex = itemCombo.SelectedIndex + 1;
                MatchedOutput.sheetsArray = wbOperator.GetSheets();
                MatchedOutput.ShowDialog();
            }
        }

        private void extractButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (listBox.Items.Count > 0)
            {
                BatchExtract batchExtract = new BatchExtract();
                batchExtract.Owner = this;
                batchExtract.blockIndex = blockCombo.SelectedIndex + 1;
                batchExtract.itemIndex = itemCombo.SelectedIndex + 1;
                batchExtract.ShowDialog();
            }
        }

        //Reset the "listBox".
        private void Reset()
        {
            foreach (WrapPanel tempWrap in listBox.Items)
            {
                (tempWrap.Children[0] as CheckBox).Foreground = new SolidColorBrush(Colors.Black);
                (tempWrap.Children[0] as CheckBox).ToolTip = null;
                (tempWrap.Children[0] as CheckBox).Tag = null;
            }
        }

        private void Reset_Click(object sender, MouseButtonEventArgs e)
        {
            Reset();
        }

        //Consult the comment at "LQNCollectorLib.SummaryManagement.repetitionCheck()".
        private void RepititionCheck_Click(object sender, MouseButtonEventArgs e)
        {
            (sender as Label).Opacity = 0.75;

            if (listBox.Items.Count>0)
            {
                int i = 0;
                foreach (ArrayList rInfo in smryMng.checkRepetition(blockCombo.SelectedIndex + 1, itemCombo.SelectedIndex + 1))
                {
                    int rCount = (rInfo[1] as string[]).Count();
                    foreach (string path in rInfo[1] as string[])
                    {
                        foreach (WrapPanel tempWrap in listBox.Items)
                        {
                            if ((tempWrap.Children[0] as CheckBox).Content.ToString() == path)
                            {
                                i++;
                                (tempWrap.Children[0] as CheckBox).Tag = "nonstandard";
                                (tempWrap.Children[0] as CheckBox).ToolTip = rCount.ToString() + " Repitition.";
                                (tempWrap.Children[0] as CheckBox).Foreground = new SolidColorBrush(Colors.IndianRed);
                            }
                        }
                    }
                }

                tipMessage(i.ToString() + " Items found.");
            }
        }

        private void BlankCheck_Click(object sender, MouseButtonEventArgs e)
        {
            (sender as Label).Opacity = 0.75;

            if (listBox.Items.Count>0)
            {
                int i = 0;
                foreach (string path in smryMng.checkBlank(blockCombo.SelectedIndex + 1, itemCombo.SelectedIndex + 1))
                {
                    foreach (WrapPanel tempWrap in listBox.Items)
                    {
                        if ((tempWrap.Children[0] as CheckBox).Content.ToString() == path)
                        {
                            i++;
                            (tempWrap.Children[0] as CheckBox).Tag = "nonstandard";
                            (tempWrap.Children[0] as CheckBox).Foreground = new SolidColorBrush(Colors.Goldenrod);
                        }
                    }

                }

                tipMessage(i.ToString() + " Items found.");
            }   
        }

        private void Inverse_Click(object sender, MouseButtonEventArgs e)
        {
            (sender as Label).Opacity = 0.75;
            foreach (WrapPanel tempWrap in listBox.Items)
            {
                (tempWrap.Children[0] as CheckBox).IsChecked = !(tempWrap.Children[0] as CheckBox).IsChecked;
            }
        }

        private void Checkall_Click(object sender, MouseButtonEventArgs e)
        {
            (sender as Label).Opacity = 0.75;
            foreach (WrapPanel tempWrap in listBox.Items)
            {
                (tempWrap.Children[0] as CheckBox).IsChecked = true;
            }
        }

        private void Invert_Click(object sender, MouseButtonEventArgs e)
        {
            (sender as Label).Opacity = 0.75;
            foreach (WrapPanel tempWrap in listBox.Items)
            {
                if ((tempWrap.Children[0] as CheckBox).Tag!=null)
                {
                    (tempWrap.Children[0] as CheckBox).IsChecked = !(tempWrap.Children[0] as CheckBox).IsChecked;
                }
            }
        }

        //Remove the unchecked item in summary xml.
        private XmlDocument GetRightSummaryXml()
        {
            XmlDocument smryXml = smryMng.SummaryXml;
            XmlNodeList summaryList = smryXml.SelectSingleNode("Summaries").ChildNodes;

            //Create a list of paths to be removed.
            ArrayList removeList = new ArrayList();
            foreach (WrapPanel tempWrap in listBox.Items)
            {
                if ((tempWrap.Children[0] as CheckBox).IsChecked == false)
                {
                    removeList.Add((tempWrap.Children[0] as CheckBox).Content.ToString());
                }
            }

            if (removeList.Count > 0)
            {
                for (int i = 0; i < removeList.Count; i++)
                {
                    foreach (XmlNode node in summaryList)
                    {
                        if (removeList.IndexOf((node as XmlElement).GetAttribute("path")) >= 0)
                        {
                            node.ParentNode.RemoveChild(node);
                        }
                    }
                }
            }

            return smryXml;
        }

        //Invoked by "OutputWindow".
        public void Output(outputSheetInfo opsInfo,fillerInfo fInfo)
        {
            progressBar.Visibility = Visibility.Visible;
            progressBar.Value = 0;

            PBSetValue(10);

            XmlDocument smryXml = GetRightSummaryXml();

            PBSetValue(30);
                
            wbOperator.Collect_Text_IntoWorkbook(ref smryXml, opsInfo, fInfo);
            PBSetValue(70);

            wbOperator.SaveWorkbook();
            PBSetValue(100);

            progressBar.Visibility = Visibility.Hidden;
        }

        //Invoked by "MatchedOutput".
        public void Output(matchingSheetInfo msInfo, fillerInfo fInfo)
        {
            progressBar.Visibility = Visibility.Visible;
            progressBar.Value = 0;

            PBSetValue(10);

            XmlDocument smryXml = GetRightSummaryXml();

            PBSetValue(30);

            wbOperator.Collect_Text_IntoWorkbook(ref smryXml, msInfo, fInfo);
            PBSetValue(70);

            wbOperator.SaveWorkbook();
            PBSetValue(100);

            progressBar.Visibility = Visibility.Hidden;
        }

        //Invoked by "BatchExtract". 
        public void BatchExtract(string indexString,string folderPath,NamingInfo namingInfo)
        {
            progressBar.Visibility = Visibility.Visible;
            progressBar.Value = 0;
            PBSetValue(5);

            XmlDocument smryXml = GetRightSummaryXml();
            XmlNodeList summaryList=smryXml.GetElementsByTagName("Summary");
            PBSetValue(20);

            //Create file name ArrayList by users selected naming method.
            ArrayList fileNames = new ArrayList();
            switch (namingInfo.namingMethod)
            {
                case NamingMethod.Enumerate:
                    for (int i = 1; i <= summaryList.Count; i++)
                    {
                        fileNames.Add(i.ToString());
                    }
                        break;
                case NamingMethod.Origin:
                     for (int i = 0; i < summaryList.Count; i++)
                     {
                        fileNames.Add(System.IO.Path.GetFileNameWithoutExtension(((XmlElement)summaryList[i]).GetAttribute("path")));
                     }
                        break;
                case NamingMethod.Att:
                     foreach (WrapPanel tempWrap in listBox.Items)
                     {
                         string contentString = (tempWrap.Children[1] as TextBlock).Text;
                         
                         fileNames.Add(System.IO.Path.GetFileNameWithoutExtension(contentString.Substring("Content= \"".Count(),contentString.Count()-"Content= \"".Count()).TrimEnd('\"')));
                     }
                        break;
                case NamingMethod.Index:
                     fileNames.Add(new string[1]{namingInfo.indexString});
                        break;                 
            }
            PBSetValue(40);

            extractInfo eInfo=new extractInfo(ref smryXml,indexString,fileNames,folderPath);
            AttachmentExtractor.Extract(eInfo);

            PBSetValue(100);

            progressBar.Visibility = Visibility.Hidden;
        }

        private void LabelMouseMove(object sender, MouseEventArgs e)
        {
            (sender as Label).Foreground = new SolidColorBrush(Colors.Gray);
            (sender as Label).Opacity = 0.75;
        }

        private void LabelMouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Label).Foreground = new SolidColorBrush(Colors.DimGray);
            (sender as Label).Opacity = 1;
        }

        private void LabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            (sender as Label).Opacity = 0.5;
        }

        private void LabelMouseUp(object sender, MouseButtonEventArgs e)
        {
            (sender as Label).Opacity = 0.75;
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Directory.Delete(CacheFolder, true);
            }
            catch
            {

            }
        }

    }
}
