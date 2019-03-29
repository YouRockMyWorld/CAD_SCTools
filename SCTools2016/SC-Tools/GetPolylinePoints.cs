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
using System.IO;

namespace SCTools
{
    public class GetPolylinePoints
    {
        private Document acDoc;
        private Database acCurDb;
        private Editor acDocEd;

        public GetPolylinePoints()
        {
            acDoc = Application.DocumentManager.MdiActiveDocument;
            acCurDb = acDoc.Database;
            acDocEd = acDoc.Editor;
        }

        [CommandMethod("GPLP")]
        public void GetPlinePoints()
        {
            try
            {

                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    BlockTable acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                    BlockTableRecord acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    //// Create a TypedValue array to define the filter criteria
                    //TypedValue[] acTypValAr = new TypedValue[1];
                    //acTypValAr.SetValue(new TypedValue((int)DxfCode.Start, "Polyline"), 0);

                    //// Assign the filter criteria to a SelectionFilter object
                    //SelectionFilter acSelFtr = new SelectionFilter(acTypValAr);

                    // Request for objects to be selected in the drawing area
                    PromptSelectionResult acSSPrompt = acDocEd.GetSelection();

                    // If the prompt status is OK, objects were selected
                    if (acSSPrompt.Status == PromptStatus.OK)
                    {
                        SelectionSet acSSet = acSSPrompt.Value;

                        foreach (SelectedObject acSSObj in acSSet)
                        {
                            if (acSSObj != null)
                            {
                                Entity acEnt = acTrans.GetObject(acSSObj.ObjectId, OpenMode.ForWrite) as Entity;

                                if(acEnt != null && acEnt is Polyline)
                                {
                                    acDocEd.WriteMessage("FIND Polyline\n");
                                    Polyline polyline = (Polyline)acEnt;
                                    if (polyline != null)
                                    {
                                        List<Point3d> points = GetPoint3Ds(polyline);
                                        WriteToFile(points);
                                    }
                                }

                            }
                        }
                    }
                    acDocEd.WriteMessage("命令执行完毕\n");
                    acTrans.Commit();
                    acTrans.Dispose();
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                acDocEd.WriteMessage(ex.Message + ex.StackTrace + ex.StackTrace);
            }
            catch (System.Exception ex)
            {
                acDocEd.WriteMessage(ex.Message + ex.TargetSite + ex.StackTrace);
            }
        }

        private List<Point3d> GetPoint3Ds(Polyline polyline)
        {
            List<Point3d> point3Ds = new List<Point3d>();
            for(int i = 0; i < polyline.NumberOfVertices; ++i)
            {
                point3Ds.Add(polyline.GetPoint3dAt(i));
            }

            return point3Ds;
        }

        private void WriteToFile(List<Point3d> points)
        {
            string path = Utils.SaveFilePath();

            if(path != "")
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    foreach (Point3d p in points)
                    {
                        sw.WriteLine($"{p.X},{p.Y},{p.Z}");
                    }
                }
            }

        }
    }
}
