﻿using System;
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
        //public delegate void NotifyPatternEvent(object sender, EventArgs args);
        public event EventHandler NotifyPatternHasChanged;

        public NotifyPatternWindow()
        {
            InitializeComponent();

            UpdateCountText();
            preview.notifyPatternDatas = patternDataGridControl.NotifyPatternDatas;
            preview.UpdatePreview();
        }
        public NotifyPatternWindow(List<NotifyPatternData> _notifyPatternDatas)
        {
            InitializeComponent();

            patternDataGridControl.patternDataGrid.SelectedCellsChanged += PatternDataGrid_SelectedCellsChanged;
            patternDataGridControl.patternDataGrid.TargetUpdated += PatternDataGrid_TargetUpdated;
            patternDataGridControl.NotifyTextColorChanged += PatternDataGridControl_NotifyTextColorChanged;

            SetNotifyPatternDatas(_notifyPatternDatas);

            UpdateCountText();
            preview.notifyPatternDatas = patternDataGridControl.NotifyPatternDatas;
            preview.UpdatePreview();
        }

        private void PatternDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            int selectedIdx = patternDataGridControl.patternDataGrid.SelectedIndex;
            preview.UpdatePreview(selectedIdx);
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (patternDataGridControl.NotifyPatternDatas.Count >= 16)
                return;

            NotifyPatternData newNotifyPatternData = new NotifyPatternData();
            newNotifyPatternData.NotifyTextColor = MainWindow.setting.notifyTextColor;

            patternDataGridControl.AddPatternData(newNotifyPatternData);
            UpdateCountText();

            int selectedIdx = patternDataGridControl.patternDataGrid.SelectedIndex;
            preview.UpdatePreview(selectedIdx);
        }

        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            patternDataGridControl.RemoveSelectedPatternData();
            UpdateCountText();

            int selectedIdx = patternDataGridControl.patternDataGrid.SelectedIndex;
            preview.UpdatePreview(selectedIdx);
		}
		private void clearButton_Click(object sender, RoutedEventArgs e)
		{
			patternDataGridControl.ClearAllPatternData();
			UpdateCountText();

			int selectedIdx = patternDataGridControl.patternDataGrid.SelectedIndex;
			preview.UpdatePreview(selectedIdx);
		}

		private void helpButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (NotifyPatternHasChanged != null)
            {
                EventArgs args = new EventArgs();
                NotifyPatternHasChanged(this, args);
            }

            Close();
        }

        private void cancleButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            int i = patternDataGridControl.patternDataGrid.SelectedIndex;
            if (i < 0)
                return;

            patternDataGridControl.MovePatternData(i, i - 1);

            int selectedIdx = patternDataGridControl.patternDataGrid.SelectedIndex;
            preview.UpdatePreview(selectedIdx);
        }

        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            int i = patternDataGridControl.patternDataGrid.SelectedIndex;
            if (i < 0)
                return;

            patternDataGridControl.MovePatternData(i, i + 1);

            int selectedIdx = patternDataGridControl.patternDataGrid.SelectedIndex;
            preview.UpdatePreview(selectedIdx);
        }

        public void SetNotifyPatternDatas(List<NotifyPatternData> _notifyPatternDatas)
        {
            patternDataGridControl.SetPatternDatas(_notifyPatternDatas);
            UpdateCountText();
        }

        public List<NotifyPatternData> GetNotifyPatternDatas()
        {
            //깊은 복사를 한다
            return patternDataGridControl.NotifyPatternDatas;
        }


        public List<NotifyPatternData> CloneNotifyPatternDatas()
        {
            //깊은 복사를 한다
            List<NotifyPatternData> cloned = new List<NotifyPatternData>();

            foreach (NotifyPatternData data in patternDataGridControl.NotifyPatternDatas)
                cloned.Add(data.Clone());

            return cloned;
        }

        private void UpdateCountText()
        {
            countText.Text = patternDataGridControl.NotifyPatternDatas.Count + "/" + NotifyPatternData.MAX_COUNT.ToString();
        }
        private void PatternDataGridControl_NotifyTextColorChanged(object sender, EventArgs e)
        {
            int selectedIdx = patternDataGridControl.patternDataGrid.SelectedIndex;
            preview.UpdatePreview(selectedIdx);
        }

        private void PatternDataGrid_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            int selectedIdx = patternDataGridControl.patternDataGrid.SelectedIndex;
            preview.UpdatePreview(selectedIdx);
        }

    }
}
