﻿using System;
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
using Autodesk.AutoCAD.EditorInput;

namespace CadHelper
{
    public class CadHelper
    { 
        public static void Move(Entity ent, Point3d basePt, Point3d targetPt)
        {
            Vector3d vec = targetPt - basePt;
            Matrix3d mt = Matrix3d.Displacement(vec);
            ent.TransformBy(mt);
        }
        
        public static List<Polyline3d> GetEntities(List<Points> pointsList)
        {
            List<Polyline3d> entityList = new List<Polyline3d>();
            foreach (Points pts in pointsList)
            {
                entityList.Add(CadHelper.getPolyline(pts.PointList, 0.0));
            }
             
            return entityList;
        }

        public static Polyline3d getPolyline(List<CadPoint3d> pointList,double height)
        {
            Point3dCollection vertices = new Point3dCollection();
            foreach (CadPoint3d pt in pointList)
            {
                vertices.Add(new Point3d(pt.X, pt.Y , pt.Z + height));
            }
            return new Polyline3d(Poly3dType.SimplePoly, vertices, true);
        }
         

        /// <summary>
        /// 添加对象到模型空间
        /// </summary>
        /// <param name="ent">要添加的对象</param>
        /// <returns>对象ObjectId</returns>
        public static ObjectId ToModelSpace(Entity ent, Database db)
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

        public static string GetSelectPath()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptResult result = ed.GetString("输入XML全路径:\n\r");
            string filePath = result.StringResult;
            if (!File.Exists(filePath))
            {
                ed.WriteMessage("文件不存在!");
                filePath = "";
            }
            return filePath;
        }

        

        public static void InsertDescription(string textString, Point3d position, Database db)
        {
            MText txt = new MText();
            txt.Location = position;
            txt.Contents = textString; 
            
            ToModelSpace(txt, db);
        }
    }
}
