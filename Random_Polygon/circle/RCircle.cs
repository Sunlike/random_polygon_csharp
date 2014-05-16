using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Random_Polygon.circle
{
    public class RCircle
    {
        public RCircle()
        {
            center = new Point(0, 0);
            m_radius = 0;
        }
        public RCircle(Point pt,double radius)
        {
            center = pt;
            m_radius = radius;
        }
        public RCircle(int x, int y, double radius)
        {
            center = new Point(x, y) ;
            m_radius = radius;
        }

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

        public double Area()
        {
            return Math.PI * Math.Pow(m_radius, 2.0);
        }

        public bool Contains(Point pt)
        {
            double distance = Math.Pow(pt.X - center.X, 2.0) + Math.Pow(pt.Y - center.Y, 2.0);
            double R2 = Math.Pow(m_radius, 2.0);
            return distance < R2;
        }
        public bool Contains(double x,double y)
        {
            double distance = Math.Pow(x - center.X, 2.0) + Math.Pow(y - center.Y, 2.0);
            double R2 = Math.Pow(m_radius, 2.0);
            return distance < R2;
        }

        public bool Contains(RCircle circle)
        {
            double distance = Math.Pow(circle.Center.X - center.X, 2.0) + Math.Pow(circle.Center.Y - center.Y, 2.0);
            double R2 = Math.Pow(m_radius-circle.Radius, 2.0);
            return distance <= R2;
        }

        public bool IntersectsWith(RCircle circle)
        {
            double distance = Math.Pow(circle.Center.X - center.X, 2.0) + Math.Pow(circle.Center.Y - center.Y, 2.0);
            double R2E = Math.Pow(m_radius + circle.Radius, 2.0); 
            return distance < R2E;
        }
    }
}
