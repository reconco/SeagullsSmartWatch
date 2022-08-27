using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SeagullsSmartWatch
{
    public class NotifyPatternData
    {
        public int Time { get; set; } = 0;
        public string Message { get; set; } = "dsfdfdf";
        public string MessageColor { get; set; } = "#FFFFFFFF";
        public string SoundFile { get; set; } = "";
    }


    /// <summary>
    /// NotifyPatternDataGridControl.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public partial class NotifyPatternDataGridControl : UserControl
    {
        List<NotifyPatternData> notifyPatternDatas = new List<NotifyPatternData>();

        public List<NotifyPatternData> NotifyPatternDatas { get { return notifyPatternDatas; } }

        public DataGrid DataGrid { get { return patternDataGrid; } }

        public NotifyPatternDataGridControl()
        {
            InitializeComponent();

            patternDataGrid.ItemsSource = notifyPatternDatas;

            notifyPatternDatas.Add(new NotifyPatternData());
            notifyPatternDatas.Add(new NotifyPatternData());
        }

        public void AddPatternData(NotifyPatternData patternData)
        {
            NotifyPatternDatas.Add(patternData);
            patternDataGrid.Items.Refresh();
        }

        public void MovePatternData(int from, int to)
        {
            if (from < 0)
                return;
            else if (from >= NotifyPatternDatas.Count)
                return;


            NotifyPatternData target = NotifyPatternDatas[from];
            NotifyPatternDatas.RemoveAt(from);

            if (to < 0)
                return;
            //else if (to >= NotifyPatternDatas.Count)
            //    to = NotifyPatternDatas.Count - 1;

            NotifyPatternDatas.Insert(to, target);

            patternDataGrid.Items.Refresh();
        }

        public void RemoveSelectedPatternData()
        {
            int i = patternDataGrid.SelectedIndex;

            if (i < 0)
                return;
            else if (i >= NotifyPatternDatas.Count)
                return;

            NotifyPatternDatas.RemoveAt(i);

            patternDataGrid.Items.Refresh();
        }
    }
}
