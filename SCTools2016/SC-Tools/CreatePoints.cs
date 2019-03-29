using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace SCTools
{
    public class CreatePoints
    {
        private Document acDoc;
        private Database acCurDb;
        private Editor acDocEd;

        public CreatePoints()
        {
            acDoc = Application.DocumentManager.MdiActiveDocument;
            acCurDb = acDoc.Database;
            acDocEd = acDoc.Editor;
        }

        [CommandMethod("CPoints")]
        public void CPoints()
        {
            try
            {
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    BlockTable acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                    BlockTableRecord acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    string file = Utils.GetFilePath();
                    acDocEd.WriteMessage("打开文件:" + file + "\n");
                    List<Point3d> p_list = Utils.GetPoint3Ds(file);

                    foreach(Point3d p in p_list)
                    {
                        DBPoint point = new DBPoint(p);

                        acBlkTblRec.AppendEntity(point);
                        acTrans.AddNewlyCreatedDBObject(point, true);
                    }

                    acCurDb.Pdmode = 34;
                    //acCurDb.Pdsize = 0;

                    acDoc.SendStringToExecute("._zoom _e ", true, false, false);
                    acDocEd.WriteMessage("命令执行完毕\n");
                    acTrans.Commit();
                    acTrans.Dispose();
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                acDocEd.WriteMessage(ex.Message);
            }
            catch (System.Exception ex)
            {
                acDocEd.WriteMessage(ex.Message);
            }
        }
    }
}
