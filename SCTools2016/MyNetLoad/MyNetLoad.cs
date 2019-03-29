using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Runtime;
using System.Reflection;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices;

namespace SCTools
{
    public class MyNetLoad
    {
        [CommandMethod("NLoad")]
        public void Load()
        {
            Editor acEd = Application.DocumentManager.MdiActiveDocument.Editor;
            string filepath = "";

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".dll"; 
            dlg.Filter = "DLL documents (.dll)|*.dll"; 

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                filepath = dlg.FileName;

                byte[] buffer = System.IO.File.ReadAllBytes(filepath);
                Assembly assembly = Assembly.Load(buffer);

                acEd.WriteMessage("\n加载dll文件：" + filepath);
            }

        }
    }
}
