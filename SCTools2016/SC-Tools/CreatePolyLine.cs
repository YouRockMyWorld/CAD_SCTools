using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace SCTools
{
    public class CreatePolyline
    {
        private Document acDoc;
        private Database acCurDb;
        private Editor acDocEd;

        public CreatePolyline()
        {
            acDoc = Application.DocumentManager.MdiActiveDocument;
            acCurDb = acDoc.Database;
            acDocEd = acDoc.Editor;
        }

        [CommandMethod("CPL")]
        public void CPL()
        {
            try
            {
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    BlockTable acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                    BlockTableRecord acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    string file = Utils.GetFilePath();
                    acDocEd.WriteMessage("打开文件:" + file + "\n");
                    List<Point2d> p_list = Utils.GetPoint2Ds(file);
                    using (Polyline acPoly = new Polyline(p_list.Count))
                    {
                        int i = 0;
                        foreach (Point2d p in p_list)
                        {
                            acPoly.AddVertexAt(i, p, 0, 0, 0);
                            i++;
                        }

                        acBlkTblRec.AppendEntity(acPoly);
                        acTrans.AddNewlyCreatedDBObject(acPoly, true);
                    }
                    acDoc.SendStringToExecute("._zoom _e ", true, false, false);
                    acDocEd.WriteMessage("命令执行完毕\n");
                    acTrans.Commit();
                    acTrans.Dispose();
                }
            }
            catch(Autodesk.AutoCAD.Runtime.Exception ex)
            {
                acDocEd.WriteMessage(ex.Message);
            }
            catch(System.Exception ex)
            {
                acDocEd.WriteMessage(ex.Message);
            }
        }


    }
}
