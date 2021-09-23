using em.Helpers;
using em.MenuPages;
using em.Models;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace em.ServicesPages
{
    public class PieViewModel : INotifyPropertyChanged, IChart
    {
        public PieChart myChart;
        public SeriesCollection SeriesCollection { get; set; }

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
        public PieViewModel(ChartType pieType, List<FullFields> tData, List<TableStruct> tsList, string chartCaption, int sliderVal = 2)
        {
            SeriesCollection = new SeriesCollection();
            dataType = pieType;
            tableData = tData;
            tablestruct = tsList;
            ChartCaption = chartCaption;
            SliderValue = sliderVal;
        }

        private void ChartFill()
        {
            SeriesCollection.Clear();
            double total;
            double total1 = 0;
            if (dataType == ChartType.UseER)
            {
                total = tableData.Sum(n => Math.Abs(n.FactCost));
                var qry1 = from o in tableData
                           where Math.Abs(o.FactCost) >= Math.Abs(total * SliderValue / 100)
                           orderby o.FactCost descending
                           select new
                           {
                               o.FactCost,
                               o.IdER,
                               o.ERName
                           };
                var qry2 = from o in tableData
                           where Math.Abs(o.FactCost) < Math.Abs(total * SliderValue / 100)
                           select new
                           {
                               o.FactCost,
                               o.IdER,
                               o.ERName
                           };

                total1 = qry2.Count() > 0 ? qry2.Sum(n => n.FactCost) : 0;

                foreach (var newY in qry1)
                {
                    PieSeries ps = new PieSeries
                    {
                        Title = newY.IdER + "_" + newY.ERName,
                        DataLabels = true,
                        FontSize = 11,
                        Foreground = Brushes.White,
                        FontWeight = FontWeights.Bold,
                        LabelPoint = point => string.Format("{0:N0}", point.Y),
                        Values = new ChartValues<ObservableValue> { new ObservableValue(newY.FactCost) }
                    };
                    SeriesCollection.Add(ps);

                }
            }
            else if (dataType == ChartType.UseCC)
            {
                total = tableData.Sum(n => Math.Abs(n.FactCost));
                var qry1 = from o in tableData
                           where Math.Abs(o.FactCost) >= Math.Abs(total * SliderValue / 100)
                           orderby o.FactCost descending
                           select new
                           {
                               o.FactCost,
                               o.IdCC,
                           };
                var qry2 = from o in tableData
                           where Math.Abs(o.FactCost) < Math.Abs(total * SliderValue / 100)
                           select new
                           {
                               o.FactCost,
                               o.IdCC,
                           };

                total1 = qry2.Count() > 0 ? qry2.Sum(n => n.FactCost) : 0;

                foreach (var newY in qry1)
                {
                    PieSeries ps = new PieSeries
                    {
                        Title = "ЦЗ-" + newY.IdCC,
                        DataLabels = true,
                        FontSize = 11,
                        Foreground = Brushes.White,
                        FontWeight = FontWeights.Bold,
                        LabelPoint = point => string.Format("{0:N0}", point.Y),
                        Values = new ChartValues<ObservableValue> { new ObservableValue(newY.FactCost) }
                    };
                    SeriesCollection.Add(ps);

                }
            }
            else if (dataType == ChartType.UsePR)
            {
                total = tableData.Sum(n => Math.Abs(n.FactCost));
                var qry1 = from o in tableData
                           where Math.Abs(o.FactCost) >= Math.Abs(total * SliderValue / 100)
                           orderby o.FactCost descending
                           select new
                           {
                               o.FactCost,
                               o.IdProduct,
                               o.ProductName,
                           };
                var qry2 = from o in tableData
                           where Math.Abs(o.FactCost) < Math.Abs(total * SliderValue / 100)
                           select new
                           {
                               o.FactCost,
                               o.IdProduct,
                               o.ProductName,
                           };

                total1 = qry2.Count() > 0 ? qry2.Sum(n => n.FactCost) : 0;

                foreach (var newY in qry1)
                {
                    PieSeries ps = new PieSeries
                    {
                        Title = newY.IdProduct + "_" + newY.ProductName,
                        DataLabels = true,
                        FontSize = 11,
                        Foreground = Brushes.White,
                        FontWeight = FontWeights.Bold,
                        LabelPoint = point => string.Format("{0:N0}", point.Y),
                        Values = new ChartValues<ObservableValue> { new ObservableValue(newY.FactCost) }
                    };
                    SeriesCollection.Add(ps);

                }
            }

            if (dataType == ChartType.LossesER)
            {
                total = tableData.Sum(n => Math.Abs(n.FactLossCost));
                var qry1 = from o in tableData
                           where Math.Abs(o.FactLossCost) >= Math.Abs(total * SliderValue / 100)
                           orderby o.FactLossCost descending
                           select new
                           {
                               o.FactLossCost,
                               o.IdER,
                               o.ERName
                           };
                var qry2 = from o in tableData
                           where Math.Abs(o.FactLossCost) < Math.Abs(total * SliderValue / 100)
                           select new
                           {
                               o.FactLossCost,
                               o.IdER,
                               o.ERName
                           };

                total1 = qry2.Sum(n => n.FactLossCost);

                foreach (var newY in qry1)
                {
                    PieSeries ps = new PieSeries
                    {
                        Title = newY.IdER + "_" + newY.ERName,
                        DataLabels = true,
                        FontSize = 11,
                        Foreground = Brushes.White,
                        FontWeight = FontWeights.Bold,
                        LabelPoint = point => string.Format("{0:N0}", point.Y),
                        Values = new ChartValues<ObservableValue> { new ObservableValue(newY.FactLossCost) }
                    };
                    SeriesCollection.Add(ps);

                }
            }
            if (total1 > 0)
            {
                PieSeries ps1 = new PieSeries
                {
                    Title = "прочие",
                    DataLabels = true,
                    FontSize = 11,
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold,
                    LabelPoint = point => string.Format("{0:N0}", point.Y),
                    Values = new ChartValues<ObservableValue> { new ObservableValue(total1) }
                };
                SeriesCollection.Add(ps1);

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
