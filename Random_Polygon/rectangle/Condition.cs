using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Random_Polygon.circle;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Random_Polygon
{
    [Serializable]
    public class Condition : INotifyPropertyChanged,ICloneable  
    {
        public Condition()
        {
            MaxEdges = 5;
        }
       
        
        private int m_MaxEdges = 5;
        public int MaxEdges
        {
            get { return m_MaxEdges; }
            set { m_MaxEdges = value;
            SubscribePropertyChanged("MaxEdges");
            this.ratioControlList.Initialize(m_MaxEdges);
            }
        }
        private int m_MinRadius = 5;

        public int MinRadius
        {
            get { return m_MinRadius; }
            set { m_MinRadius = value; SubscribePropertyChanged("MinRadius"); }
        }
        private int m_MaxRadius = 60;
        public int MaxRadius
        {
            get { return m_MaxRadius; }
            set { m_MaxRadius = value; SubscribePropertyChanged("MaxRadius"); }
        }
        private int m_MinAngle = 10;
        public int MinAngle
        {
            get { return m_MinAngle; }
            set { m_MinAngle = value; SubscribePropertyChanged("MinAngle"); }
        }
        private int m_MaxAngle = 179;
        public int MaxAngle
        {
            get { return m_MaxAngle; }
            set { m_MaxAngle = value; SubscribePropertyChanged("MaxAngle"); }
        }
        private int m_MinCoverRadio = 50;
        public int MinCoverRadio
        {
            get { return m_MinCoverRadio; }
            set { m_MinCoverRadio = value; SubscribePropertyChanged("MinCoverRadio"); }
        }
        private int m_IterCount = 25000;
        public int IterCount
        {
            get { return m_IterCount; }
            set { m_IterCount = value; SubscribePropertyChanged("IterCount"); }
        }

        private int x = 0;
        public int X
        {
            get { return x; }
            set { x = value; SubscribePropertyChanged("X"); }
        }
        private int y = 0;
        public int Y
        {
            get { return y; }
            set { y = value; SubscribePropertyChanged("Y"); }
        }

        private int m_Height = 250;
        public int CHeight
        {
            get { return m_Height; }
            set { m_Height = value; SubscribePropertyChanged("CHeight"); }
        }
        private int m_Width = 500;
        public int CWidth
        {

            get { return m_Width; }
            set { m_Width = value; SubscribePropertyChanged("CWidth"); }
        }

        private int m_stepX = -1;
        public int StepX
        {
            get { return m_stepX; }
            set { m_stepX = value; }
        }
        private int m_stepY = 1;
        public int StepY
        {
            get { return m_stepY; }
            set { m_stepY = value; }
        }

        private int m_expandStep = 1;
        public int ExpandStep
        {
            get { return m_expandStep; }
            set { m_expandStep = value; }
        }

        // 总边界
        private int m_BoundaryWidth=500;
        public int BoundaryWidth
        {
            get { return m_BoundaryWidth; }
            set { m_BoundaryWidth = value; SubscribePropertyChanged("BoundaryWidth"); }
        }
        private int m_BoundaryHeight = 500;
        public int BoundaryHeight
        {
            get { return m_BoundaryHeight; }
            set { m_BoundaryHeight = value; SubscribePropertyChanged("BoundaryHeight"); }
        }


        private RatioControlList ratioControlList = new RatioControlList();

       
        public Random_Polygon.circle.RatioControlList RatioControlList
        {
            get { return ratioControlList; }
            set { ratioControlList = value; SubscribePropertyChanged("RatioControlList"); }
        }


        #region INotifyPropertyChanged Members
        [field: NonSerialized]  
        public event PropertyChangedEventHandler PropertyChanged;
      
        private void SubscribePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

       
        public Condition DeepClone()
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, this);
                objectStream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(objectStream) as Condition;
            }  
        }



        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}
