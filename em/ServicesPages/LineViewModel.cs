using em.Helpers;
using em.MenuPages;
using em.Models;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace em.ServicesPages
{
    public class LineViewModel : INotifyPropertyChanged, IChart
    {
        public LineChart myChart;

        public SeriesCollection SeriesCollection { get; set; }
        public ObservableCollection<string> Labels { get; set; }

        private List<FullFields> tableData;
        private List<TableStruct> tablestruct;
        private ChartType dataType;
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

        public LineViewModel(ChartType cType, List<FullFields> tData, List<TableStruct> tsList, string chartCaption)
        {
            SeriesCollection = new SeriesCollection();
            Labels = new ObservableCollection<string>();
            ChartCaption = chartCaption;
            this.tableData = tData;
            this.tablestruct = tsList;
            this.dataType = cType;
            Formatter = val => val.ToString("N0");
            ChartFill();
        }

        public void ChartFill()
        {
            SeriesCollection.Clear();
            if (dataType == ChartType.UseER)
            {
                LineSeries ls1 = new LineSeries()
                {
                    Title = "План",
                    LineSmoothness = 0,
                    StrokeThickness = 2.5,
                    AreaLimit = 0,
                    DataLabels = false,
                    FontSize = 12,
                    FontWeight = FontWeights.Normal,
                    LabelPoint = point => point.Y != 0 ? string.Format("{0:N0}", point.Y) : "",
                    Values = new ChartValues<ObservableValue>()
                };
                foreach (var newY in tableData)
                {
                    ls1.Values.Add(new ObservableValue(newY.PlanCost));
                }
                SeriesCollection.Add(ls1);

                LineSeries ls2 = new LineSeries()
                {
                    Title = "Факт",
                    LineSmoothness = 0,
                    StrokeThickness = 2.5,
                    AreaLimit = 0,
                    Fill = Brushes.Transparent,
                    DataLabels = true,
                    FontSize = 12,
                    FontWeight = FontWeights.Normal,
                    LabelPoint = point => point.Y != 0 ? string.Format("{0:N0}", point.Y) : "",
                    Values = new ChartValues<ObservableValue>()
                };
                foreach (var newY in tableData)
                {
                    ls2.Values.Add(new ObservableValue(newY.FactCost));
                }
                SeriesCollection.Add(ls2);

                LineSeries ls3 = new LineSeries()
                {
                    Title = "Откл",
                    LineSmoothness = 0,
                    StrokeThickness = 2.5,
                    AreaLimit = 0,
                    DataLabels = true,
                    FontSize = 12,
                    FontWeight = FontWeights.Normal,
                    LabelPoint = point => point.Y != 0 ? string.Format("{0:N0}", point.Y) : "",
                    Values = new ChartValues<ObservableValue>()
                };
                foreach (var newY in tableData)
                {
                    ls3.Values.Add(new ObservableValue(newY.DiffCost));
                }
                SeriesCollection.Add(ls3);
            }
            else if (dataType == ChartType.CompareER)
            {
                LineSeries ls1 = new LineSeries()
                {
                    Title = "Отклонение",
                    LineSmoothness = 0,
                    StrokeThickness = 2.5,
                    AreaLimit = 0,
                    DataLabels = true,
                    FontSize = 12,
                    FontWeight = FontWeights.Normal,
                    LabelPoint = point => point.Y != 0 ? string.Format("{0:N0}", point.Y) : "",
                    Values = new ChartValues<ObservableValue>()
                };
                foreach (var newY in tableData)
                {
                    ls1.Values.Add(new ObservableValue(newY.dFactCost));
                }
                SeriesCollection.Add(ls1);

            }
            else if (dataType == ChartType.LossesER)
            {
                LineSeries ls1 = new LineSeries()
                {
                    Title = "Норматив",
                    LineSmoothness = 0,
                    StrokeThickness = 2.5,
                    AreaLimit = 0,
                    DataLabels = false,
                    FontSize = 12,
                    FontWeight = FontWeights.Normal,
                    LabelPoint = point => point.Y != 0 ? string.Format("{0:N0}", point.Y) : "",
                    Values = new ChartValues<ObservableValue>()
                };
                foreach (var newY in tableData)
                {
                    ls1.Values.Add(new ObservableValue(newY.NormLossCost));
                }
                SeriesCollection.Add(ls1);

                LineSeries ls2 = new LineSeries()
                {
                    Title = "Факт",
                    LineSmoothness = 0,
                    StrokeThickness = 2.5,
                    AreaLimit = 0,
                    Fill = Brushes.Transparent,
                    DataLabels = true,
                    FontSize = 12,
                    FontWeight = FontWeights.Normal,
                    LabelPoint = point => point.Y != 0 ? string.Format("{0:N0}", point.Y) : "",
                    Values = new ChartValues<ObservableValue>()
                };
                foreach (var newY in tableData)
                {
                    ls2.Values.Add(new ObservableValue(newY.FactLossCost));
                }
                SeriesCollection.Add(ls2);

                LineSeries ls3 = new LineSeries()
                {
                    Title = "Откл",
                    LineSmoothness = 0,
                    StrokeThickness = 2.5,
                    AreaLimit = 0,
                    DataLabels = true,
                    FontSize = 12,
                    FontWeight = FontWeights.Normal,
                    LabelPoint = point => point.Y != 0 ? string.Format("{0:N0}", point.Y) : "",
                    Values = new ChartValues<ObservableValue>()
                };
                foreach (var newY in tableData)
                {
                    ls3.Values.Add(new ObservableValue(newY.DiffLossCost));
                }
                SeriesCollection.Add(ls3);
            }
            Labels.Clear();
            foreach (var newX in tableData)
            {
                Labels.Add(newX.PeriodStr);
            }

        }

        private MyRelayCommand tableWindowShow_Command;
        public MyRelayCommand TableWindowShow_Command
        {
            get
            {
                return tableWindowShow_Command ??
                    (tableWindowShow_Command = new MyRelayCommand(obj =>
                    {
                        TableWindow twind = new TableWindow(tableData, tablestruct, chartCaption);
                        twind.ShowDialog();
                    }));
            }
        }

        private MyRelayCommand screenSave_Command;
        public MyRelayCommand ScreenSave_Command
        {
            get
            {
                return screenSave_Command ??
                    (screenSave_Command = new MyRelayCommand(obj =>
                    {
                        ScreenSave();
                    }));
            }
        }

        private void ScreenSave()
        {
            string destinationPath = null;
            SaveFileDialog dialog = new SaveFileDialog();
            string filename = "_Screen";

            if (dialog.ShowDialog() == true)
                destinationPath = dialog.FileName;
            else return;
            using (FileStream stream = new FileStream(string.Format("{0}.png", destinationPath + filename), FileMode.Create))
            {
                RenderTargetBitmap bmp = new RenderTargetBitmap(1410, 410, 96, 96, PixelFormats.Pbgra32);

                bmp.Render(myChart);
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Interlace = PngInterlaceOption.On;
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                encoder.Save(stream);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}
