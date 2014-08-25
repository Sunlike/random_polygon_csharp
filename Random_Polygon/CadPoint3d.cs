using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;


namespace Random_Polygon
{
    [Serializable]
    public class CadPoint3d
    {
        private double x = 0; 
        private double y = 0; 
        private double z = 0;

        public CadPoint3d()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        public CadPoint3d(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X
        {
            get { return x; }
            set { x = value; }
        }
       
        public double Y
        {
            get { return y; }
            set { y = value; }
        }
       
        public double Z
        {
            get { return z; }
            set { z = value; }
        }

    }
    [Serializable]
    public class Points
    {
        private List<CadPoint3d> pointList = new List<CadPoint3d>();
        public List<CadPoint3d> PointList
        {
            get { return pointList; }
            set { pointList = value; }
        }

        public void Add(List<System.Windows.Point> items)
        {
            foreach(System.Windows.Point pt in items)
            {
                PointList.Add(new CadPoint3d(pt.X, pt.Y, 0));
            }
        }

        private double m_Radius = 0.0;
        public double Radius
        {
            get { return m_Radius; }
            set { m_Radius = value; }
        }

        private CadPoint3d m_centerPoint = new CadPoint3d(0, 0, 0);
        public Random_Polygon.CadPoint3d CenterPoint
        {
            get { return m_centerPoint; }
            set { m_centerPoint = value; }
        }
    }
}
