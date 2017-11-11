using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

//Library for extracting files from source file made by "FileGlueLib".
namespace ExtractFileLib
{
    public class ExtractFile
    {
        private string sourceFile;
        public ExtractFile(string SourceFile)
        {
            sourceFile = SourceFile;//Set the source file path.
        }

        /* Extract a file as stream by its index in the source file.
           # Indexes begin from 1.
           # 0 stands for the last file index.
        */
        public byte[] ExtractStream(int fileIndex)
        {
            FileStream fStream = new FileStream(sourceFile, FileMode.Open);
            BinaryReader bReader = new BinaryReader(fStream);

            int maxFileCount = Convert.ToInt32(System.Text.Encoding.Default.GetString(bReader.ReadBytes(3)));//Get how much files the source file can have.
            int fileCount = Convert.ToInt32(System.Text.Encoding.Default.GetString(bReader.ReadBytes(3)));//Get how much files the source file now have.
            if (fileIndex == 0) { fileIndex = fileCount; }//Turn zero into the last file index.

            int tempLength = 0;
            int[] lengthArray = new int[fileIndex];
            for (int i = 0; i < fileCount; i++)
            {
                tempLength = Convert.ToInt32(System.Text.Encoding.Default.GetString(bReader.ReadBytes(15)));
                if (i <= fileIndex - 1)
                {
                    lengthArray[i] = tempLength;//Create an array of sizes until the appointed file index.
                }
            }
            bReader.BaseStream.Seek(6 + maxFileCount * 15 + lengthArray.Sum() - lengthArray[fileIndex - 1], SeekOrigin.Begin);//Move to the beginning of the appointed file.
            byte[] returnStream = bReader.ReadBytes(lengthArray[fileIndex - 1]);//Get the stream.

            bReader.Close();
            fStream.Close();

            return returnStream;
        }

        //Delete files in the source file by an array of file index.
        public void Delete(int[] fileIndexes)
        {
            FileStream fStream = new FileStream(sourceFile, FileMode.Open);
            BinaryReader bReader = new BinaryReader(fStream);

            int maxFileCount = Convert.ToInt32(System.Text.Encoding.Default.GetString(bReader.ReadBytes(3)));
            int fileCount = Convert.ToInt32(System.Text.Encoding.Default.GetString(bReader.ReadBytes(3)));
            for (int i = 0; i < fileIndexes.Count(); i++)
            {
                if (fileIndexes[i] == 0) { fileIndexes[i] = fileCount; }
            }

            int tempLength = 0;
            int[] lengthArray = new int[fileCount];
            for (int i = 0; i < fileCount; i++)
            {
                tempLength = Convert.ToInt32(System.Text.Encoding.Default.GetString(bReader.ReadBytes(15)));
                lengthArray[i] = tempLength;//Create a size array of all files.
            }

            FileStream fTempStream = new FileStream(sourceFile , FileMode.Create);
            BinaryWriter bWriter = new BinaryWriter(fTempStream);

            //Restruct the fHead.
            string fHead = maxFileCount.ToString().PadLeft(3, '0');
            fHead += (fileCount - fileIndexes.Count()).ToString().PadLeft(3, '0');
            int j = 0;
            for (int i = 0; i < lengthArray.Count(); i++)
            {
                if (i == (fileIndexes[j] - 1))
                {
                    if (j < (fileIndexes.Count() - 1)) { j += 1; }
                }
                else
                {
                    fHead += lengthArray[i].ToString().PadLeft(15, '0');
                }
            }
            fHead += new string('0', (maxFileCount - fileCount + fileIndexes.Count()) * 15);
            bWriter.Write(System.Text.Encoding.Default.GetBytes(fHead));

            //Jump over the files in "fileIndexes".
            bReader.BaseStream.Seek(6 + maxFileCount * 15, SeekOrigin.Begin);
            bool inList = false;
            for (int i = 0; i < lengthArray.Count(); i++)
            {
                for (int k = 0; k < fileIndexes.Count(); k++)
                {
                    if (i == fileIndexes[k] - 1) { inList = true; break; }
                }

                if (inList == true)
                {
                    bReader.BaseStream.Seek(lengthArray[i], SeekOrigin.Current);
                }
                else
                {
                    bWriter.Write(bReader.ReadBytes(lengthArray[i]));
                }
                inList = false;
            }
            bWriter.Flush();
            bWriter.Close();
            bReader.Close();
            fTempStream.Close();
            fStream.Close();
        }

        /* Extract file from the source file.
           "string path" accept the path file extract to.
           "int fileIndex" accept the index of the file we want to extract.
           
           # Indexes begin from 1.
           # 0 stands for the last file index.
        */
        public void ExtractTo(string path, int fileIndex)
        {
            FileStream createFile = new FileStream(path, FileMode.Create);
            BinaryWriter fWriter = new BinaryWriter(createFile);

            fWriter.Write(ExtractStream(fileIndex));

            fWriter.Flush();
            fWriter.Close();
            createFile.Close();
        }

        
        public int GetNowFileCount()
        {
            FileStream fStream = new FileStream(sourceFile, FileMode.Open);
            BinaryReader bReader = new BinaryReader(fStream);

            bReader.BaseStream.Seek(3, SeekOrigin.Begin);
            int nowFileCount = Convert.ToInt32(System.Text.Encoding.Default.GetString(bReader.ReadBytes(3)));

            bReader.Close();
            fStream.Close();
            return nowFileCount;
        }

        public int GetMaxFileCount()
        {
            FileStream fStream = new FileStream(sourceFile, FileMode.Open);
            BinaryReader bReader = new BinaryReader(fStream);

            int maxFileCount = Convert.ToInt32(System.Text.Encoding.Default.GetString(bReader.ReadBytes(3)));

            bReader.Close();
            fStream.Close();
            return maxFileCount;
        }

        public int GetSize(int fileIndex)
        {
            FileStream fStream = new FileStream(sourceFile, FileMode.Open);
            BinaryReader bReader = new BinaryReader(fStream);

            int Size;
            bReader.BaseStream.Seek(6+(fileIndex-1)*15,SeekOrigin.Begin);
            Size = Convert.ToInt32(System.Text.Encoding.Default.GetString(bReader.ReadBytes(15)));
            
            bReader.Close();
            fStream.Close();
            return Size;
        }
    }
}
