using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

//Library for combining files together.
namespace FileGlueLib
{
    public class FileGlue
    {
        private static string targetFile;

        public FileGlue(string TargetFile)
        {          
            targetFile = TargetFile;//Set the path of the final combined file.
        }

        /* Glue an array of files together.
           "string[] FileList" accept the path array.
           "int totalFileCount" accept the maximum number of files which final file can have.
        */
        public void Glue(string[] FileList,int maxFileCount)
        {
            FileStream fStream=new FileStream(targetFile,FileMode.Create);
            BinaryWriter bWriter = new BinaryWriter(fStream);         
            int fileCount = FileList.Count();

            /*"fHead" is a serials of numbers which described:
             (1)How much files the final file can have.(first three digits)
             (2)How much files the final file now have.(next three digits)
             (3)The size(bytes) of each files.(every fifteen digits)
            */
            string fHead = maxFileCount.ToString();
            fHead=fHead.PadLeft(3,'0');//Complete the digits by zero front.
            fHead += fileCount.ToString().PadLeft(3, '0');

            string strNowFileLength="";
            string strHEAD000 = new string('0',maxFileCount*15+6);
            bWriter.Write(System.Text.Encoding.Default.GetBytes(strHEAD000));//Leaving space for fHead beforehand.

            FileStream fTempStream;
            BinaryReader bTempReader;
            for (int i = 0; i < fileCount; i++)
            {
                fTempStream = new FileStream(FileList[i], FileMode.Open);
                bTempReader = new BinaryReader(fTempStream);

                bWriter.Write(bTempReader.ReadBytes((int)fTempStream.Length));//Read source file and write the stream into final file.
                strNowFileLength= fTempStream.Length.ToString();
                fHead += strNowFileLength.PadLeft(15, '0');//Record the size of source file.

                fTempStream.Close();
                bTempReader.Close();
            }

            bWriter.Seek(0,SeekOrigin.Begin);
            bWriter.Write(System.Text.Encoding.Default.GetBytes(fHead));//Finally write the fHead into the space we have leaved.

            bWriter.Flush();
            bWriter.Close();
            fStream.Close();
   
        }

        //Glue stream to the end of target file.
        public void GlueStream(byte[] stream) 
        {
            FileStream fStream = new FileStream(targetFile,FileMode.OpenOrCreate);
            BinaryWriter bWriter = new BinaryWriter(fStream);
            BinaryReader bReader = new BinaryReader(fStream);

            bReader.BaseStream.Seek(3, SeekOrigin.Begin);
            int FileCount = Convert.ToInt32(System.Text.Encoding.Default.GetString(bReader.ReadBytes(3)));//Get the number of files the target file now have.
            bWriter.Seek(3, SeekOrigin.Begin);
            bWriter.Write(System.Text.Encoding.Default.GetBytes((FileCount + 1).ToString().PadLeft(3, '0')));//Plus one.
         
            bWriter.Seek(6 + FileCount * 15, SeekOrigin.Begin);
            bWriter.Write(System.Text.Encoding.Default.GetBytes(stream.Count().ToString().PadLeft(15,'0')));//Write the size of new file.

            bWriter.Flush();
            bWriter.Close();
            bReader.Close();
            fStream.Close();

            fStream = new FileStream(targetFile, FileMode.Append);
            bWriter = new BinaryWriter(fStream);

            bWriter.Write(stream);//Write the new file.

            bWriter.Flush();
            bWriter.Close();
            fStream.Close();
        }

        //Read stream from a file.
        public byte[] GetFileStream(string file)
        {
            FileStream fStream = new FileStream(file,FileMode.Open);
            BinaryReader bReader=new BinaryReader(fStream);
            byte[] returnStream=bReader.ReadBytes((int)fStream.Length);
            bReader.Close();
            fStream.Close();
            return returnStream;
        }

        //Glue a file to the end of target file.
        public void GlueFile(string file)
        {
            GlueStream(GetFileStream(file));
        }
    }
}
