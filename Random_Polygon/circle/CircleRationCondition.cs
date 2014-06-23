using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Random_Polygon.circle
{
    [Serializable]
    public class CircleRatioCondition : INotifyPropertyChanged
    {

        private Circle_Condition m_condition = new Circle_Condition();
        public Random_Polygon.Circle_Condition Condition
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

        public CircleRatioCondition Clone()
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, this);
                objectStream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(objectStream) as CircleRatioCondition;
            }  
        }

        public override string ToString()
        {
            return m_condition.ToString() + m_controlRatio.ToString();
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

    public class CircleRatioConditionList : INotifyPropertyChanged
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


        private int m_radius = 250;
        public int Radius
        {
            get { return m_radius; }
            set { m_radius = value; SubscribePropertyChanged("Radius"); }
        }

        private string m_finalRatio = "";
        public string FinalRatio
        {
            get { return m_finalRatio; }
            set { m_finalRatio = value; SubscribePropertyChanged("FinalRatio"); }
        }

        private string m_costTime = "";
        public string CostTime
        {
            get { return m_costTime; }
            set { m_costTime = value; SubscribePropertyChanged("CostTime"); }
        }

        private ObservableCollection<CircleRatioCondition> m_RatioConditionList= new ObservableCollection<CircleRatioCondition>();
        public ObservableCollection<CircleRatioCondition> RatioConditionList
        {
            get { return m_RatioConditionList; }
            set { m_RatioConditionList = value; }
        }

        public CircleRatioCondition getMiniRatioControl()
        {
            CircleRatioCondition ratioConditon = (from ratio in m_RatioConditionList orderby ratio.ControlRatio.Diff descending select ratio).First();
            return ratioConditon;
        }

        public void ClearGeneraterInfo()
        {
            foreach (CircleRatioCondition ratio in this.m_RatioConditionList)
            {
                ratio.ControlRatio.ClearGenteraterInfo();
            }
            CadPoint3dList.Clear();
        }

        public void UpdateTotalCount()
        {
            foreach (CircleRatioCondition ratio in this.m_RatioConditionList)
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

        public override string ToString()
        {
            string format = "圆形信息:\n\r" +
                            "半径:{0}\n\r" +
                            "最总填充率:{1}\n\r" +
                            "花费时间:{2}ms\n\r";
            string result = string.Format(format, this.m_radius, this.m_finalRatio, this.m_costTime) + "\n\r";
            format = "第{0}种物料信息:\n\r{1}\n\r";
            for (int i = 0; i < m_RatioConditionList.Count;++i)
            {
                result += string.Format(format, i + 1, m_RatioConditionList[i].ToString());
            }

            return result;
        }


    }
}
