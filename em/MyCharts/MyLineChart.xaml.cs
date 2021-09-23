using em.Models;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace em.MyCharts
{
    /// <summary>
    /// Логика взаимодействия для DashboardLineChart.xaml
    /// </summary>
    public partial class MyLineChart : UserControl, INotifyPropertyChanged
    {
        public SeriesCollection SeriesCollection { get; set; }
        public ObservableCollection<string> Labels { get; set; }
        private List<DataChart> tableData;

        private string chartCaption;
        public string ChartCaption
        {
            get
            {
                return chartCaption;
            }
            set
            {
                chartCaption = value;
                OnPropertyChanged("ChartCaption");
            }
        }
        public Func<double, string> Formatter { get; set; }

        public MyLineChart(List<DataChart> tData, string caption = "")
        {
            SeriesCollection = new SeriesCollection();
            Labels = new ObservableCollection<string>();
            ChartCaption = chartCaption;
            tableData = tData;
            Formatter = val => val.ToString("N0");
            ChartFill();
            InitializeComponent();
            Header.Text = caption;
            DataContext = this;
        }
        public void ChartFill()
        {
            LineSeries ls1 = new LineSeries()
            {
                Title = "Отклонение",
                LineSmoothness = 0,
                StrokeThickness = 4,
                AreaLimit = 0,
                DataLabels = true,
                FontSize = 10,
                FontWeight = FontWeights.Normal,
                LabelPoint = point => string.Format("{0:N0}", point.Y),
                PointGeometrySize = 0,
                Values = new ChartValues<ObservableValue>()
            };
            foreach (var newY in tableData)
            {
                ls1.Values.Add(new ObservableValue(newY.YParam1));
                Labels.Add(newY.XParam);
            }
            SeriesCollection.Add(ls1);

        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}
