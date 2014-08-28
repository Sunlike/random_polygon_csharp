using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Random_Polygon.circle;
using System.IO;
using System.Xml.Serialization;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD;
using Autodesk.AutoCAD.EditorInput;


namespace CadHelper
{
    public class Circle_CadHelper
    {
        /// <summary>
        /// 获取圆形边界生成的信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static CircleRatioConditionList GetCircleInfo(string path)
        {
            CircleRatioConditionList result = null;
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                XmlSerializer xs = new XmlSerializer(typeof(CircleRatioConditionList));
                result = (CircleRatioConditionList)xs.Deserialize(stream);
            }

            return result;
        }

        public static Circle GetCircleBoundary(CircleRatioConditionList circleCondition)
        {
            int R = circleCondition.Radius;
            Circle circle = new Circle(new Point3d(R, R, 0), new Vector3d(0, 0, 1), R);

            return circle;
        }

        

        [CommandMethod("RunCircle")]
        public void RunCircle1()
        {
            Database db = Application.DocumentManager.MdiActiveDocument.Database; 

            string filePath = CadHelper.GetSelectPath();
            if (filePath == "")
            {
                return;
            }
            string savePath = filePath.Replace(".xml", ".sat");
            CircleRatioConditionList conditonList = GetCircleInfo(filePath);

            Circle boundaryEntity = GetCircleBoundary(conditonList);
            List<Polyline3d> interEntities = CadHelper.GetEntities(conditonList.CadPoint3dList.ToList());
            
            string text = conditonList.ToString();
            CadHelper.InsertDescription(text, new Point3d(-100, 200, 0), db);
            CadHelper.ToModelSpace(boundaryEntity, db);
            foreach (Polyline3d entity in interEntities)
            {
                CadHelper.ToModelSpace(entity, db);
            }



            //Document acDoc = Application.DocumentManager.MdiActiveDocument;

            //acDoc.Database.SaveAs(savePath, acDoc.Database.SecurityParameters);


        }

    }
}
