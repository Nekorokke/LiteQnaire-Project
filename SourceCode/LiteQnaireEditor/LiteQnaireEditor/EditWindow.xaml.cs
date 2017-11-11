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
using System.Reflection;
using LiteQnaireEditor;
using System.Collections;
using System.IO;

namespace LiteQnaireEditor
{
    /// <summary>
    /// EditWindow.xaml
    /// Load the option controls by a template xml file.
    /// </summary>
    public partial class EditWindow : Window
    {
        public static string EditType;//Such as 'Block','RadioButton','CheckBox'...

        block GetBlock;//"block" in the "MainWindow".

        public static int blockIndex;
        public static int itemIndex;

        HeadXMLFile EditTemplate;//The template xml to load the option controls.

        public EditWindow()
        {
            InitializeComponent();
            EditTemplate = new HeadXMLFile(LiteQnaireEditor.MainWindow.CurrentDirectory+@"\templates\"+EditType+"_Edit.xml");
            GetBlock = (block)MainWindow.hXML.blocks[blockIndex];
        }

        private void EditWindowLoaded(object sender, RoutedEventArgs e)
        {
            LoadContents();
            LoadAttributes();          
        }

        //Load the option controls.
        private void LoadContents()
        {
            EditTemplate.Read();

            int i = -1;
            int j = -1;

            double totalHeight = 0;
            foreach (block block in EditTemplate.blocks)
            {
                i += 1;
                StackPanel tempStack = new StackPanel();
                tempStack.Height = 0;
                mainStackPanel.Children.Add(tempStack);
                tempStack.Background = new SolidColorBrush(ConvertTo.Color(block.BackgroundColor));
                tempStack.Margin = new Thickness(4, 2, 4, 0);

                foreach (var item in block.Contents)
                {
                    if (item is labelText)
                    {
                        j += 1;
                        Label tempLabel = new Label();
                        labelText XMLLabel = ((labelText)item);

                        tempLabel.Content = XMLLabel.text;
                        tempLabel.FontSize = XMLLabel.fontsize;
                        tempLabel.Foreground = new SolidColorBrush(ConvertTo.Color(XMLLabel.forecolor));
                        if (XMLLabel.fontstyle.ToLower() == "bold") { tempLabel.FontWeight = FontWeights.Bold; }
                        tempLabel.FontStyle = ConvertTo.FontStyle(XMLLabel.fontstyle);

                        tempLabel.Tag = i.ToString() + "," + j.ToString();

                        tempStack.Children.Add(tempLabel);
                        switch (XMLLabel.position)
                        {
                            case "left":
                                tempLabel.Margin = new Thickness(20, 4, 0, 0);
                                break;
                            case "right":
                                tempLabel.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                                tempLabel.Margin = new Thickness(0, 4, 20, 0);
                                break;
                            case "middle":
                                tempLabel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                tempLabel.Margin = new Thickness(0, 4, 0, 0);
                                break;
                        }
                        tempLabel.UpdateLayout();
                        tempStack.Height += (tempLabel.ActualHeight + 4);
                    }
                    else if (item is radioButton)
                    {
                        j += 1;
                        RadioButton tempRadio = new RadioButton();
                        radioButton XMLRadio = ((radioButton)item);

                        tempRadio.Content = XMLRadio.text;
                        tempRadio.FontSize = XMLRadio.fontsize;
                        tempRadio.Foreground = new SolidColorBrush(ConvertTo.Color(XMLRadio.forecolor));
                        if (XMLRadio.fontstyle.ToLower() == "bold") { tempRadio.FontWeight = FontWeights.Bold; }
                        tempRadio.FontStyle = ConvertTo.FontStyle(XMLRadio.fontstyle);

                        tempRadio.Tag = i.ToString() + "," + j.ToString();

                        tempStack.Children.Add(tempRadio);
                        switch (XMLRadio.position)
                        {
                            case "left":
                                tempRadio.Margin = new Thickness(20, 4, 0, 0);
                                break;
                            case "right":
                                tempRadio.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                                tempRadio.Margin = new Thickness(0, 4, 20, 0);
                                break;
                            case "middle":
                                tempRadio.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                tempRadio.Margin = new Thickness(0, 4, 0, 0);
                                break;
                        }
                        tempRadio.UpdateLayout();
                        tempStack.Height += (tempRadio.ActualHeight + 4);
                        tempRadio.IsChecked = XMLRadio.selected;
                        tempRadio.GroupName = i.ToString();
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

                        tempCheck.Tag = i.ToString() + "," + j.ToString();

                        tempStack.Children.Add(tempCheck);
                        switch (XMLCheck.position)
                        {
                            case "left":
                                tempCheck.Margin = new Thickness(20, 4, 0, 0);
                                break;
                            case "right":
                                tempCheck.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                                tempCheck.Margin = new Thickness(0, 4, 20, 0);
                                break;
                            case "middle":
                                tempCheck.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                tempCheck.Margin = new Thickness(0, 4, 0, 0);
                                break;
                        }
                        tempCheck.UpdateLayout();
                        tempStack.Height += (tempCheck.ActualHeight + 4);
                        tempCheck.IsChecked = XMLCheck.selected;
                    }
                    else if (item is textBox)
                    {
                        j += 1;
                        TextBox tempText = new TextBox();
                        textBox XMLText = ((textBox)item);

                        tempText.Width = 300;

                        tempText.FontSize = XMLText.fontsize;
                        tempText.Foreground = new SolidColorBrush(ConvertTo.Color(XMLText.forecolor));
                        if (XMLText.fontstyle.ToLower() == "bold") { tempText.FontWeight = FontWeights.Bold; }
                        tempText.FontStyle = ConvertTo.FontStyle(XMLText.fontstyle);


                        tempText.Tag = i.ToString() + "," + j.ToString();

                        tempStack.Children.Add(tempText);


                        if (XMLText.isLong == true)
                        {
                            tempText.UpdateLayout();
                            tempText.Height = tempText.ActualHeight * 3.5;
                            tempText.TextWrapping = System.Windows.TextWrapping.WrapWithOverflow;
                            tempText.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;
                            tempText.AcceptsReturn = true;
                        }
                        else
                        {
                            tempText.TextWrapping = System.Windows.TextWrapping.NoWrap;
                        }
                        if (XMLText.isLocked == true) { tempText.IsReadOnly = true; }

                        tempText.Text = XMLText.text;

                        switch (XMLText.position)
                        {
                            case "left":
                                tempText.Margin = new Thickness(20, 4, 0, 0);
                                break;
                            case "right":
                                tempText.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                                tempText.Margin = new Thickness(0, 4, 20, 0);
                                break;
                            case "middle":
                                tempText.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                tempText.Margin = new Thickness(0, 4, 0, 0);
                                break;
                        }
                        tempText.UpdateLayout();
                        tempStack.Height += (tempText.ActualHeight + 4);
                    }
                    else if (item is comboBox)
                    {
                        j += 1;
                        ComboBox tempCombo = new ComboBox();
                        comboBox XMLCombo = ((comboBox)item);

                        tempCombo.ItemsSource = XMLCombo.texts;
                        tempCombo.FontSize = XMLCombo.fontsize;
                        tempCombo.Foreground = new SolidColorBrush(ConvertTo.Color(XMLCombo.forecolor));
                        if (XMLCombo.fontstyle.ToLower() == "bold") { tempCombo.FontWeight = FontWeights.Bold; }
                        tempCombo.FontStyle = ConvertTo.FontStyle(XMLCombo.fontstyle);

                        tempCombo.Tag = i.ToString() + "," + j.ToString();

                        tempStack.Children.Add(tempCombo);
                        tempCombo.Width = 200;
                        switch (XMLCombo.position)
                        {
                            case "left":
                                tempCombo.Margin = new Thickness(20, 4, 0, 0);
                                break;
                            case "right":
                                tempCombo.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                                tempCombo.Margin = new Thickness(0, 4, 20, 0);
                                break;
                            case "middle":
                                tempCombo.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                tempCombo.Margin = new Thickness(0, 4, 0, 0);
                                break;
                        }
                        tempCombo.UpdateLayout();
                        tempStack.Height += (tempCombo.ActualHeight + 4);
                        tempCombo.SelectedIndex = XMLCombo.selectedIndex;
                    }

                    Border split = new Border();
                    split.Height = 0.5;
                    split.BorderThickness = new Thickness(1, 1, 1, 1);
                    split.Margin = new Thickness(0, 4, 0, 0);
                    split.BorderBrush = new SolidColorBrush(ConvertTo.Color(block.SplitColor));
                    tempStack.Children.Add(split);
                    tempStack.Height += (split.Height + 4);
                }
                tempStack.UpdateLayout();
                totalHeight += (tempStack.ActualHeight + 2);
                if (totalHeight >= mainStackPanel.Height) { mainStackPanel.Height = totalHeight; }
                j = -1;
            }
        }

        //Load the item attributes to the option controls.
        private void LoadAttributes()
        {
            switch (EditType)
            {
                case"Text":
                    labelText tempLabel=((labelText)GetBlock.Contents[itemIndex]);
                    (returnPanel(2).Children[0] as TextBox).Text = tempLabel.text;
                    (returnPanel(4).Children[0] as ComboBox).SelectedIndex = (tempLabel.fontsize /2) -1;

                    ArrayList Array=new ArrayList(new string[5]{"regular","","bold","","italic"});//"" means the split.
                    int fsIndex=Array.IndexOf(tempLabel.fontstyle.ToLower());
                    (returnPanel(6).Children[fsIndex] as RadioButton).IsChecked = true;

                    Array = new ArrayList(new string[5] { "left","", "middle","", "right" });
                    int pIndex=Array.IndexOf(tempLabel.position.ToLower());
                    (returnPanel(10).Children[pIndex] as RadioButton).IsChecked = true;

                    (returnPanel(8).Children[0] as ComboBox).SelectedIndex=LoadColorCombo(8,tempLabel.forecolor);

                    break;
                case"RadioButton":
                    radioButton tempRadio=((radioButton)GetBlock.Contents[itemIndex]);
                    (returnPanel(2).Children[0] as TextBox).Text = tempRadio.text;
                    (returnPanel(4).Children[0] as ComboBox).SelectedIndex = (tempRadio.fontsize /2) -1;

                    Array=new ArrayList(new string[5]{"regular","","bold","","italic"});
                    fsIndex=Array.IndexOf(tempRadio.fontstyle.ToLower());
                    (returnPanel(6).Children[fsIndex] as RadioButton).IsChecked = true;

                    Array = new ArrayList(new string[5] { "left","", "middle","", "right" });
                    pIndex=Array.IndexOf(tempRadio.position.ToLower());
                    (returnPanel(10).Children[pIndex] as RadioButton).IsChecked = true;

                    (returnPanel(12).Children[0] as CheckBox).IsChecked = tempRadio.selected;

                    (returnPanel(8).Children[0] as ComboBox).SelectedIndex=LoadColorCombo(8,tempRadio.forecolor);

                    break;
                case "CheckBox":
                    checkBox tempCheck = ((checkBox)GetBlock.Contents[itemIndex]);
                    (returnPanel(2).Children[0] as TextBox).Text = tempCheck.text;
                    (returnPanel(4).Children[0] as ComboBox).SelectedIndex = (tempCheck.fontsize / 2) - 1;

                    Array = new ArrayList(new string[5] { "regular", "", "bold", "", "italic" });
                    fsIndex = Array.IndexOf(tempCheck.fontstyle.ToLower());
                    (returnPanel(6).Children[fsIndex] as RadioButton).IsChecked = true;

                    Array = new ArrayList(new string[5] { "left", "", "middle", "", "right" });
                    pIndex = Array.IndexOf(tempCheck.position.ToLower());
                    (returnPanel(10).Children[pIndex] as RadioButton).IsChecked = true;

                    (returnPanel(12).Children[0] as CheckBox).IsChecked = tempCheck.selected;

                    (returnPanel(8).Children[0] as ComboBox).SelectedIndex = LoadColorCombo(8, tempCheck.forecolor);

                    break;
                case "TextBox":
                    textBox tempText = ((textBox)GetBlock.Contents[itemIndex]);
                    (returnPanel(2).Children[0] as TextBox).Text = tempText.text;
                    (returnPanel(4).Children[0] as ComboBox).SelectedIndex = (tempText.fontsize / 2) - 1;

                    Array = new ArrayList(new string[5] { "regular", "", "bold", "", "italic" });
                    fsIndex = Array.IndexOf(tempText.fontstyle.ToLower());
                    (returnPanel(6).Children[fsIndex] as RadioButton).IsChecked = true;

                    Array = new ArrayList(new string[5] { "left", "", "middle", "", "right" });
                    pIndex = Array.IndexOf(tempText.position.ToLower());
                    (returnPanel(10).Children[pIndex] as RadioButton).IsChecked = true;

                    (returnPanel(12).Children[0] as CheckBox).IsChecked = tempText.isLong;
                    
                    (returnPanel(12).Children[2] as CheckBox).IsChecked = tempText.isLocked;

                    (returnPanel(8).Children[0] as ComboBox).SelectedIndex = LoadColorCombo(8, tempText.forecolor);

                    break;
                case "Block":
                    (returnPanel(2).Children[0] as ComboBox).SelectedIndex = LoadColorCombo(2, GetBlock.BackgroundColor);
                    (returnPanel(4).Children[0] as ComboBox).SelectedIndex = LoadColorCombo(4, GetBlock.SplitColor);

                    break;
                case "ComboBox":
                     comboBox tempCombo = ((comboBox)GetBlock.Contents[itemIndex]);
                     foreach (string item in tempCombo.texts)
                     {
                         (returnPanel(2).Children[0] as TextBox).Text += item+"\r\n";
                     }

                    (returnPanel(4).Children[0] as ComboBox).SelectedIndex = (tempCombo.fontsize / 2) - 1;

                    Array = new ArrayList(new string[5] { "regular", "", "bold", "", "italic" });
                    fsIndex = Array.IndexOf(tempCombo.fontstyle.ToLower());
                    (returnPanel(6).Children[fsIndex] as RadioButton).IsChecked = true;

                    Array = new ArrayList(new string[5] { "left", "", "middle", "", "right" });
                    pIndex = Array.IndexOf(tempCombo.position.ToLower());
                    (returnPanel(10).Children[pIndex] as RadioButton).IsChecked = true;

                    (returnPanel(8).Children[0] as ComboBox).SelectedIndex = LoadColorCombo(8, tempCombo.forecolor);

                    break;
                case "Attachment":
                    attachment tempAtt = ((attachment)GetBlock.Contents[itemIndex]);
                    (returnPanel(2).Children[0] as TextBox).Text = tempAtt.text;

                    foreach (string item in tempAtt.filter)
                    {
                        (returnPanel(4).Children[0] as TextBox).Text += item + "\r\n";
                    }

                    Array = new ArrayList(new string[5] { "left", "", "middle", "", "right" });
                    pIndex = Array.IndexOf(tempAtt.position.ToLower());
                    (returnPanel(6).Children[pIndex] as RadioButton).IsChecked = true;
                  
                    break;             
            }           
        }

        /* Load "Colors" to the "ComboBox".
           "int index" accept the "StackPanel" index where the "ComboBox" in.
           "string SelectColorName" accept a string of color.
           Return the index of "string SelectColorName" in the target "ComboBox" .
        */
        private int LoadColorCombo(int index,string SelectColorName="black")
        {
            int SelectColorIndex=-1;

            (returnPanel(index).Children[0] as ComboBox).ItemsSource = null;

            foreach (PropertyInfo pi in typeof(Colors).GetProperties())
            {
                TextBlock colorBlock = new TextBlock();
                colorBlock.Width = (returnPanel(index).Children[0] as ComboBox).Width;
                colorBlock.Height = 14;
                colorBlock.Background = new SolidColorBrush((Color)pi.GetValue(null));
                colorBlock.ToolTip = pi.Name;
                if (((Color)pi.GetValue(null)) == ConvertTo.Color(SelectColorName))
                {
                    SelectColorIndex = (returnPanel(index).Children[0] as ComboBox).Items.Count;
                }
                (returnPanel(index).Children[0] as ComboBox).Items.Add(colorBlock);
            }

            return SelectColorIndex;
        }

        //Return the "StackPanel" by its index.
        private StackPanel returnPanel(int index)
        {
            return (mainStackPanel.Children[index] as StackPanel);
        }

        //Save the change of item.
        private void SaveItemChange()
        {
            switch (EditType)
            {
                case"Text":
                    labelText tempLabel = new labelText(true);

                    tempLabel.text = (returnPanel(2).Children[0] as TextBox).Text;

                    tempLabel.fontsize = Convert.ToInt32((returnPanel(4).Children[0] as ComboBox).SelectedItem.ToString());

                    if ((returnPanel(6).Children[0] as RadioButton).IsChecked == true) { tempLabel.fontstyle = "regular"; }
                    else if ((returnPanel(6).Children[2] as RadioButton).IsChecked == true) { tempLabel.fontstyle = "bold"; }
                    else{ tempLabel.fontstyle = "italic"; }

                    tempLabel.forecolor = ConvertTo.Color(((returnPanel(8).Children[0] as ComboBox).SelectedItem as TextBlock).Background.ToString()).ToString();

                    if ((returnPanel(10).Children[0] as RadioButton).IsChecked == true) { tempLabel.position = "left"; }
                    else if ((returnPanel(10).Children[2] as RadioButton).IsChecked == true) { tempLabel.position = "middle"; }
                    else { tempLabel.position = "right"; }

                    ((block)MainWindow.hXML.blocks[blockIndex]).Contents[itemIndex] = tempLabel;
          
                    break;
                case"RadioButton":
                    radioButton tempRadio = new radioButton(true);

                    tempRadio.text = (returnPanel(2).Children[0] as TextBox).Text;

                    tempRadio.fontsize = Convert.ToInt32((returnPanel(4).Children[0] as ComboBox).SelectedItem.ToString());

                    if ((returnPanel(6).Children[0] as RadioButton).IsChecked == true) { tempRadio.fontstyle = "regular"; }
                    else if ((returnPanel(6).Children[2] as RadioButton).IsChecked == true) { tempRadio.fontstyle = "bold"; }
                    else{ tempRadio.fontstyle = "italic"; }

                    tempRadio.forecolor = ConvertTo.Color(((returnPanel(8).Children[0] as ComboBox).SelectedItem as TextBlock).Background.ToString()).ToString();

                    if ((returnPanel(10).Children[0] as RadioButton).IsChecked == true) { tempRadio.position = "left"; }
                    else if ((returnPanel(10).Children[2] as RadioButton).IsChecked == true) { tempRadio.position = "middle"; }
                    else { tempRadio.position = "right"; }

                    tempRadio.selected =Convert.ToBoolean((returnPanel(12).Children[0] as CheckBox).IsChecked);

                    ((block)MainWindow.hXML.blocks[blockIndex]).Contents[itemIndex] = tempRadio;

                    break;
                case "CheckBox":
                    checkBox tempCheck = new checkBox(true);

                    tempCheck.text = (returnPanel(2).Children[0] as TextBox).Text;

                    tempCheck.fontsize = Convert.ToInt32((returnPanel(4).Children[0] as ComboBox).SelectedItem.ToString());

                    if ((returnPanel(6).Children[0] as RadioButton).IsChecked == true) { tempCheck.fontstyle = "regular"; }
                    else if ((returnPanel(6).Children[2] as RadioButton).IsChecked == true) { tempCheck.fontstyle = "bold"; }
                    else { tempCheck.fontstyle = "italic"; }

                    tempCheck.forecolor = ConvertTo.Color(((returnPanel(8).Children[0] as ComboBox).SelectedItem as TextBlock).Background.ToString()).ToString();

                    if ((returnPanel(10).Children[0] as RadioButton).IsChecked == true) { tempCheck.position = "left"; }
                    else if ((returnPanel(10).Children[2] as RadioButton).IsChecked == true) { tempCheck.position = "middle"; }
                    else { tempCheck.position = "right"; }

                    tempCheck.selected = Convert.ToBoolean((returnPanel(12).Children[0] as CheckBox).IsChecked);

                    ((block)MainWindow.hXML.blocks[blockIndex]).Contents[itemIndex] = tempCheck;

                    break;
                case "Block":
                    block tempBlock = GetBlock;

                    tempBlock.BackgroundColor = ConvertTo.Color(((returnPanel(2).Children[0] as ComboBox).SelectedItem as TextBlock).Background.ToString()).ToString();

                    tempBlock.SplitColor = ConvertTo.Color(((returnPanel(4).Children[0] as ComboBox).SelectedItem as TextBlock).Background.ToString()).ToString();

                    MainWindow.hXML.blocks[blockIndex] = tempBlock;

                    break;
                case"TextBox":
                    textBox tempText = new textBox(true);

                    tempText.text = (returnPanel(2).Children[0] as TextBox).Text;

                    tempText.fontsize = Convert.ToInt32((returnPanel(4).Children[0] as ComboBox).SelectedItem.ToString());

                    if ((returnPanel(6).Children[0] as RadioButton).IsChecked == true) { tempText.fontstyle = "regular"; }
                    else if ((returnPanel(6).Children[2] as RadioButton).IsChecked == true) { tempText.fontstyle = "bold"; }
                    else { tempText.fontstyle = "italic"; }

                    tempText.forecolor = ConvertTo.Color(((returnPanel(8).Children[0] as ComboBox).SelectedItem as TextBlock).Background.ToString()).ToString();

                    if ((returnPanel(10).Children[0] as RadioButton).IsChecked == true) { tempText.position = "left"; }
                    else if ((returnPanel(10).Children[2] as RadioButton).IsChecked == true) { tempText.position = "middle"; }
                    else { tempText.position = "right"; }

                    tempText.isLong = Convert.ToBoolean((returnPanel(12).Children[0] as CheckBox).IsChecked);
                    tempText.isLocked = Convert.ToBoolean((returnPanel(12).Children[2] as CheckBox).IsChecked);

                    ((block)MainWindow.hXML.blocks[blockIndex]).Contents[itemIndex] = tempText;
                    break;
                case "ComboBox":
                    comboBox tempCombo = (comboBox)GetBlock.Contents[itemIndex];

                    tempCombo.texts = new ArrayList();
                    for (int i = 0; i < (returnPanel(2).Children[0] as TextBox).LineCount; i++)
                    {
                        string line = (returnPanel(2).Children[0] as TextBox).GetLineText(i);
                        if (line != "\r\n" && line!="")
                        {
                            tempCombo.texts.Add(line.Replace("\r\n",""));
                        }
                    }

                    tempCombo.fontsize = Convert.ToInt32((returnPanel(4).Children[0] as ComboBox).SelectedItem.ToString());

                    if ((returnPanel(6).Children[0] as RadioButton).IsChecked == true) { tempCombo.fontstyle = "regular"; }
                    else if ((returnPanel(6).Children[2] as RadioButton).IsChecked == true) { tempCombo.fontstyle = "bold"; }
                    else { tempCombo.fontstyle = "italic"; }

                    tempCombo.forecolor = ConvertTo.Color(((returnPanel(8).Children[0] as ComboBox).SelectedItem as TextBlock).Background.ToString()).ToString();

                    if ((returnPanel(10).Children[0] as RadioButton).IsChecked == true) { tempCombo.position = "left"; }
                    else if ((returnPanel(10).Children[2] as RadioButton).IsChecked == true) { tempCombo.position = "middle"; }
                    else { tempCombo.position = "right"; }

                    ((block)MainWindow.hXML.blocks[blockIndex]).Contents[itemIndex] = tempCombo;

                    break;
                case "Attachment":
                    attachment tempAtt = (attachment)GetBlock.Contents[itemIndex];

                    tempAtt.text = (returnPanel(2).Children[0] as TextBox).Text;

                    tempAtt.filter = new ArrayList();
                    for (int i = 0; i < (returnPanel(4).Children[0] as TextBox).LineCount; i++)
                    {
                        string line = ConvertTo.Extension((returnPanel(4).Children[0] as TextBox).GetLineText(i));
                        if (line != "\r\n" && line!="")
                        {
                            
                            tempAtt.filter.Add(line.Replace("\r\n", ""));
                        }
                    }

                    if ((returnPanel(6).Children[0] as RadioButton).IsChecked == true) { tempAtt.position = "left"; }
                    else if ((returnPanel(6).Children[2] as RadioButton).IsChecked == true) { tempAtt.position = "middle"; }
                    else { tempAtt.position = "right"; }

                    ((block)MainWindow.hXML.blocks[blockIndex]).Contents[itemIndex] = tempAtt;

                    break;
            }
        }
     
        private void SaveQuitClick(object sender, MouseButtonEventArgs e)
        {
            ((sender as Label).Parent as Border).Opacity = 0.75;
            SaveItemChange();
            (this.Owner as MainWindow).RefreshOwner(blockIndex,itemIndex);
            this.Close();
        }

        private void CancelClick(object sender, MouseButtonEventArgs e)
        {
            ((sender as Label).Parent as Border).Opacity = 0.75;
            this.Close();
        }
        private void ApplyClick(object sender, MouseButtonEventArgs e)
        {
            ((sender as Label).Parent as Border).Opacity = 0.75;
            SaveItemChange();
            (this.Owner as MainWindow).RefreshOwner(blockIndex,itemIndex);
        }
              
        private void ButtonMouseMove(object sender, MouseEventArgs e)
        {
            ((sender as Label).Parent as Border).Background = new SolidColorBrush(Colors.LightBlue);
            ((sender as Label).Parent as Border).Opacity = 0.75;
        }

        private void ButtonMouseLeave(object sender, MouseEventArgs e)
        {
            ((sender as Label).Parent as Border).Background= new SolidColorBrush(Colors.WhiteSmoke);
            ((sender as Label).Parent as Border).Opacity = 1;
        }

        private void ButtonMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((sender as Label).Parent as Border).Opacity = 0.5;
        }
    }
}
