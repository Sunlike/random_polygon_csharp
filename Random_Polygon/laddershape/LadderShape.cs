using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Random_Polygon.laddershape
{
    public class LadderShape
    {
        // 默认上底小，下底大
        public LadderShape(double up_layer,double down_layer,double height)
        {
            this.down_layer = down_layer > up_layer ? down_layer : up_layer;
            this.up_layer = down_layer <= up_layer ? down_layer : up_layer; ;
            this.height = height;
            Initlaize();
          
        }
    

        #region  properties

        private Point center = new Point(0,0);
        public System.Drawing.Point Center
        {
            get { return center; }
            set { center = value; }
        }
        private double m_radius = 10;
        public double Radius
        {
            get { return m_radius; }
            set { m_radius = value; }
        }

        private double up_layer = 10;
        public double Up_layer
        {
            get { return up_layer; }
            set { up_layer = value; }
        }
        private double down_layer = 20;
        public double Down_layer
        {
            get { return down_layer; }
            set { down_layer = value; }
        }

        private double height = 10;
        public double Height
        {
            get { return height; }
            set { height = value; }
        }

        private List<Point> points = new List<Point>();
        public List<Point> Points
        {
            get { return points; }
            set { points = value; }
        }
        #endregion
 

        private void Initlaize()
        {

            Point pA = new Point(0, 0);
            Point pB = new Point((int)(down_layer - up_layer) / 2, (int)height);
            Point pC = new Point((int)(down_layer + up_layer) / 2, (int)height);
            Point pD = new Point((int)down_layer, 0);
            points.Clear();
            points.Add(pA);
            points.Add(pB);
            points.Add(pC);
            points.Add(pD);
           
           
        }

       
        public double Area()
        {
            return (up_layer+down_layer)*height/2;
        }

        public bool Contains(Point pt)
        {
           return Contains(pt.X,pt.Y);
        }
        public bool Contains(double x,double y)
        {


            //int result = 0;
            bool result = false;

            Rectangle rect = new Rectangle((int)(down_layer - up_layer) / 2, 0, (int)up_layer, (int)height);
            if (rect.Contains((int)x, (int)y))
            {
                return true;
            } 

            int i = 0, j = points.Count-1;
            for (i = 0; i < points.Count; ++i)
            {
                Point pti = points[i];
                Point ptj = points[j];
                
                if((pti.Y < y && ptj.Y>=y 
                    || ptj.Y < y && pti.Y >=y)
                    &&(pti.X <=x || ptj.X <=x))
                {
                    if (pti.X + (y - pti.Y) / (ptj.Y - pti.Y) * (ptj.X - pti.X) < x)
                    {
                        result = !result;
                    }
                    //result = result == 0 ? 0 : 1;
                  
                }
                j = i;
            }

            //return result > 0;
            return result;

        } 
 
    }
}
