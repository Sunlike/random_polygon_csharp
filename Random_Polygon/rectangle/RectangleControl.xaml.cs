using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using Random_Polygon.circle;
using System.IO;
using System.Xml.Serialization;

namespace Random_Polygon.rectangle
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class RectangleControl : UserControl, INotifyPropertyChanged
    {
        public RectangleControl()
        {
            InitializeComponent();
            this.DataContext = m_ConditionList;
            ui_layerCondition.DataContext = m_uiLayerCondition;
            ui_listview.DataContext = m_uiLayerCondition;
            ui_Condition.DataContext = m_uiCondition;
            //layer_condaition.DataContext = m_ConditionList;
            OpenRatio = false;
        }
        // 物料比率控制条件
        private RectRationLayerCondition m_uiCondition = new RectRationLayerCondition();
        public Random_Polygon.rectangle.RectRationLayerCondition Condition
        {
            get { return m_uiCondition; }
            set { m_uiCondition = value; OnPropertyChanged("Condition"); }
        }
        // 矩形分层控制条件
        private RectRationLayerConditionList m_uiLayerCondition = new RectRationLayerConditionList();
        public Random_Polygon.rectangle.RectRationLayerConditionList LayerCondition
        {
            get { return m_uiLayerCondition; }
            set { m_uiLayerCondition = value; OnPropertyChanged("LayerCondition"); }
        }
        // 
        private RectRationConditionList m_ConditionList = new RectRationConditionList();
        public Random_Polygon.rectangle.RectRationConditionList ConditionList
        {
            get { return m_ConditionList; }
            set { m_ConditionList = value; OnPropertyChanged("ConditionList"); }
        }
       
        private bool openRatio = false;
        public bool OpenRatio
        {
            get { return openRatio; }
            set
            {
                openRatio = value;
                if (!openRatio)
                {
                    Condition.ControlRatio.TargetRatio = 1.0;
                }
            }
        }

        private string costTime = "0";
        public string CostTime
        {
            get { return costTime; }
            set { costTime = value; OnPropertyChanged("CostTime"); }
        }

        private string logInfo = "";
        public string LogInfo
        {
            get { return logInfo; }
            set { logInfo = value; OnPropertyChanged("LogInfo"); }
        }
        private string coverRadio = "0";
        public string CoverRadio
        {
            get { return coverRadio; }
            set { coverRadio = value; OnPropertyChanged("CoverRadio"); }
        }

        private bool m_condation_enable = true;
        public bool Condation_Enable
        {
            get { return m_condation_enable; }
            set { m_condation_enable = value; OnPropertyChanged("Condation_Enable"); }
        }

        private bool canStopThread = false;

         

        private List<double> m_recover_radio = new List<double>();


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region 随机生成多边形的方法接口

        public static ExtendedPolygon randPolygonWithinBox(System.Drawing.Rectangle box, int edgeNum, int minAngle, int maxAngle)
        {
            ExtendedPolygonBuilder pBuilder = new ExtendedPolygonBuilder();
            return pBuilder.buildPolygon(box, edgeNum, minAngle, maxAngle);
        }
        public static ExtendedPolygon randPolygonWithinBox(RectangleContainer box, int edgeNum, int minRadius, int maxRadius, int minAngle, int maxAngle)
        {
            ExtendedPolygonBuilder pBuilder = new ExtendedPolygonBuilder(box);
            return pBuilder.buildPolygon(edgeNum, minRadius, maxRadius, minAngle, maxAngle);
        }
        #endregion

        #region 随机生成多边形的算法

        // TODO(Rye): 1.   Randomly run points, pick those that are not in any of the polygons in container
        //              2.    For each point, change it into random boxes with a small unit bound.
        //              2.1.  Then increase there bounds by a small random step independently,
        //              3.    Go through the small boxes in 2.1, throw out those intersect with exist polygons
        //              3.1   For each of the rest boxes, randomly run polygons within
        //              3.2   Put the generated polygons into container
        //              4     Repeat for reasonable times
        private void awesomelyFill(RectangleContainer container, RectRationLayerConditionList ratioLayerCondition)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < ratioLayerCondition.IterCount; ++i)
            {
                if (canStopThread)
                {
                    break;
                }
                RectRationLayerCondition ratioControl = ratioLayerCondition.getMiniRatioControl();
                System.Drawing.Rectangle box = new System.Drawing.Rectangle();
                box.X = rand.Next(container.Width - 1) + 1;
                box.Y = rand.Next(container.Height - 1) + 1;
                box.Width = ratioControl.Condition.MinRadius * 2;
                box.Height = box.Width;

                ExtendedPolygon polygon = null;
                bool bSuccess = false;

                for (int j = 0; j < ratioControl.Condition.MaxRadius * 2; j += ratioControl.Condition.ExpandStep)
                {
                    if (canStopThread)
                    {
                        break;
                    }
                    box.Width += ratioControl.Condition.ExpandStep;
                    box.Height += ratioControl.Condition.ExpandStep;

                    ExtendedPolygon tmpPolygon = randPolygonWithinBox(box, ratioControl.Condition.MaxEdges, ratioControl.Condition.MinAngle, ratioControl.Condition.MaxAngle);
                    bSuccess = container.canSafePut(tmpPolygon);
                    if (bSuccess)
                    {
                        polygon = tmpPolygon;
                    }
                    else
                    {
                        break;
                    }
                }

                if (polygon != null)
                {
                    container.put(polygon);
                    AddPolygon(polygon, ratioLayerCondition, container);
                    ++ratioControl.ControlRatio.Count;
                    ratioLayerCondition.UpdateTotalCount();
                    ratioControl = ratioLayerCondition.getMiniRatioControl();
                }
            }
        }

        public void genteraterRun(RectRationLayerConditionList ratioLayerCondition)
        {
            RectangleContainer container = new RectangleContainer(0, 0, ratioLayerCondition.CWidth, ratioLayerCondition.CHeight);

            while (true)
            {
                if (canStopThread)
                {
                    break;
                }
                bool bSuccess = false;
                RectRationLayerCondition ratioControl = ratioLayerCondition.getMiniRatioControl();
                ExtendedPolygon polygon = randPolygonWithinBox(container, ratioControl.Condition.MaxEdges, ratioControl.Condition.MinRadius, ratioControl.Condition.MaxRadius, ratioControl.Condition.MinAngle, ratioControl.Condition.MaxAngle);
                bSuccess = container.canSafePut(polygon);
                if (!bSuccess)
                {
                    for (int i = 0; i < 20; ++i)
                    {
                        Random rand = new Random(DateTime.Now.Millisecond);
                        int deltX = ratioControl.Condition.StepX + rand.Next(3);
                        int deltY = ratioControl.Condition.StepY;
                        polygon.translate(deltX, deltY);
                        bSuccess = container.canSafePut(polygon);
                        if (bSuccess)
                        {
                            container.put(polygon);
                            AddPolygon(polygon, ratioLayerCondition, container);
                            ++ratioControl.ControlRatio.Count;
                            LayerCondition.UpdateTotalCount();
                            break;
                        }
                    }
                }
                else
                {
                    container.put(polygon);
                    AddPolygon(polygon, ratioLayerCondition, container);
                    ++ratioControl.ControlRatio.Count;
                    ratioLayerCondition.UpdateTotalCount();

                }


                this.awesomelyFill(container, ratioLayerCondition);

                double radio = container.getCoverageRatio() * 100;
                if (radio > ratioLayerCondition.MinCoverRadio)
                {
                    break;
                }
            }

            m_recover_radio.Add(container.getCoverageRatio()*100); 
             
        }

        public void run()
        {  
            sw.Start();           
            foreach(RectRationLayerConditionList layerCondition in this.ConditionList.LayerConditionList)
            {
                try
                {
                    genteraterRun(layerCondition);
                }
                catch (System.Exception ex)
                {
                    Debug.Write("Wrong:" + ex.ToString());
                }
            }

            sw.Stop();
         

            int layer = 1;
            string strRadio = "";
            foreach(double radio in m_recover_radio)
            {
                strRadio += "" + (layer++) + "层:" + radio + "%  "; 
            }
            CoverRadio = strRadio;
            Condation_Enable = true;

        }
        #endregion

        private Stopwatch sw = null;


        private Brush getRandomBrush()
        {
            Random rand = new Random();

            byte r = Convert.ToByte(rand.Next(255));
            byte g = Convert.ToByte(rand.Next(255));
            byte b = Convert.ToByte(rand.Next(255));
            SolidColorBrush br = new SolidColorBrush();
            br.Color = Color.FromRgb(r, g, b);
            return br;
        }

        // 创建ui多边形
        private Polygon createPolygon(List<Point> pts)
        {
            Polygon polygon = new Polygon();
            foreach (Point pt in pts)
            {
                polygon.Points.Add(pt);
            }

            polygon.Stroke = getRandomBrush(); ;
            polygon.Fill = System.Windows.Media.Brushes.LightSeaGreen;
            polygon.StrokeThickness = 1;
            return polygon;
        }
        // 异步函数，保证其他线程能在UI 线程上进行操作
        private void AddPolygon(ExtendedPolygon polygon, RectRationLayerConditionList layerCondition, RectangleContainer container)
        {

            ExtendedPolygon polygonTemp = new ExtendedPolygon();
            foreach (Point pt in polygon.Points)
            {
                polygonTemp.addPoint(new System.Drawing.Point((int)pt.X,(int)pt.Y));
            }
            polygonTemp.translate(layerCondition.X, layerCondition.Y);

            this.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(
              () =>
              {
                  CostTime = sw.ElapsedMilliseconds.ToString();
                  LogInfo = container.LogInfo;
                  CoverRadio = (container.getCoverageRatio() * 100).ToString();

                  Polygon ui_polygon = createPolygon(polygonTemp.Points);
                  m_ConditionList.Add(polygonTemp.Points,polygonTemp.CircleCenter,polygonTemp.Radius);
                  bg_draw.Children.Add(ui_polygon);

              }));
        }

        private Thread m_thread = null;

        // 开启随机生成多面体
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Condation_Enable = false;
            canStopThread = false;

            rect_container.Width = m_ConditionList.BoundaryHeight;
            rect_container.Height = m_ConditionList.BoundaryHeight;
            rect_container.Visibility = Visibility.Visible;
            bg_draw.Children.Clear();
            m_recover_radio.Clear();
            LogInfo = "";
            CoverRadio = "0";

            this.m_ConditionList.clearInfo();
           
            bg_draw.Children.Add(rect_container);
            if (null == sw)
            {
                sw = new Stopwatch();
            }
            sw.Reset();

            // 工作线程，生成多边形
            m_thread = new Thread(new ThreadStart(run));
            m_thread.IsBackground = true;
            m_thread.Start();


        }
        // 终止线程
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            canStopThread = true;
            if (m_thread != null && m_thread.IsAlive)
            {
                m_thread.Abort(1000);
            }

            Condation_Enable = true;
            Debug.WriteLine("canStopThread = true");
        }

        private int getCurrentLayerHeight()
        {
            return this.m_ConditionList.LayerConditionList.Sum(x => x.CHeight);
        }
        // 添加分层控制
        private void AddContidation_Click(object sender, RoutedEventArgs e)
        {
            if (!openRatio)
            {
                Button_AddClick(null,null);               
            }

            double totalTargetRatio = m_uiLayerCondition.CalcTotalRatio();
            if (totalTargetRatio + this.m_uiCondition.ControlRatio.TargetRatio < 1.0)
            {
                MessageBox.Show("目标比率之和不足100%\n\r目标比率只和必须为100%", "警告");
                return;
            }
            this.ConditionList.Add(this.m_uiLayerCondition.Clone());
           
            this.LayerCondition.Clear();
            this.Condition = new RectRationLayerCondition();
            ui_Condition.DataContext = this.Condition;
        }

        // 清空分层控制参数
        private void ResetContidation_Click(object sender, RoutedEventArgs e)
        {
            this.LayerCondition.Clear();
            this.Condition = new RectRationLayerCondition();
            this.ConditionList.Clear();            
        }

        // 删除选中的分层
        private void DeleteContidation_Click(object sender, RoutedEventArgs e)
        {
            RectRationLayerConditionList item = this.layer_condaition.SelectedValue as RectRationLayerConditionList;
            if(item != null)
            {
                this.ConditionList.Remove(item);
            }
            
        }
        // 添加一种物料比率
        private void Button_AddClick(object sender, RoutedEventArgs e)
        {
                      
            double totalTargetRatio = m_uiLayerCondition.CalcTotalRatio();
            if (totalTargetRatio + this.m_uiCondition.ControlRatio.TargetRatio > 1.0)
            {
                 MessageBox.Show("目标比率之和已经超过100%", "警告");
                 return;
             }
             m_uiLayerCondition.RatioConditionList.Add(this.m_uiCondition.Clone());  
             
                    
        }
        // 删除选中的物料比率
        private void Button_DeleteSelectClick(object sender, RoutedEventArgs e)
        {
            RectRationLayerCondition item = this.ui_listview.SelectedValue as RectRationLayerCondition;
            if (item != null)
            {
                LayerCondition.Remove(item);
            }
        }
        // 清空物料比率条件 
        private void Button_ClearClick(object sender, RoutedEventArgs e)
        {
            LayerCondition.Clear();
        }

        private void layer_condaition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ui_listview.DataContext = this.layer_condaition.SelectedItem;
        }

        private void layer_condaition_MouseLeave(object sender, MouseEventArgs e)
        {
            ui_listview.DataContext = m_uiLayerCondition;
        }

        /// <summary>
        /// 保存生成的列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                m_ConditionList.CostTime = this.CostTime;
                m_ConditionList.FinalRatio = this.CoverRadio + "%";

                save_tips.Text = "正在保存中……";
                string path = AppDomain.CurrentDomain.BaseDirectory + "rectangle";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string filename = path + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xml";

                using (FileStream stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(RectRationConditionList));
                    serializer.Serialize(stream, this.m_ConditionList);
                }
                save_tips.Text = "保存成功";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("保存失败", "错误", MessageBoxButton.OK);
            }
        }

       
    }

}
