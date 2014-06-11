using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Random_Polygon.circle;
using System.Collections.ObjectModel;

namespace Random_Polygon.rectangle
{
    
    [Serializable]
    public class RectRationLayerCondition: INotifyPropertyChanged
    {
        private Condition m_condition = new Condition();
        public Random_Polygon.Condition Condition
        {
            get { return m_condition; }
            set { m_condition = value; SubscribePropertyChanged("Condition"); }
        }
        private RatioControl m_controlRatio = new RatioControl();
        public Random_Polygon.circle.RatioControl ControlRatio
        {
            get { return m_controlRatio; }
            set { m_controlRatio = value; SubscribePropertyChanged("ControlRatio"); }
        }


        public RectRationLayerCondition Clone()
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, this);
                objectStream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(objectStream) as RectRationLayerCondition;
            }
        }



        #region INotifyPropertyChanged Members
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        private void SubscribePropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
    [Serializable]
    public class RectRationLayerConditionList : INotifyPropertyChanged
    {

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


        private ObservableCollection<RectRationLayerCondition> m_RatioConditionList = new ObservableCollection<RectRationLayerCondition>();
        public ObservableCollection<RectRationLayerCondition> RatioConditionList
        {
            get { return m_RatioConditionList; }
            set { m_RatioConditionList = value; }
        }

        public void Add(RectRationLayerCondition condition)
        {
            this.RatioConditionList.Add(condition);
        }

        public void Remove(RectRationLayerCondition condition)
        {
            this.RatioConditionList.Remove(condition);
        }

        public double CalcTotalRatio()
        {
            return m_RatioConditionList.Sum(x => x.ControlRatio.TargetRatio); 
        }

        public RectRationLayerCondition getMiniRatioControl()
        {
            RectRationLayerCondition ratioConditon = (from ratio in m_RatioConditionList orderby ratio.ControlRatio.Diff descending select ratio).First();
            return ratioConditon;
        }

        public void ClearGeneraterInfo()
        {
            foreach (RectRationLayerCondition ratio in this.m_RatioConditionList)
            {
                ratio.ControlRatio.ClearGenteraterInfo();
            }
        }

        public void Clear()
        {
            this.RatioConditionList.Clear();
        }

        public void UpdateTotalCount()
        {
            foreach (RectRationLayerCondition ratio in this.m_RatioConditionList)
            {
                ++ratio.ControlRatio.TotalCount;
            }
        }

        public int IsTargetOutLimited(double current)
        {
            double total = current + this.m_RatioConditionList.Sum(x => x.ControlRatio.TargetRatio);
            if (total > 1.0)
                return 1;
            else if (total < 1.0)
                return -1;
            else
                return 0;

        }

        public RectRationLayerConditionList Clone()
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, this);
                objectStream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(objectStream) as RectRationLayerConditionList;
            }
        }


        #region INotifyPropertyChanged Members
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        private void SubscribePropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }

    public class RectRationConditionList : INotifyPropertyChanged
    {
        // 总边界
        private int m_BoundaryWidth = 500;
        public int BoundaryWidth
        {
            get { return m_BoundaryWidth; }
            set { m_BoundaryWidth = value; SubscribePropertyChanged("BoundaryWidth"); UpdateWidth(m_BoundaryWidth); }
        }
        private int m_BoundaryHeight = 500;
        public int BoundaryHeight
        {
            get { return m_BoundaryHeight; }
            set { m_BoundaryHeight = value; SubscribePropertyChanged("BoundaryHeight"); }
        }

        private ObservableCollection<RectRationLayerConditionList> m_LayerConditionList = new ObservableCollection<RectRationLayerConditionList>();
        public ObservableCollection<RectRationLayerConditionList> LayerConditionList
        {
            get { return m_LayerConditionList; }
            set { m_LayerConditionList = value; SubscribePropertyChanged("LayerConditionList"); }
        }

        public void clearInfo()
        {
            foreach (RectRationLayerConditionList conditionList in m_LayerConditionList)
            {
                conditionList.ClearGeneraterInfo();
            }
            this.CadPoint3dList.Clear();
        }

        public void clear()
        {
            foreach (RectRationLayerConditionList conditionList in m_LayerConditionList)
            {
                conditionList.Clear();
            }
           
        }

        #region INotifyPropertyChanged Members
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        private void SubscribePropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        public void Add(RectRationLayerConditionList rectRationLayerConditionList)
        {
            m_LayerConditionList.Add(rectRationLayerConditionList);
            UpdateHeight();
        }

        public void Clear()
        {
            m_LayerConditionList.Clear();
        }

        public void Remove(RectRationLayerConditionList item)
        {
            m_LayerConditionList.Remove(item);
            UpdateHeight();
            
        }

        public void UpdateHeight()
        {
            int y = 0;
            foreach (RectRationLayerConditionList conditionList in m_LayerConditionList)
            {
                conditionList.Y = y;
                y += conditionList.CHeight;
            }
        }

        public void UpdateWidth(int with)
        {
            
            foreach (RectRationLayerConditionList conditionList in m_LayerConditionList)
            {
                conditionList.CWidth = with;
            }
        }

        public void Add(List<System.Windows.Point> pts)
        {
            Points pt = new Points();
            pt.Add(pts);
            CadPoint3dList.Add(pt);
        }

        private List<Points> m_CadPoint3dList = new List<Points>();
        public List<Points> CadPoint3dList
        {
            get { return m_CadPoint3dList; }
            set { m_CadPoint3dList = value; }
        }
        
    }

}
