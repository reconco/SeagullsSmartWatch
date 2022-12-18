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
using Dsafa.WpfColorPicker;

namespace SeagullsSmartWatch
{
    /// <summary>
    /// NotifyPatternDataGridControl.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public partial class NotifyPatternDataGridControl : UserControl
    {
        List<NotifyPatternData> notifyPatternDatas = new List<NotifyPatternData>();

        public List<NotifyPatternData> NotifyPatternDatas { get { return notifyPatternDatas; } }

        public DataGrid DataGrid { get { return patternDataGrid; } }

        public event EventHandler NotifyTextColorChanged;

        public NotifyPatternDataGridControl()
        {
            InitializeComponent();

            //patternDataGrid.ItemsSource = notifyPatternDatas;
        }

        public void SetPatternDatas(List<NotifyPatternData> _notifyPatternDatas)
        {
            this.notifyPatternDatas.Clear();
            foreach (NotifyPatternData data in _notifyPatternDatas)
                this.notifyPatternDatas.Add(data.Clone());

            patternDataGrid.ItemsSource = notifyPatternDatas;
        }

        public void AddPatternData(NotifyPatternData patternData)
        {
            if (notifyPatternDatas.Count >= 16)
                return;

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
                to = 0;
            else if (to >= NotifyPatternDatas.Count)
                to = NotifyPatternDatas.Count;

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

        private void textColor_Initialized(object sender, EventArgs e)
        {
        }

        private void textColorButton_Click(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;
            if (sender == null)
                return;

            Grid textColor = senderButton.FindName("textColor") as Grid;
            SolidColorBrush textSolidBrush = textColor.Background as SolidColorBrush;
            ColorPickerDialog colorPickerDialog = new ColorPickerDialog(textSolidBrush.Color);

            Color newColor = Colors.White;
            bool? res = colorPickerDialog.ShowDialog();
            if (res.HasValue && res.Value)
            {
                newColor = colorPickerDialog.Color;
                SolidColorBrush newBrush = new SolidColorBrush(newColor);
                textColor.Background = newBrush;

                var row = GetParent<DataGridRow>((Button)sender);
                int index = DataGrid.Items.IndexOf(row.Item);
                NotifyPatternDatas[index].NotifyTextColor = newBrush.Color.ToString();
                NotifyTextColorChanged(this, null);
            }
        }

        private TargetType GetParent<TargetType>(DependencyObject o)
    where TargetType : DependencyObject
        {
            if (o == null || o is TargetType) return (TargetType)o;
            return GetParent<TargetType>(VisualTreeHelper.GetParent(o));
        }

        private void textColor_Loaded(object sender, RoutedEventArgs e)
        {
            var row = GetParent<DataGridRow>((Grid)sender);
            int index = DataGrid.Items.IndexOf(row.Item);

            Color _textColor = (Color)ColorConverter.ConvertFromString(NotifyPatternDatas[index].NotifyTextColor);
            ((Grid)sender).Background = new SolidColorBrush(_textColor);
        }

    }
}
