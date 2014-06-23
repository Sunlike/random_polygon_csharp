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
using Random_Polygon.rectangle;
namespace CadHelper
{
    public class Rectangle_CadHelper
    {
        /// <summary>
        /// 获取矩形形边界生成的信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static RectRationConditionList GetRectangleInfo(string path)
        {
            RectRationConditionList result = null;
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                XmlSerializer xs = new XmlSerializer(typeof(RectRationConditionList));
                result = (RectRationConditionList)xs.Deserialize(stream);
            }

            return result;
        }

        private static Polyline3d GetBoundary(RectRationConditionList condition)
        {
            double height = condition.BoundaryHeight;
            double width = condition.BoundaryWidth;
            Point3dCollection p3dList = new Point3dCollection();
            p3dList.Add(new Point3d(0,0,0));
            p3dList.Add(new Point3d(width,0,0));
            p3dList.Add(new Point3d(width,height,0));
            p3dList.Add(new Point3d(0,height,0));

            return new Polyline3d(Poly3dType.SimplePoly,p3dList,true);
        }

        [CommandMethod("RunRectangle")]
        public void RunRectangle()
        {
            OpenFileDialog openDialog = new OpenFileDialog("打开梯形边界中间文件", "", "xml", "打开", OpenFileDialog.OpenFileDialogFlags.AllowAnyExtension);
            bool? result = openDialog.ShowModal();
            if (result != null && result == true)
            {
                string filePath = openDialog.Filename;
                string savePath = filePath.Replace(".xml", ".sat");
                RectRationConditionList conditonList = GetRectangleInfo(filePath);
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

                Document acDoc = Application.DocumentManager.MdiActiveDocument;

                acDoc.Database.SaveAs(savePath, acDoc.Database.SecurityParameters);

            }
        }
    }
}
