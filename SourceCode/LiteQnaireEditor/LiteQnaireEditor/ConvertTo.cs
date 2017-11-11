using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace LiteQnaireEditor
{
    class ConvertTo
    {

        public static string FileType(string filename)
        {
            return Path.GetExtension(filename);
        }

        public static string FileName(string filepath)
        {
            return Path.GetFileName(filepath);
        }

        public static System.Windows.Media.Color Color(string color)
        {
            return (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color);
            
        }

        public static System.Windows.FontStyle FontStyle(string fontstyle)
        {
            switch (fontstyle.ToLower())
            {
                case "regular":
                    return System.Windows.FontStyles.Normal;
                case "italic":
                    return System.Windows.FontStyles.Italic;
                case "oblique":
                    return System.Windows.FontStyles.Oblique;
            }
            return System.Windows.FontStyles.Normal;
        }

        //Convert to a normalized extension string.
        public static string Extension(string line)
        {
            string[] replaceChar=new string[6]{"/","?",":","<",">"," "};

            foreach(string CHAR in replaceChar)
            {
                line = line.Replace(CHAR, "");
            }
                      
            if (line.Count() >= 4) 
            {
                if (line.Substring(0, 2) != "*." && line.Substring(0, 1) != ".") { line = "*." + line; }
                else if (line.Substring(0, 1) == ".") { line = "*" + line; }
            }
            else if (line.Count() == 3) { line = "*." + line; }

            return line;
        }
    }
}
