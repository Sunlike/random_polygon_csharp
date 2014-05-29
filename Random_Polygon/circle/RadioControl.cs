using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Random_Polygon.circle
{
    [Serializable]
    public class RatioControl:INotifyPropertyChanged
    {

        private int key = 3;
        public int Key
        {
            get { return key; }
            set { key = value; }
        }
        private double m_TargetRatio = 0.0;
        public double TargetRatio
        {
            get { return m_TargetRatio; }
            set 
            { 
                m_TargetRatio = value; 


                  


                caldiff();
                SubscribePropertyChanged("TargetRatio");
            }
        }
        private double m_RealRatio = 0.0;
        public double RealRatio
        {
            get { return m_RealRatio; }
            set { m_RealRatio = value; caldiff();
            SubscribePropertyChanged("RealRatio");
            }
        }
        public RatioControl(int key)
        {
            this.key = key;
        }
        public RatioControl(int key, double target)
        {
            this.key = key;
            TargetRatio = target;
            Diff = TargetRatio - RealRatio;
        }

        private int m_Count = 0;
        public int Count
        {
            get { return m_Count; }
            set { m_Count = value; calreal();
            SubscribePropertyChanged("Count");
            }
        }
        public double Diff = 0;

        private int m_totalCount = 0;
        public int TotalCount
        {
            get { return m_totalCount; }
            set
            {
                m_totalCount = value;
                calreal();
                SubscribePropertyChanged("TotalCount");
            }
        }
        private void caldiff()
        {
            Diff = this.TargetRatio - RealRatio;
        }

        private void calreal()
        {
            if (m_totalCount == 0) RealRatio = 0;
            else RealRatio = Count * 1.0 / TotalCount;
            caldiff();
        }


        public override string ToString()
        {
            return "Target:" + TargetRatio.ToString() + "\n\rReal:" + RealRatio + "\n\rCount:" + Count + "/ TotalCount:" + TotalCount.ToString() + "\n\r";
        }

        public void ClearGenteraterInfo()
        {
            TotalCount = 0;
            Count = 0;
            RealRatio = 0;
        }

      

        #region INotifyPropertyChanged Members
        [field: NonSerialized]  
        public event PropertyChangedEventHandler PropertyChanged;
        private void SubscribePropertyChanged(string propertyName)
        {
            if(null != PropertyChanged)
            {
                this.PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
    [Serializable]
    public class RatioControlList
    {
        private ObservableCollection<RatioControl> m_RatioList = new ObservableCollection<RatioControl>();
        public ObservableCollection<RatioControl> RatioList
        {
            get { return m_RatioList; }
            set { m_RatioList = value; }
        }

        public int getMaxKey()
        {
            if (m_RatioList.Count <= 0)
            {
                return 2;
            }
            return m_RatioList.Max(x => x.Key);
        }

        public double calculateTotalTargetRatio()
        {
            return m_RatioList.Sum(val => val.TargetRatio);

        }

        public void UpdateRatio(int key, double targetRatio)
        {
            foreach (RatioControl ratio in this.m_RatioList)
            {
                if (key == ratio.Key)
                {
                    ratio.TargetRatio = targetRatio;
                }
            }
        }

        public RatioControl getMinRatio()
        {
            RatioControl ratioConditon = (from ratio in m_RatioList orderby ratio.Diff descending select ratio).First();
            return ratioConditon;
        }

        public void UpdateTotalCount( int count)
        {
            foreach (RatioControl ratio in this.m_RatioList)
            {
                ratio.TotalCount = count;
            }
        }
        public void UpdateTotalCount()
        {
            foreach (RatioControl ratio in this.m_RatioList)
            {
                ratio.TotalCount++;
            }
        }

        public void UpdateCount(int key, int count)
        {
            foreach (RatioControl ratio in this.m_RatioList)
            {
                if (key == ratio.Key)
                {
                    ratio.Count = count;
                }
            }
        }

        public void UpdateCount(int key)
        {
            foreach (RatioControl ratio in this.m_RatioList)
            {
                if (key == ratio.Key)
                {
                    ratio.Count++;
                }
            }
        }

        public void UpdateAllTargetRatio()
        {
            double avgRatio = 1.0 / (this.m_RatioList.Count + 1);
            foreach (RatioControl ratio in this.m_RatioList)
            {
                ratio.TargetRatio = avgRatio;
            }
        }

        public void Initialize(int maxKey)
        {
            int count = maxKey - 2;
            double avgRatio = 1.0 / count;
            m_RatioList.Clear();
            for (int i = 0; i < count; ++i)
            {
                m_RatioList.Add(new RatioControl(i + 3, avgRatio));
            }

        }


        public void ClearGeneraterInfo()
        {
            foreach (RatioControl ratio in this.m_RatioList)
            {
                ratio.ClearGenteraterInfo();
            }
        }
    }
}
