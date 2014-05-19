using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Random_Polygon.circle;
using System.Drawing;
using Random_Polygon.laddershape;

namespace Random_Polygon
{
    public class LadderShape_PolygonBuilder
    {
        private LadderShapeContainer container = null;
        public LadderShape_PolygonBuilder(LadderShapeContainer container)
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
