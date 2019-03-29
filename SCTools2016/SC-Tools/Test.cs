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
    public class Test
    {
        private Document acDoc;
        private Database acCurDb;
        private Editor acDocEd;

        public Test()
        {
            acDoc = Application.DocumentManager.MdiActiveDocument;
            acCurDb = acDoc.Database;
            acDocEd = acDoc.Editor;
        }

        [CommandMethod("Test1")]
        public void Test1()
        {
            try
            {
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    BlockTable acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                    BlockTableRecord acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    // Request for objects to be selected in the drawing area
                    PromptSelectionResult acSSPrompt = acDocEd.GetSelection();

                    // If the prompt status is OK, objects were selected
                    if (acSSPrompt.Status == PromptStatus.OK)
                    {
                        SelectionSet acSSet = acSSPrompt.Value;
                        int i = 0;
                        // Step through the objects in the selection set
                        foreach (SelectedObject acSSObj in acSSet)
                        {
                            // Check to make sure a valid SelectedObject object was returned
                            if (acSSObj != null)
                            {
                                // Open the selected object for write
                                Entity acEnt = acTrans.GetObject(acSSObj.ObjectId, OpenMode.ForWrite) as Entity;

                                if (acEnt != null)
                                {
                                    if (acEnt is Arc)
                                    {
                                        acEnt.ColorIndex = 3;
                                        i++;
                                        Arc arc = (Arc)acEnt;
                                        if (arc == null) return;
                                        double a = arc.TotalAngle / Math.PI * 180;
                                        //List<Line> lines = divide_arc(arc, (int)a);
                                        List<Line> lines = divide_curve(arc, (int)Math.Ceiling(a));
                                        foreach (var l in lines)
                                        {
                                            acBlkTblRec.AppendEntity(l);
                                            acTrans.AddNewlyCreatedDBObject(l, true);
                                        }
                                        acDocEd.WriteMessage($"START:{((Arc)acEnt).StartParam}   END:{((Arc)acEnt).EndParam}\n");
                                    }
                                }
                            }
                        }
                        acDocEd.WriteMessage($"Arc:{i}\n");
                        acTrans.Commit();
                        acTrans.Dispose();
                    }
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

        //private List<Line> divide_arc(Arc arc, int count)
        //{
        //    List<Line> lines = new List<Line>();

        //    double l = (arc.EndParam - arc.StartParam) / count;
        //    double[] paras = new double[count + 1];
        //    for(int i = 0; i < paras.Length; ++i)
        //    {
        //        paras[i] = arc.StartParam + l * i;
        //    }

        //    Point3d[] point3Ds = new Point3d[count + 1];
        //    for(int i = 0;i < paras.Length; ++i)
        //    {
        //        acEd.WriteMessage($"{paras[i]}\n");
        //        point3Ds[i] = arc.GetPointAtParameter(paras[i]);
        //        acEd.WriteMessage($"{point3Ds[i]}\n");
        //    }

        //    for(int i =0;i <point3Ds.Length - 1; ++i)
        //    {
        //        Line line = new Line(point3Ds[i], point3Ds[i + 1]);
        //        lines.Add(line);
        //    }

        //    return lines;
        //}

        private List<Line> divide_curve(Curve curve, int count)
        {
            List<Line> lines = new List<Line>();

            double l = (curve.EndParam - curve.StartParam) / count;
            double[] paras = new double[count + 1];
            for (int i = 0; i < paras.Length; ++i)
            {
                paras[i] = curve.StartParam + l * i;
            }

            Point3d[] point3Ds = new Point3d[count + 1];
            for (int i = 0; i < paras.Length; ++i)
            {
                acDocEd.WriteMessage($"{paras[i]}\n");
                point3Ds[i] = curve.GetPointAtParameter(paras[i]);
                acDocEd.WriteMessage($"{point3Ds[i]}\n");
            }

            for (int i = 0; i < point3Ds.Length - 1; ++i)
            {
                Line line = new Line(point3Ds[i], point3Ds[i + 1]);
                lines.Add(line);
            }

            return lines;
        }
    }
}
