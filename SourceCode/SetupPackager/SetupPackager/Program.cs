using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileGlueLib;
using System.IO;

namespace SetupPackager
{
    //Create a packaged file used in "LiteQnaire Setup".
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Folder Path : ");//Example : c:/test ,everything under it will be packaged(without "c:\test" itself).
            string folderPath = Console.ReadLine();
            Console.Write("Final File Path : ");//Example : d:/package.pkg .
            string ffilePath = Console.ReadLine();

            FileStream fStream=new FileStream(ffilePath + ".info",FileMode.Create);
            StreamWriter sWriter = new StreamWriter(fStream);

            string[] paths=Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
            foreach (string path in paths)
            {
               sWriter.WriteLine(path.Replace(folderPath,""));
            }

            sWriter.Close();
            fStream.Close();

            FileGlue fGlue = new FileGlue(ffilePath);
            fGlue.Glue(paths, paths.Count() + 1);
            fGlue.GlueFile(ffilePath + ".info");

            Console.ReadKey();
        }
    }
}
