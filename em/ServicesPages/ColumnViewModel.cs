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
    public class ColumnViewModel : INotifyPropertyChanged, IChart
    {
        public ColumnChart myChart;

        public SeriesCollection SeriesCollection { get; set; }
        public ObservableCollection<string> Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        private List<FullFields> tableData;
        private List<TableStruct> tablestruct;
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

        private int sliderValue;
        public int SliderValue
        {
            get
            {
                return sliderValue;
            }
            set
            {
                sliderValue = value;
                SliderValueTxt = value.ToString() + "%";
                ChartFill();
                OnPropertyChanged("SliderValue");
            }
        }
        private string sliderValueTxt;
        public string SliderValueTxt
        {
            get
            {
                return sliderValueTxt;
            }
            set
            {
                sliderValueTxt = value;
                OnPropertyChanged("SliderValueTxt");
            }
        }

        private ChartType dataType;

        public ColumnViewModel(ChartType cType, List<FullFields> tData, List<TableStruct> tsList, string chartCaption, int sliderVal = 2)
        {
            SeriesCollection = new();
            Labels = new();
            dataType = cType;
            tableData = tData;
            tablestruct = tsList;
            ChartCaption = chartCaption;
            SliderValue = sliderVal;
        }

        private void ChartFill()
        {
            SeriesCollection.Clear();
            double total;
            if (dataType == ChartType.UseER)
            {
                total = tableData.Sum(n => Math.Abs(n.DiffCost));
                var qry1 = from o in tableData
                           where Math.Abs(o.DiffCost) >= Math.Abs(total * SliderValue / 100)
                           orderby o.DiffCost descending
                           select new
                           {
                               o.DiffCost,
                               o.IdER,
                               o.ERName
                           };
                var qry2 = from o in tableData
                           where Math.Abs(o.DiffCost) < Math.Abs(total * SliderValue / 100)
                           select new
                           {
                               o.DiffCost,
                               o.IdER,
                               o.ERName
                           };
                double total1 = 0;
                total1 = qry2.Count() > 0 ? qry2.Sum(n => n.DiffCost) : 0;
                //if (tableData.Count > 1)
                //{
                //    total1 = qry2.Sum(n => n.DiffCost);
                //}
                //double total1 = qry2.Sum(n => n.YParam1);

                ColumnSeries ps = new ColumnSeries
                {
                    DataLabels = true,
                    LabelPoint = point => string.Format("{0:N0}", point.Y),
                    FontSize = 11,
                    FontWeight = FontWeights.Normal,
                    Values = new ChartValues<ObservableValue>()
                };

                foreach (var newY in qry1.ToList())
                {
                    ps.Values.Add(new ObservableValue(newY.DiffCost));
                }
                if (total1 != 0)
                    ps.Values.Add(new ObservableValue(total1));
                SeriesCollection.Add(ps);

                Labels.Clear();
                foreach (var newX in qry1.ToList())
                {
                    Labels.Add(newX.IdER + "_" + newX.ERName);
                };
                if (total1 != 0) Labels.Add("прочие");
            }
            if (dataType == ChartType.UsePR)
            {
                total = tableData.Sum(n => Math.Abs(n.DiffCost));
                var qry1 = from o in tableData
                           where Math.Abs(o.DiffCost) >= Math.Abs(total * SliderValue / 100)
                           orderby o.DiffCost descending
                           select new
                           {
                               o.DiffCost,
                               o.IdProduct,
                               o.ProductName
                           };
                var qry2 = from o in tableData
                           where Math.Abs(o.DiffCost) < Math.Abs(total * SliderValue / 100)
                           select new
                           {
                               o.DiffCost,
                               o.IdProduct,
                               o.ProductName
                           };
                double total1 = 0;
                total1 = qry2.Count() > 0 ? qry2.Sum(n => n.DiffCost) : 0;
                //if (tableData.Count > 1)
                //{
                //    total1 = qry2.Sum(n => n.DiffCost);
                //}
                //double total1 = qry2.Sum(n => n.YParam1);

                ColumnSeries ps = new ColumnSeries
                {
                    DataLabels = true,
                    LabelPoint = point => string.Format("{0:N0}", point.Y),
                    FontSize = 11,
                    FontWeight = FontWeights.Normal,
                    Values = new ChartValues<ObservableValue>()
                };

                foreach (var newY in qry1.ToList())
                {
                    ps.Values.Add(new ObservableValue(newY.DiffCost));
                }
                if (total1 != 0)
                    ps.Values.Add(new ObservableValue(total1));
                SeriesCollection.Add(ps);

                Labels.Clear();
                foreach (var newX in qry1.ToList())
                {
                    Labels.Add(newX.IdProduct + "_" + newX.ProductName);
                };
                if (total1 != 0) Labels.Add("прочие");
            }

            else if (dataType == ChartType.UseCC)
            {
                total = tableData.Sum(n => Math.Abs(n.DiffCost));
                var qry1 = from o in tableData
                           where Math.Abs(o.DiffCost) >= Math.Abs(total * SliderValue / 100)
                           orderby o.DiffCost descending
                           select new
                           {
                               o.DiffCost,
                               o.IdCC
                           };
                var qry2 = from o in tableData
                           where Math.Abs(o.DiffCost) < Math.Abs(total * SliderValue / 100)
                           select new
                           {
                               o.DiffCost,
                               o.IdCC
                           };
                double total1 = 0;
                if (tableData.Count > 1)
                {
                    total1 = qry2.Sum(n => n.DiffCost);
                }
                //double total1 = qry2.Sum(n => n.YParam1);

                ColumnSeries ps = new ColumnSeries
                {
                    DataLabels = true,
                    LabelPoint = point => string.Format("{0:N0}", point.Y),
                    FontSize = 11,
                    FontWeight = FontWeights.Normal,
                    Values = new ChartValues<ObservableValue>()
                };

                foreach (var newY in qry1.ToList())
                {
                    ps.Values.Add(new ObservableValue(newY.DiffCost));
                }
                if (total1 != 0)
                    ps.Values.Add(new ObservableValue(total1));
                SeriesCollection.Add(ps);

                Labels.Clear();
                foreach (var newX in qry1.ToList())
                {
                    Labels.Add("ЦЗ-" + newX.IdCC);
                };
                if (total1 != 0) Labels.Add("прочие");
            }
            else if (dataType == ChartType.CompareER)
            {
                total = tableData.Sum(n => Math.Abs(n.dFactCost));

                var qry1 = from o in tableData
                           where Math.Abs(o.dFactCost) >= Math.Abs(total * SliderValue / 100)
                           orderby o.dFactCost descending
                           select new
                           {
                               o.dFactCost,
                               o.IdER,
                               o.ERName
                           };
                var qry2 = from o in tableData
                           where Math.Abs(o.dFactCost) < Math.Abs(total * SliderValue / 100)
                           select new
                           {
                               o.dFactCost,
                               o.IdER,
                               o.ERName
                           };
                double total1 = 0;
                if (tableData.Count > 1)
                {
                    total1 = qry2.Sum(n => n.dFactCost);
                }
                //double total1 = qry2.Sum(n => n.YParam1);

                ColumnSeries ps = new ColumnSeries
                {
                    DataLabels = true,
                    LabelPoint = point => string.Format("{0:N0}", point.Y),
                    FontSize = 11,
                    FontWeight = FontWeights.Normal,
                    Values = new ChartValues<ObservableValue>()
                };

                foreach (var newY in qry1.ToList())
                {
                    ps.Values.Add(new ObservableValue(newY.dFactCost));
                }
                //if (total1 != 0) 
                ps.Values.Add(new ObservableValue(total1));
                SeriesCollection.Add(ps);

                Labels.Clear();
                foreach (var newX in qry1.ToList())
                {
                    Labels.Add(newX.IdER + "_" + newX.ERName);
                };
                if (total1 != 0) Labels.Add("прочие");
            }
            else if (dataType == ChartType.CompareCC)
            {
                total = tableData.Sum(n => Math.Abs(n.dFactCost));
                var qry1 = from o in tableData
                           where Math.Abs(o.dFactCost) >= Math.Abs(total * SliderValue / 100)
                           orderby o.dFactCost descending
                           select new
                           {
                               o.dFactCost,
                               o.IdCC
                           };
                var qry2 = from o in tableData
                           where Math.Abs(o.dFactCost) < Math.Abs(total * SliderValue / 100)
                           select new
                           {
                               o.dFactCost,
                               o.IdCC
                           };
                double total1 = 0;
                if (tableData.Count > 1)
                {
                    total1 = qry2.Sum(n => n.dFactCost);
                }
                //double total1 = qry2.Sum(n => n.YParam1);

                ColumnSeries ps = new ColumnSeries
                {
                    DataLabels = true,
                    LabelPoint = point => string.Format("{0:N0}", point.Y),
                    FontSize = 11,
                    FontWeight = FontWeights.Normal,
                    Values = new ChartValues<ObservableValue>()
                };

                foreach (var newY in qry1.ToList())
                {
                    ps.Values.Add(new ObservableValue(newY.dFactCost));
                }
                if (total1 != 0)
                    ps.Values.Add(new ObservableValue(total1));
                SeriesCollection.Add(ps);

                Labels.Clear();
                foreach (var newX in qry1.ToList())
                {
                    Labels.Add("ЦЗ-" + newX.IdCC);
                };
                if (total1 != 0) Labels.Add("прочие");
            }
            else if (dataType == ChartType.LossesER)
            {
                total = tableData.Sum(n => Math.Abs(n.DiffLossCost));

                var qry1 = from o in tableData
                           where Math.Abs(o.DiffLossCost) >= Math.Abs(total * SliderValue / 100)
                           orderby o.DiffLossCost descending
                           select new
                           {
                               o.DiffLossCost,
                               o.IdER,
                               o.ERName
                           };
                var qry2 = from o in tableData
                           where Math.Abs(o.DiffLossCost) < Math.Abs(total * SliderValue / 100)
                           select new
                           {
                               o.DiffLossCost,
                               o.IdER,
                               o.ERName
                           };
                double total1 = 0;
                if (tableData.Count > 1)
                {
                    total1 = qry2.Sum(n => n.DiffLossCost);
                }
                //double total1 = qry2.Sum(n => n.YParam1);

                ColumnSeries ps = new ColumnSeries
                {
                    DataLabels = true,
                    LabelPoint = point => string.Format("{0:N0}", point.Y),
                    FontSize = 11,
                    FontWeight = FontWeights.Normal,
                    Values = new ChartValues<ObservableValue>()
                };

                foreach (var newY in qry1.ToList())
                {
                    ps.Values.Add(new ObservableValue(newY.DiffLossCost));
                }
                //if (total1 != 0) 
                ps.Values.Add(new ObservableValue(total1));
                SeriesCollection.Add(ps);

                Labels.Clear();
                foreach (var newX in qry1.ToList())
                {
                    Labels.Add(newX.IdER + "_" + newX.ERName);
                };
                if (total1 != 0) Labels.Add("прочие");
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
                RenderTargetBitmap bmp = new RenderTargetBitmap(700, 410, 96, 96, PixelFormats.Pbgra32);

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
