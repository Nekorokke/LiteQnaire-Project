using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace LiteQnaire
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

        public static int[] IandJ(string tag)
        {
            int i;
            int[] IandJ = new int[2];
            for (i = 0; i < tag.Count(); i++)
            {
                if (tag.Substring(i, 1) == ",")
                {
                    break;
                }
            }

            IandJ[0] = Convert.ToInt32(tag.Substring(i-1,i));
            IandJ[1] = Convert.ToInt32(tag.Substring(i + 1, tag.Count() - i - 1));

            return IandJ;
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

        public static string Size(int size)
        {
            if(size<=1024)
            {
                return size.ToString() + " B";
            }
            else if (size <= 1024 * 1024)
            {
                Decimal dsize = Convert.ToDecimal(size)/1024;
                return dsize.ToString("0.00") + " KB";
            }
            else if (size <= 1024 * 1024*1024)
            {
                Decimal dsize = Convert.ToDecimal(size) / (1024*1024);
                return dsize.ToString("0.00") + " MB";
            }
            else
            {
                Decimal dsize = Convert.ToDecimal(size) / (1024*1024*1024);
                return dsize.ToString("0.00") + " GB";
            }
        }
    }
}
