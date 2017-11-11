using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Xml;
using System.IO;
using ExtractFileLib;

namespace LQNCollectorLib
{
    public struct extractInfo
    {
        public string indexString;
        public XmlDocument summaryXml;
        public ArrayList fileNames;
        public string targetFolder;

        public extractInfo(ref XmlDocument summaryXml,string indexString,ArrayList fileNames,string targetFolder)
        {
            this.indexString = indexString;
            this.summaryXml=summaryXml;
            this.fileNames=fileNames;
            this.targetFolder = targetFolder;
        }
    }
    public class AttachmentExtractor
    {
        public static void Extract(extractInfo efInfo)
        {
            ExtractFile eFile;

            XmlDocument summaryXml=efInfo.summaryXml;

            XmlNodeList summaryList=summaryXml.GetElementsByTagName("Summary");

            //Load information.
            ArrayList paths = new ArrayList();
            ArrayList attIndex = new ArrayList();
            ArrayList attFileNames = new ArrayList();
            foreach(XmlNode node in summaryList)
            {
                paths.Add(((XmlElement)node).GetAttribute("path"));
                attIndex.Add(Convert.ToInt32(((XmlElement)node.SelectSingleNode(".//Item[@Index=" + efInfo.indexString + "]")).GetAttribute("FileIndex")));
                attFileNames.Add(((XmlElement)node.SelectSingleNode(".//Item[@Index=" + efInfo.indexString + "]")).InnerText);
            }

            ArrayList fileNames = new ArrayList();
            if (efInfo.fileNames[0] is string[])//The "string[]" type means naming the extracted files by other inner texts, string[0] is the index string.
            {
                foreach (XmlNode node in summaryList)
                {
                    fileNames.Add(((XmlElement)node.SelectSingleNode(".//Item[@Index=" + (efInfo.fileNames[0] as string[])[0] + "]")).InnerText);
                }
            }
            else
            {
                fileNames = efInfo.fileNames;
            }

            int i = 0;
            foreach (string path in paths)
            {
                eFile = new ExtractFile(path);

                if ((int)attIndex[i] > 0)
                {
                    eFile.ExtractTo(GetCurrectFileName(efInfo.targetFolder + (string)fileNames[i]+Path.GetExtension(attFileNames[i] as string)) , (int)attIndex[i]);
                }
                i++;
            }
        }

        //Add '_' before the file name while there is already a file at that path.
        private static string GetCurrectFileName(string path)
        {
            string tempPath = path;

            while(File.Exists(tempPath))
            {
                tempPath = Path.GetDirectoryName(tempPath)  + "\\_"+Path.GetFileName(tempPath);
            }

            return tempPath;
        }
    }
}
