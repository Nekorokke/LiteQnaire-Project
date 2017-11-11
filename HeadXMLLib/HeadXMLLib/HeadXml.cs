using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Xml;
using System.IO;

/*Library which can:
  (1)Provide structs for storing and editing attributes parse from xml file.
  (2)Provide an ArrayList named "blocks" including all the blocks information with items.
  (3)Read and parse a xml file into the "blocks".
  (4)Edit the "blocks" and then save it as a xml file.
  
   Example :
       <Example>
         <Block BackgroundColor="Black" SplitColor="Black">
            <labelText Text="Some Text" ForeColor="Black" .../>
             ...
         </Block>
          ...
       <Example>
*/
namespace HeadXMLLib
{
    //Structs.
    public struct labelText
    {
        public string text;
        public string position;
        public string forecolor;
        public string fontstyle;
        public int fontsize;
 
        public labelText(Boolean Activate)
        {
            text = "";
            position = "middle";
            forecolor = "black";
            fontstyle = "regular";
            fontsize = 12;
        }
    }

    public struct textBox
    {
        public string text;
        public string position;
        public string forecolor;
        public string fontstyle;
        public int fontsize;

        public Boolean isLong;
        public Boolean isLocked;
        public textBox(Boolean Activate)
        {
            text = "";
            position = "middle";
            forecolor = "black";
            isLocked = false;
            fontstyle = "regular";
            isLong = false;
            fontsize = 12;
        }
    }

    public struct radioButton
    {
        public string text;
        public string position;
        public Boolean selected;
        public string forecolor;
        public string fontstyle;
        public int fontsize;

        public radioButton(Boolean Activate)
        {
            text = "";
            position = "middle";
            selected = false;
            forecolor = "black";
            fontsize = 12;
            fontstyle = "regular";
        }
  
    }

    public struct checkBox
    {
        public string text;
        public string position;
        public Boolean selected;
        public string forecolor;
        public string fontstyle;
        public int fontsize;

        public checkBox(Boolean Activate)
        {
            text = "";
            position = "middle";
            selected = false;
            forecolor = "black";
            fontstyle = "regular";
            fontsize = 12;
        }
    }

    public struct comboBox
    {
        public ArrayList texts;
        public string position;
        public int selectedIndex;
        public string forecolor;
        public string fontstyle;
        public int fontsize;
        public comboBox(Boolean Activate)
        {
            texts = new ArrayList();
            position = "middle";
            selectedIndex = 0;
            forecolor = "black";
            fontstyle = "regular";
            fontsize = 12;
        }
    }

    public struct attachment
    {
        public ArrayList filter;
        public string filename;
        public int fileIndex;
        public string position;
        public string text;
        public Boolean isLocked;
        public attachment(Boolean Activate)
        {
            filename = "";
            filter = new ArrayList();
            fileIndex = -1;
            position = "middle";
            text = "";
            isLocked = false;
        }
    }

    public struct block
    {
        public ArrayList Contents;
        public string BackgroundColor;
        public string SplitColor;

        public block(Boolean Activate)
        {
            Contents = new ArrayList();
            BackgroundColor = "WhiteSmoke";
            SplitColor = "Gray";
        }
    }
    public class HeadXMLFile
    {
        public ArrayList blocks;

        private string file;

        //Instantiate with a target xml file.
        public HeadXMLFile(string File)
        {
            file = File;
            blocks = new ArrayList();
        }

        public HeadXMLFile()
        {
            blocks = new ArrayList();
        }

        /*Read and parse the target xml file into the "blocks".
          If isEditing=true,"attachment" items will not load their "fileindex" attributes(set as "-1").
          Return true when a lqn file have already attached files.        
        */
        public bool Read(bool isEditing=false)
        {
            FileStream fStream=new FileStream(file,FileMode.Open);
            StreamReader sReader = new StreamReader(fStream);

            return ReadStream(sReader,isEditing);
        }

        /* Read and parse a xml stream into the "blocks". 
           "TextReader Text" can accept "StringReader" or "StreamReader".       
        */
        public bool ReadStream(TextReader Text,bool isEditing=false)
        {
            bool haveAttachment=false;
            
            XmlDocument headXml = new XmlDocument();
            headXml.Load(Text);

            XmlElement xmlElement = headXml.DocumentElement;
            XmlNodeList blockNodeList = xmlElement.GetElementsByTagName("Block");
          
            foreach (XmlNode node in blockNodeList)
            {
                block block= new block(true);
             
                block.SplitColor=((XmlElement)node).GetAttribute("SplitColor");
                block.BackgroundColor = ((XmlElement)node).GetAttribute("BackgroundColor");

                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if(childNode.Name=="LabelText"){
                        labelText tempLabelText=new labelText(true);

                        tempLabelText.text = ((XmlElement)childNode).GetAttribute("Text");
                        tempLabelText.fontstyle = ((XmlElement)childNode).GetAttribute("FontStyle");
                        tempLabelText.fontsize = Convert.ToInt32(((XmlElement)childNode).GetAttribute("FontSize"));
                        tempLabelText.position =((XmlElement)childNode).GetAttribute("Position");
                        tempLabelText.forecolor = ((XmlElement)childNode).GetAttribute("ForeColor");

                        block.Contents.Add(tempLabelText);
                    }
                    else if (childNode.Name == "RadioButton")
                    {
                        radioButton tempRadioButton = new radioButton(true);

                        tempRadioButton.text = ((XmlElement)childNode).GetAttribute("Text");
                        tempRadioButton.fontstyle = ((XmlElement)childNode).GetAttribute("FontStyle");
                        tempRadioButton.position = ((XmlElement)childNode).GetAttribute("Position");
                        tempRadioButton.fontsize = Convert.ToInt32(((XmlElement)childNode).GetAttribute("FontSize"));
                        tempRadioButton.selected = Convert.ToBoolean(((XmlElement)childNode).GetAttribute("Selected"));
                        tempRadioButton.forecolor = ((XmlElement)childNode).GetAttribute("ForeColor");

                        block.Contents.Add(tempRadioButton);
                    }
                    else if (childNode.Name == "CheckBox")
                    {
                        checkBox tempCheckBox = new checkBox(true);

                        tempCheckBox.text = ((XmlElement)childNode).GetAttribute("Text");
                        tempCheckBox.fontstyle = ((XmlElement)childNode).GetAttribute("FontStyle");
                        tempCheckBox.position = ((XmlElement)childNode).GetAttribute("Position");
                        tempCheckBox.fontsize = Convert.ToInt32(((XmlElement)childNode).GetAttribute("FontSize"));
                        tempCheckBox.selected = Convert.ToBoolean(((XmlElement)childNode).GetAttribute("Selected"));
                        tempCheckBox.forecolor = ((XmlElement)childNode).GetAttribute("ForeColor");

                        block.Contents.Add(tempCheckBox);
                    }
                    else if (childNode.Name == "TextBox")
                    {
                        textBox tempTextBox = new textBox(true);

                        tempTextBox.text = ((XmlElement)childNode).GetAttribute("Text");
                        tempTextBox.fontsize = Convert.ToInt32(((XmlElement)childNode).GetAttribute("FontSize"));
                        tempTextBox.fontstyle = ((XmlElement)childNode).GetAttribute("FontStyle");
                        tempTextBox.position = ((XmlElement)childNode).GetAttribute("Position");
                        tempTextBox.forecolor = ((XmlElement)childNode).GetAttribute("ForeColor");
                        tempTextBox.isLocked = Convert.ToBoolean(((XmlElement)childNode).GetAttribute("Locked"));
                        tempTextBox.isLong = Convert.ToBoolean(((XmlElement)childNode).GetAttribute("Long"));

                        block.Contents.Add(tempTextBox);
                    }
                    else if (childNode.Name == "ComboBox")
                    {
                        comboBox tempCombo = new comboBox(true);      

                        tempCombo.fontstyle = ((XmlElement)childNode).GetAttribute("FontStyle");
                        tempCombo.position = ((XmlElement)childNode).GetAttribute("Position");
                        tempCombo.fontsize = Convert.ToInt32(((XmlElement)childNode).GetAttribute("FontSize"));

                        tempCombo.texts = new ArrayList();
                        foreach (XmlNode textNode in childNode.ChildNodes)
                        {
                            tempCombo.texts.Add(((XmlElement)textNode).InnerText);
                        }

                        tempCombo.selectedIndex = Convert.ToInt32(((XmlElement)childNode).GetAttribute("SelectedIndex"));
                        tempCombo.forecolor = ((XmlElement)childNode).GetAttribute("ForeColor");

                        block.Contents.Add(tempCombo);
                    }
                    else if (childNode.Name == "Attachment")
                    {
                        attachment tempAttachment = new attachment(true);

                        tempAttachment.filename = ((XmlElement)childNode).GetAttribute("FileName");
                        tempAttachment.position = ((XmlElement)childNode).GetAttribute("Position");
                        tempAttachment.text = ((XmlElement)childNode).GetAttribute("Text");
                        tempAttachment.isLocked = Convert.ToBoolean(((XmlElement)childNode).GetAttribute("Locked"));

                        tempAttachment.filter = new ArrayList();
                        foreach (XmlNode filterNode in childNode.ChildNodes)
                        {
                            tempAttachment.filter.Add(((XmlElement)filterNode).InnerText);
                        }

                        if (Convert.ToInt32(((XmlElement)childNode).GetAttribute("FileIndex")) >= 1)
                        {
                            haveAttachment = true;
                        }

                        if (isEditing == false)
                        {
                            tempAttachment.fileIndex = Convert.ToInt32(((XmlElement)childNode).GetAttribute("FileIndex"));
                        }
                        else { tempAttachment.fileIndex = -1; }

                        block.Contents.Add(tempAttachment);
                    }

                }
                blocks.Add(block);
            }

            Text.Close();

            return haveAttachment;
        }

        //Turn the "blocks" into a xml lines ArrayList.
        public ArrayList GetXMLArrayList()
        {
            ArrayList tempArray = new ArrayList();

            tempArray.Add("<LiteQnaire>");
            tempArray.Add("");

            foreach (block blk in blocks)
            {
                tempArray.Add(" <Block BackgroundColor=\""+blk.BackgroundColor
                                      +"\" SplitColor=\""+blk.SplitColor+"\">");
                foreach (var item in blk.Contents)
                {
                    foreach (string line in GetItemXML(item))
                    {
                        tempArray.Add(line);
                    }
                    
                }
                tempArray.Add(" </Block>");
                tempArray.Add("");               
            }

            tempArray.Add("</LiteQnaire>");
            return tempArray;
        }

        //Return an item's xml lines.
        public ArrayList GetItemXML(object item)
        {
            ArrayList tempArray = new ArrayList();

            if (item is labelText)
            {
                labelText XMLLabel = (labelText)item;
                tempArray.Add("  <LabelText " + "Text=\"" + XMLLabel.text
                    + "\" FontStyle=\"" + XMLLabel.fontstyle
                    + "\" FontSize=\"" + XMLLabel.fontsize.ToString()
                    + "\" ForeColor=\"" + XMLLabel.forecolor
                    + "\" Position=\"" + XMLLabel.position
                    + "\"/>");
            }
            if (item is radioButton)
            {
                radioButton XMLRadio = (radioButton)item;
                tempArray.Add("  <RadioButton " + "Text=\"" + XMLRadio.text
                    + "\" FontStyle=\"" + XMLRadio.fontstyle
                    + "\" FontSize=\"" + XMLRadio.fontsize.ToString()
                    + "\" ForeColor=\"" + XMLRadio.forecolor
                    + "\" Position=\"" + XMLRadio.position
                    + "\" Selected=\"" + XMLRadio.selected.ToString()
                    + "\"/>");
            }
            if (item is checkBox)
            {
                checkBox XMLCheck = (checkBox)item;
                tempArray.Add("  <CheckBox " + "Text=\"" + XMLCheck.text
                    + "\" FontStyle=\"" + XMLCheck.fontstyle
                    + "\" FontSize=\"" + XMLCheck.fontsize.ToString()
                    + "\" ForeColor=\"" + XMLCheck.forecolor
                    + "\" Position=\"" + XMLCheck.position
                    + "\" Selected=\"" + XMLCheck.selected.ToString()
                    + "\" />");
            }
            if (item is textBox)
            {
                textBox XMLText = (textBox)item;
                tempArray.Add("  <TextBox " + "Text=\"" + XMLText.text
                    + "\" FontStyle=\"" + XMLText.fontstyle
                    + "\" FontSize=\"" + XMLText.fontsize.ToString()
                    + "\" ForeColor=\"" + XMLText.forecolor
                    + "\" Position=\"" + XMLText.position
                    + "\" Long=\"" + XMLText.isLong.ToString()
                    + "\" Locked=\"" + XMLText.isLocked.ToString()
                    + "\"/>");
            }
            if (item is comboBox)
            {
                comboBox XMLCombo = (comboBox)item;
                tempArray.Add("  <ComboBox "
                    + "FontStyle=\"" + XMLCombo.fontstyle
                    + "\" FontSize=\"" + XMLCombo.fontsize.ToString()
                    + "\" ForeColor=\"" + XMLCombo.forecolor
                    + "\" Position=\"" + XMLCombo.position
                    + "\" SelectedIndex=\"" + XMLCombo.selectedIndex.ToString()
                    + "\">");
                foreach (string text in XMLCombo.texts)
                {
                    tempArray.Add("    <Text>" + text + "</Text>");
                }
                tempArray.Add("  </ComboBox>");
            }
            if (item is attachment)
            {
                attachment XMLAtt = (attachment)item;
                tempArray.Add("   <Attachment "
                    + "FileIndex=\"" + XMLAtt.fileIndex
                    + "\" FileName=\"" + XMLAtt.filename
                    + "\" Position=\"" + XMLAtt.position
                    + "\" Locked=\"" + XMLAtt.isLocked.ToString()
                    + "\" Text=\"" + XMLAtt.text
                    + "\">");
                foreach (string filter in XMLAtt.filter)
                {
                    tempArray.Add("    <Filter>" + filter + "</Filter>");
                }
                tempArray.Add("   </Attachment>");
            }

            return tempArray;
        }

        //Return true when the "ref HeadXMLFile targetHXML" is equal to mine.
        public bool CompareTo(ref HeadXMLFile targetHXML)
        {

            ArrayList myArray=GetXMLArrayList();
            ArrayList targetArray=targetHXML.GetXMLArrayList();
            if (myArray.Count == targetArray.Count)
            {
                int i = 0;
                foreach (string line in targetArray)
                {
                    if (myArray[i].ToString() != line) { return false; }
                    i++;
                }

            }
            else
            {
                return false;
            }

            return true;         
        }

        //Save the "blocks" as a xml file.
        public void Save()
        {
            FileStream fStream = new FileStream(file,FileMode.Create);
            StreamWriter sWriter = new StreamWriter(fStream);

            foreach (string text in GetXMLArrayList())
            {
                sWriter.WriteLine(text);
            }

            sWriter.Flush();
            sWriter.Close();
            fStream.Close();
        }
    }
}
