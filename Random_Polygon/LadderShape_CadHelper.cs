using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD;
using System.IO;
using Random_Polygon.laddershape;
using System.Threading;

namespace CadHelper
{
    public class LadderShape_CadHelper
    {
        /// <summary>
        /// 获取梯形形边界生成的信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static LadderShapeRationConditionList GetLadderShapeInfo(string path)
        {
            LadderShapeRationConditionList result = null;
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                XmlSerializer xs = new XmlSerializer(typeof(LadderShapeRationConditionList));
                result = (LadderShapeRationConditionList)xs.Deserialize(stream);
            }

            return result;
        }

        private static Polyline3d GetBoundary(LadderShapeRationConditionList condition)
        {
            LadderShape ls = new LadderShape(condition.UpLayer, condition.DownLayer, condition.Height);
           
            return new Polyline3d(Poly3dType.SimplePoly, GetBoundaryPoints(ls), true);
            
        }

        private static Point3dCollection GetBoundaryPoints(LadderShape ls)
        {
            Point3dCollection p3dList = new Point3dCollection();
            foreach (System.Drawing.Point pt in ls.Points)
            {
                p3dList.Add(new Point3d(pt.X, pt.Y, 0));
            }
            return p3dList;
        }

        [CommandMethod("RunLadderShape")]
        public void RunLadderShape()
        {

            string filePath = CadHelper.GetSelectPath();
            if (filePath == "")
            {
                return;
            }
            string savePath = filePath.Replace(".xml", ".sat");
            LadderShapeRationConditionList conditonList = GetLadderShapeInfo(filePath);
            Polyline3d boundaryEntity = GetBoundary(conditonList);
            List<Polyline3d> interEntities = CadHelper.GetEntities(conditonList.CadPoint3dList.ToList());
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            string text = conditonList.ToString();
            CadHelper.InsertDescription(text, new Point3d(-100, 200, 0), db);

            CadHelper.ToModelSpace(boundaryEntity, db);
            foreach (Polyline3d entity in interEntities)
            {
                CadHelper.ToModelSpace(entity, db);
            } 
            
            //Document acDoc = Application.DocumentManager.MdiActiveDocument;
            //try
            //{
            //    //acDoc.SendStringToExecute("region\r", true, false, false);
            //    //acDoc.SendStringToExecute("_ai_selall\r", true, false, false);
            //    //Thread.Sleep(1500);
            //    //acDoc.SendStringToExecute("extrude\r", true, false, false);
            //    //acDoc.SendStringToExecute("_ai_selall\r", true, false, false);
            //    //Thread.Sleep(1500);
            //    //int height = conditonList.Height;
            //    //acDoc.SendStringToExecute(height.ToString() + "\r\r", true, false, false); 
               
            //}
            //catch (System.Exception ex)
            //{
            //    System.Windows.MessageBox.Show(ex.ToString());
            //}
            

           // acDoc.Database.SaveAs(savePath, acDoc.Database.SecurityParameters);


        }
    }
}
