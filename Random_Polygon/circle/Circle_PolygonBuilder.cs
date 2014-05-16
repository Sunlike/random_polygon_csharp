using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Random_Polygon.circle;
using System.Drawing;

namespace Random_Polygon
{
    public class Circle_PolygonBuilder
    {
        private CircleContainer container = null;
        public Circle_PolygonBuilder(CircleContainer container)
        {
            this.container = container;
        } 

        public ExtendedPolygon randPolygonWithCircle(int edgeNum, int minRadius, int maxRadius, double minAngle, double maxAngle)
        {
            RectangleContainer box = container.GetBoundBox(); 
            ExtendedPolygon polygon = ExtendedPolygonBuilder.randAnyPolygon(box, edgeNum, minRadius, maxRadius, minAngle, maxAngle);
            return polygon;  
        } 
    }
}
