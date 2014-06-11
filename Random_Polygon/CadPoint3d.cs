using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Random_Polygon
{
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

    public class Points:List< CadPoint3d>
    {
        public void Add(List<System.Windows.Point> items)
        {
            foreach(System.Windows.Point pt in items)
            {
                this.Add(new CadPoint3d(pt.X, pt.Y, 0));
            }
        }        
    }
}
