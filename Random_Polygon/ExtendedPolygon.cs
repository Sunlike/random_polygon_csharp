using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
namespace Random_Polygon
{
    public class ExtendedPolygon
    {
        private Polygon m_Polygon = null;
        private List<Point> norms = null;
        private double m_area = 0.0;
        private double m_radius = 0.0;
        private Point m_circleCenter;
        private int m_quadrant = -1;

        public ExtendedPolygon()
        {
            this.m_Polygon = new Polygon();
            m_Polygon.Stroke = System.Windows.Media.Brushes.Black;
            m_Polygon.Fill = System.Windows.Media.Brushes.LightSeaGreen;
            m_Polygon.StrokeThickness = 1;
        }
        
        public int Quadrant
        {
            get { return m_quadrant; }
            set { this.m_quadrant = value;}
        }

        public Point CircleCenter
        {
            get { return m_circleCenter; }
            set { m_circleCenter = value; }
        }

        public double Radius
        {
            get { return m_radius; }
            set { m_radius = value; }
        }

        public PointCollection Points
        {
            get { return m_Polygon == null ? null : m_Polygon.Points; }
        }

        public Polygon MPolygon
        {

            get { return m_Polygon; }
        }
        private void getMinMaxProjs(Point axis, ref double minProj, ref double maxProj)
        {
            if (m_Polygon == null)
            {
                return;
            }

            List<Point>  points = this.m_Polygon.Points.ToList<Point>();

            minProj = maxProj = points[0].X * axis.X + points[0].Y * axis.Y;
            for (int i = 0; i < points.Count; ++i)
            {
                double proj = points[i].X * axis.X + points[i].Y * axis.Y;
                if (minProj > proj)
                {
                    minProj = proj;
                }

                if (maxProj < proj)
                {
                    maxProj = proj;
                }
            }

        }

        public void translate(int deltX, int deltY)
        {
            //m_Polygon.TranslatePoint(new Point(deltX, deltY), null);
            PointCollection pc = new PointCollection();
            foreach (Point pt in m_Polygon.Points)
            { 
                pc.Add(new Point(pt.X + deltX,pt.Y + deltY));
            }
            m_Polygon.Points = pc;

            this.m_circleCenter.X += deltX;
            this.m_circleCenter.Y += deltY;
        }

        // true 相交和包含
        private bool intersectsWithBox(ExtendedPolygon polygon)
        {
            // 计算圆心之间的距离
            double distance = Math.Sqrt(Math.Pow(this.m_circleCenter.X - polygon.CircleCenter.X, 2.0) + Math.Pow(this.m_circleCenter.Y - polygon.CircleCenter.Y, 2.0));
            // 半径差
            double diffRadius = Math.Abs(this.m_radius - polygon.Radius);
            //半径和
            double sumRadius = this.m_radius + polygon.Radius;

            return distance < sumRadius; 
        }

        public bool intersects(ExtendedPolygon polygon)
        {
            if (false == intersectsWithBox(polygon))
            {
                return false;
            }

            // check each of this prolygon's norms
            int size = this.getNorms().Count;
            for (int i = 0; i < size; ++i)
            {
                double minProj1 = 0.0, maxProj1 = 0.0, minProj2 = 0.0, maxProj2 = 0.0;
                this.getMinMaxProjs(this.getNorms()[i],ref minProj1,ref maxProj1);
                polygon.getMinMaxProjs(this.getNorms()[i], ref minProj2, ref maxProj2);
                if (maxProj1 < minProj2 || maxProj2 < minProj1)
                {
                    return false;
                }

            }

            // check each of other polygon's norms
            size = polygon.getNorms().Count; 
            for (int i = 0; i < size; ++i)
            {
                double minProj1 = 0.0, maxProj1 = 0.0, minProj2 = 0.0, maxProj2 = 0.0;
                this.getMinMaxProjs(polygon.getNorms()[i], ref minProj1, ref maxProj1);
                polygon.getMinMaxProjs(polygon.getNorms()[i], ref minProj2, ref maxProj2);
                if (maxProj1 < minProj2 || maxProj2 < minProj1)
                {
                    return false;
                }

            }

            return true;
        }

        public List<Point> getNorms()
        {
            if (this.norms != null)
            {
                return norms;
            }

            this.norms = new List<Point>();
            int i = 0;
            for (; i < this.m_Polygon.Points.Count - 1; ++i)
            {
                Point pt = this.m_Polygon.Points[i];
                Point pt1 = this.m_Polygon.Points[i + 1];
                Point norm = new Point(pt.Y - pt1.Y, pt1.X - pt.X);
                this.norms.Add(norm);
            }

            {
                Point pt = this.m_Polygon.Points[i];
                Point pt1 = this.m_Polygon.Points[0];
                Point norm = new Point(pt.Y - pt1.Y, pt1.X - pt.X);
                this.norms.Add(norm);
            }

            return this.norms;
        }

        private double calculateArea()
        {
            double area1 = 0, area2 = 0;
            int size =this.m_Polygon.Points.Count;
            for (int i = 0; i < size -1; ++i)
            {
                area1 += this.m_Polygon.Points[i].X * this.m_Polygon.Points[i + 1].Y;
            }
            area1 += this.m_Polygon.Points[size - 1].X * this.m_Polygon.Points[0].Y;


            for (int i = 0; i < size - 1; ++i)
            {
                area2 += this.m_Polygon.Points[i].Y * this.m_Polygon.Points[i + 1].X;
            }
            area2 += this.m_Polygon.Points[size - 1].Y * this.m_Polygon.Points[0].X;

            this.m_area = Math.Abs(area1 - area2) / 2;

            return this.m_area;
        }

        public double getArea()
        {
            if (m_area > 0.0)
            {
                return m_area;
            }

            m_area = calculateArea();
            return m_area;
        }

        public void addPoint(System.Drawing.Point pt)
        {
            m_Polygon.Points.Add(new Point(pt.X,pt.Y));
        }



    }
}
