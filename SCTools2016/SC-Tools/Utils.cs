using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCTools
{
    public class Utils
    {
        public static string GetFilePath()
        {
            string path = "";

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                path = dlg.FileName;
            }

            return path;
        }

        public static string SaveFilePath()
        {
            string path = "";

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".text";
            dlg.Filter = "Text documents (.txt)|*.txt";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                path = dlg.FileName;
            }

            return path;
        }

        public static List<Point2d> GetPoint2Ds(string path)
        {
            List<Point2d> plist = new List<Point2d>();

            using (StreamReader sr = new StreamReader(path))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    string[] s = line.Trim().Split(',');
                    double x, y;
                    if (double.TryParse(s[0], out x) && double.TryParse(s[1], out y))
                    {
                        plist.Add(new Point2d(x, y));
                    }
                }
            }

            return plist;
        }

        public static List<Point3d> GetPoint3Ds(string path)
        {
            List<Point3d> plist = new List<Point3d>();

            using (StreamReader sr = new StreamReader(path))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    string[] s = line.Trim().Split(',');
                    double x, y, z;
                    if (double.TryParse(s[0], out x) && double.TryParse(s[1], out y) && double.TryParse(s[2], out z))
                    {
                        plist.Add(new Point3d(x, y, z));
                    }
                }
            }

            return plist;
        }

    }
}
