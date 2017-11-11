using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeadXMLLib;
using ExtractFileLib;
using FileGlueLib;
using System.IO;
using System.Collections;
using System.Xml;

namespace LQNCollectorLib
{
    /* Collect editable attributes of lqn files(in same template) and fill them into a summary xml.
     
       Example :
       <Summaries>
         <Summary path="c:\test.lqn">
            <Item Index="1-1">Some Text</Item>
             ...
         </Summary>
          ...
       <Summaries>
     
    */
    public class SummaryManagement
    {
        /* All the lqn files must in same template as it.
           Without the case of loading or saving a lqnc file,it isn't neccessary.
        */
        public HeadXMLFile TemplateHXML;

        public XmlDocument SummaryXml{get;private set;}//Created summary xml.

        public ArrayList Files;//lqn file paths

        private ArrayList SummaryArray;//String ArrayList form of the summary xml.

        private string LQNCFile; //lqnc file path,a package of template xml and summary xml.

        public SummaryManagement()
        {
            SummaryArray = new ArrayList();
        }

        //"ArrayList files" accept an ArrayList of lqn file paths.
        public void SummarizeFromFiles(ArrayList files)
        {
            Files=files;
            SummaryArray=GetSummaryArray();
            SummaryXml = ChangeArrayIntoXml(SummaryArray);
        }

        private XmlDocument ChangeArrayIntoXml(ArrayList arraylist)
        {
            string XmlString="";
            foreach(string line in arraylist)
            {
                XmlString+=line+"\r\n";
            }

            StringReader strReader=new StringReader(XmlString);
            XmlDocument tempXml = new XmlDocument();
            tempXml.Load(strReader);

            return tempXml;
        }

        private ArrayList GetSummaryArray()
        {
            ArrayList SummaryXML = new ArrayList();
            SummaryXML.Add("<Summaries>");
            SummaryXML.Add("");
            int j = 0;//item index
            int i = 0;//block index

            foreach(string file in Files)
            {
                ExtractFile EFile = new ExtractFile(file);

                //Load the editable xml file.
                EFile.ExtractTo(file+"_temp",0);
                HeadXMLFile tempXML = new HeadXMLFile(file + "_temp");
                tempXML.Read();
                File.Delete(file + "_temp");

                SummaryXML.Add(" <Summary path=\""+file+"\">");
                foreach (block blk in tempXML.blocks)
                {
                    i++;
                    foreach (var item in blk.Contents)
                    {
                        j++;
                        string ij = "Item Index=\"" +i.ToString() + "," + j.ToString()+"\"";//index string
                        
                        if(item is labelText)
                        {                     
                            SummaryXML.Add("  <" + ij +">"
                                          +((labelText)item).text
                                          +"</Item>");
                        }
                        else if (item is textBox)
                        {                            
                            SummaryXML.Add("  <" + ij + ">"
                                          + ((textBox)item).text
                                          + "</Item>");
                        }
                        else if (item is radioButton)
                        {
                            if (((radioButton)item).selected == true)//While true write its text.
                            {
                                SummaryXML.Add("  <" + ij + ">"
                                              + ((radioButton)item).text
                                              + "</Item>");
                            }
                            else//While false write nothing(make it can be filtered by "BlankCheck").
                            {
                                SummaryXML.Add("  <" + ij + ">"                                             
                                             + "</Item>");
                            }
                        }
                        else if (item is checkBox)
                        {
                            if (((checkBox)item).selected == true)
                            {
                                SummaryXML.Add("  <" + ij + ">"
                                              + ((checkBox)item).text
                                              + "</Item>");
                            }
                            else
                            {
                                SummaryXML.Add("  <" + ij + ">"
                                             + "</Item>");
                            }
                        }
                        else if (item is comboBox)
                        {                           
                            SummaryXML.Add("  <" + ij + ">"
                                          + ((comboBox)item).texts[((comboBox)item).selectedIndex].ToString()//Write text corresponding with the "selectedIndex".
                                          + "</Item>");
                        }
                        else if (item is attachment)
                        {                          
                            SummaryXML.Add("  <" + ij 
                                          + " FileIndex=\""//It has an extra attribute to show its file index in a lqn file.
                                          + ((attachment)item).fileIndex.ToString()
                                          + "\">"
                                          + ((attachment)item).filename
                                          + "</Item>");
                        }
                    }

                    j = 0;
                }

                SummaryXML.Add(" </Summary>");
                SummaryXML.Add("");

                i = 0;
            }

            SummaryXML.Add("</Summaries>");

            return SummaryXML;
        }

        //Save a package of template xml and summary xml.
        public void SaveLQNCFile(string savefilePath)
        {
            FileStream fStream = new FileStream(savefilePath+".smry.xml", FileMode.Create);
            StreamWriter sWriter = new StreamWriter(fStream);

            HeadXMLFile tempHXML = new HeadXMLFile(savefilePath + ".head.xml");
            tempHXML.blocks = TemplateHXML.blocks;
            tempHXML.Save();

            foreach (string line in GetSummaryArray())
            {
                sWriter.WriteLine(line);
            }

            sWriter.Flush();
            sWriter.Close();
            fStream.Close();

            FileGlue FGlue = new FileGlue(savefilePath);
            FGlue.Glue(new string[2] { savefilePath + ".head.xml", savefilePath + ".smry.xml" }, 2);

            File.Delete(savefilePath + ".head.xml");
            File.Delete(savefilePath + ".smry.xml");
        }

        //Get an inner text by i (block index),j (item index) and path (source lqn fie path).
        public string GetInnerText(int i, int j, string path)
        {
            string indexString = "\"" + i.ToString() + "," + j.ToString() + "\"";
            string innerText = SummaryXml.SelectSingleNode("/Summary[@path=\"" + path + "\"]/Item[@Index=" + indexString + "]").InnerText;
            return innerText;
        }

        //Get an ArrayList of innder texts by i and j.
        public ArrayList GetInnerTexts(int i,int j)
        {
            ArrayList tempArray = new ArrayList();
            string indexString = "\"" + i.ToString() + "," + j.ToString() + "\"";

            XmlNodeList summaryNodes = SummaryXml.GetElementsByTagName("Summary");

            foreach (XmlNode node in summaryNodes)
            {
                string innerText = node.SelectSingleNode(".//Item[@Index=" + indexString + "]").InnerText;
                tempArray.Add(innerText);
            }

            return tempArray;
        }

        /* Check if there are repeated inner texts.
         
           Struct of its returned ArrayList:
         
           repetitionList :0 -> repetition(1)[type = ArrayList] :0 -> repeated value[type = string]
                                                                :1 -> paths of source lqn files[type = string[]]
                          :1 -> repetition(2)...
        */
        public ArrayList checkRepetition(int i,int j)
        {
            ArrayList repetitionList = new ArrayList();

            Dictionary<string,string> dic=new Dictionary<string,string>();

            string indexString = "\"" + i.ToString() + "," + j.ToString() + "\"";

            XmlNodeList summaryNodes = SummaryXml.GetElementsByTagName("Summary");

            foreach (XmlNode node in summaryNodes)
            {
                string path = ((XmlElement)node).GetAttribute("path");
                string innerText = node.SelectSingleNode(".//Item[@Index=" + indexString + "]").InnerText;
                dic.Add(path, innerText);
            }

            string[] queryArray;
            foreach (KeyValuePair<string, string> kvp in dic)
            {
                var query = from d in dic
                        where d.Value == kvp.Value
                        select d.Key;

                queryArray = query.ToArray<string>();

                bool haveChecked=false;
                if (queryArray.Count() > 1)//Have repititions.
                {
                    foreach (ArrayList array in repetitionList)
                    {
                        if ((string)array[0] == kvp.Value)//It is already in the list.
                        {
                            haveChecked = true;
                        }
                    }

                    if (haveChecked == true) { continue; }

                    ArrayList tempArray = new ArrayList();
                    tempArray.Add(kvp.Value);
                    tempArray.Add(queryArray);

                    repetitionList.Add(tempArray);
                }
            }

            return repetitionList;          
        }

        // Check if there are blank inner texts.
        public ArrayList checkBlank(int i, int j)
        {
            ArrayList blankList = new ArrayList();

            string indexString = "\"" + i.ToString() + "," + j.ToString() + "\"";

            XmlNodeList summaryNodes = SummaryXml.GetElementsByTagName("Summary");

            foreach (XmlNode node in summaryNodes)
            {
                string path = ((XmlElement)node).GetAttribute("path");
                string innerText = node.SelectSingleNode(".//Item[@Index=" + indexString + "]").InnerText;

                if (innerText.Trim(' ') == "")
                {
                    blankList.Add(path);
                }
            }

            return blankList;
        }

        public void LoadFromLQNCFile(string LQNCFilepath)
        {
            LQNCFile = LQNCFilepath;

            ExtractFile EFile = new ExtractFile(LQNCFile);
            EFile.ExtractTo(LQNCFilepath+".head.xml",1);
            EFile.ExtractTo(LQNCFilepath + ".smry.xml", 0);

            SummaryXml = new XmlDocument();
            SummaryXml.Load(LQNCFilepath + ".smry.xml");

            TemplateHXML = new HeadXMLFile(LQNCFilepath + ".head.xml");
            TemplateHXML.Read();

            File.Delete(LQNCFilepath + ".head.xml");
            File.Delete(LQNCFilepath + ".smry.xml");
        }      
    }
}
