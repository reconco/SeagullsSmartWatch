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
using System.Windows.Shapes;

namespace SeagullsSmartWatch
{
    /// <summary>
    /// NotifyPatternWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NotifyPatternWindow : Window
    {
        public NotifyPatternWindow()
        {
            InitializeComponent();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            patternDataGridControl.AddPatternData(new NotifyPatternData());
        }

        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            patternDataGridControl.RemoveSelectedPatternData();
        }

        private void helpButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
