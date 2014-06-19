using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.DatabaseServices;
using Random_Polygon.circle;
using System.Xml.Serialization;
using System.IO;
using Random_Polygon.laddershape;
using Random_Polygon.rectangle;
using Autodesk.AutoCAD.Geometry;
using Random_Polygon;
using Autodesk.AutoCAD.Windows;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD;

namespace CadHelper
{
    public class CadHelper
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
            Circle circle = new Circle(new Point3d(R,R,0),new Vector3d(0,0,1),R);
            
            return circle;
        }

        public static List<Polyline3d> GetCircleEntity(List<Points> pointsList)
        {
            List<Polyline3d> entityList = new List<Polyline3d>();
            foreach (List<CadPoint3d> pts in pointsList)
            {
                entityList.Add(CadHelper.getPolyline(pts));
            }
             
            return entityList;
        }

        public static Polyline3d getPolyline(List<CadPoint3d> pointList)
        {
            Point3dCollection vertices = new Point3dCollection();
            foreach (CadPoint3d pt in pointList)
            {
                vertices.Add(new Point3d(pt.X, pt.Y, pt.Z));
            }
            return new Polyline3d(Poly3dType.SimplePoly, vertices, true);
        }

        [CommandMethod("CreateDocument")]
        public void CreateDocument()
        {
            Document doc = Application.DocumentManager.Add("");
            Application.DocumentManager.MdiActiveDocument = doc;
        }

        [CommandMethod("RunCircle")]
        public void RunCircle()
        {
            OpenFileDialog openDialog = new OpenFileDialog("打开圆形边界中间文件","","xml","打开",OpenFileDialog.OpenFileDialogFlags.AllowAnyExtension);
            bool? result = openDialog.ShowModal();
            if (result != null && result == true)
            {
                string filePath = openDialog.Filename;
                string savePath = filePath.Replace(".xml", ".sat");
                CircleRatioConditionList conditonList = GetCircleInfo(filePath);
                Circle boundaryEntity =GetCircleBoundary(conditonList);
                List<Polyline3d> interEntities = GetCircleEntity(conditonList.CadPoint3dList.ToList()); 
                Database db = Application.DocumentManager.MdiActiveDocument.Database; 
                ToModelSpace(boundaryEntity, db);
                foreach (Polyline3d entity in interEntities)
                {
                    ToModelSpace(entity, db);
                }
                
                Document acDoc = Application.DocumentManager.MdiActiveDocument;

                acDoc.Database.SaveAs(savePath, acDoc.Database.SecurityParameters);
               
            }
        }



        /// <summary>
        /// 添加对象到模型空间
        /// </summary>
        /// <param name="ent">要添加的对象</param>
        /// <returns>对象ObjectId</returns>
        static ObjectId ToModelSpace(Entity ent, Database db)
        {
            
            ObjectId entId;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord modelSpace = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                entId = modelSpace.AppendEntity(ent);
                trans.AddNewlyCreatedDBObject(ent, true);
                trans.Commit();
            }
            return entId;
        }
    }
}
