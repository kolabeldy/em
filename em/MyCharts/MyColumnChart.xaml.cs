using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace em.MyCharts
{
    public partial class MyColumnChart : UserControl, INotifyPropertyChanged
    {
        enum ValuesType { AllPlus, AllMinus, PlusMinus };
        private ValuesType valuesType;
        #region Caption
        public class Captions
        {
            public string Text { get; set; } = "Chart Caption";
            public int FontSize { get; set; } = 10;
            public FontWeight FontWeight { get; set; } = FontWeights.Normal;
            public Brush Foreground { get; set; } = Brushes.Black;
        }
        public Captions Caption = new Captions();
        #endregion

        #region Labels
        public class Labels
        {
            public int FontSize { get; set; } = 10;
            public FontWeight FontWeight { get; set; } = FontWeights.Normal;
            public Brush Foreground { get; set; } = Brushes.Black;
            public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Center;
            public TextWrapping TextWrapping { get; set; } = TextWrapping.Wrap;
            public int PanelHeight { get; set; } = 30;
        }
        public Labels Label = new Labels();

        #endregion

        #region ValueLabels
        public class ValueLabels
        {
            public int FontSize { get; set; } = 10;
            public FontWeight FontWeight { get; set; } = FontWeights.Normal;
            public Brush Foreground { get; set; } = Brushes.Black;
            public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Center;
            public TextWrapping TextWrapping { get; set; } = TextWrapping.Wrap;
            public string StringFormat { get; set; } = "N";
        }
        public ValueLabels ValueLabel = new ValueLabels();

        #endregion

        public class ColumnSerie
        {
            public string Name { get; set; }
            public double Value { get; set; }
            public Brush Fill { get; set; } = Brushes.SteelBlue;

        }

        private List<ColumnSerie> columnSeries;
        public List<ColumnSerie> ColumnSeries
        {
            get => columnSeries;
            set
            {
                columnSeries = value;
                OnPropertyChanged("ColumnSeries");
            }
        }
        public new Brush Background { get; set; } = Brushes.Transparent;
        public bool SeparatorShow { get; set; } = false;

        public MyColumnChart()
        {
            InitializeComponent();
            DataContext = this;
        }
        public void Show(double height, double width)
        {
            GridChart.Height = height;
            GridChart.Width = width;
            GridChart.Background = Background;

            for (int i = 0; i < ColumnSeries.Count; i++)
            {
                FooterArea.ColumnDefinitions.Add(new ColumnDefinition()); // { Width = new GridLength((width - 10) / ColumnSeries.Count) }) 
                ChartArea.ColumnDefinitions.Add(new ColumnDefinition());

                StackPanel footerPanel = new();
                //footerPanel.Background = Brushes.LightCyan;

                TextBlock xLabel = new();
                //xLabel.Background = Brushes.LightCoral;
                xLabel.Text = ColumnSeries[i].Name;
                xLabel.Margin = new Thickness(2, 0, 2, 0);
                xLabel.HorizontalAlignment = Label.HorizontalAlignment;
                xLabel.TextWrapping = Label.TextWrapping;
                xLabel.FontSize = Label.FontSize;
                xLabel.FontWeight = Label.FontWeight;
                xLabel.Foreground = Label.Foreground;

                footerPanel.Children.Add(xLabel);

                FooterArea.Children.Add(footerPanel);
                Grid.SetRow(footerPanel, 2);
                Grid.SetColumn(footerPanel, i);
            }

            double headerHeight = 30;
            Header.Height = headerHeight;
            Header.Text = Caption.Text;
            Header.FontSize = Caption.FontSize;
            Header.FontWeight = Caption.FontWeight;
            Header.Foreground = Caption.Foreground;

            double footerHeight = Label.PanelHeight;
            Footer.Height = new GridLength(footerHeight);

            double minValue = ColumnSeries.Min(m => m.Value);
            double maxValue = ColumnSeries.Max(m => m.Value);
            double areaHeight = height - headerHeight - footerHeight - 5;
            double areaWidth = width;
            ChartArea.Width = areaWidth;
            double sumAbsHeight = Math.Abs(maxValue) + Math.Abs(minValue);

            valuesType = (maxValue <= 0) && (minValue <= 0) ? ValuesType.AllMinus : (maxValue > 0) && (minValue > 0) ? ValuesType.AllPlus : ValuesType.PlusMinus;
            double upRowHeight = 0;
            double downRowHeight = 0;
            if (valuesType == ValuesType.AllMinus)
            {
                upRowHeight = 10;
                downRowHeight = areaHeight - 10;
            }
            else if(valuesType == ValuesType.AllPlus)
            {
                downRowHeight = 10;
                upRowHeight = areaHeight - 10;
            }
            else
            {
                upRowHeight = areaHeight / sumAbsHeight * Math.Abs(maxValue);
                downRowHeight = areaHeight - upRowHeight;
            }

            //ChartArea.Background = Brushes.LightGray;

            for (int i = 0; i < ColumnSeries.Count; i++)
            {
                double val = ColumnSeries[i].Value;

                Border borderUp = new();
                borderUp.BorderBrush = Brushes.Black;
                Border borderDown = new();
                borderDown.BorderBrush = Brushes.Black;
                if (i < ColumnSeries.Count - 1)
                {
                    borderUp.BorderThickness = SeparatorShow ? new Thickness(0.2, 0, 0, 0.2) : new Thickness(0, 0, 0, 0.2);
                    borderDown.BorderThickness = SeparatorShow ? new Thickness(0.2, 0.2, 0, 0) : new Thickness(0, 0.2, 0, 0);
                }
                else
                {
                    borderUp.BorderThickness = SeparatorShow ? new Thickness(0.2, 0, 0.2, 0.2) : new Thickness(0, 0, 0, 0.2);
                    borderDown.BorderThickness = SeparatorShow ? new Thickness(0.2, 0.2, 0.2, 0) : new Thickness(0, 0.2, 0, 0);
                }
                StackPanel sp = new StackPanel();
                sp.HorizontalAlignment = HorizontalAlignment.Stretch;
                sp.VerticalAlignment = val > 0 ? VerticalAlignment.Bottom : VerticalAlignment.Top;
                Border column = new();
                column.Margin = new Thickness((areaWidth / ColumnSeries.Count) / 8, 0, (areaWidth / ColumnSeries.Count) / 8, 0);
                column.Background = ColumnSeries[i].Fill;

                if(valuesType == ValuesType.PlusMinus)
                    column.Height = (areaHeight - 20) / sumAbsHeight * Math.Abs(val);
                else if (valuesType == ValuesType.AllMinus)
                    column.Height = (areaHeight - 20) / Math.Abs(minValue) * Math.Abs(val);
                else column.Height = (areaHeight - 20) / maxValue * val;

                TextBlock labelValue = new();
                labelValue.HorizontalAlignment = ValueLabel.HorizontalAlignment;
                labelValue.Text = val.ToString(ValueLabel.StringFormat);
                labelValue.FontSize = ValueLabel.FontSize;
                labelValue.FontWeight = ValueLabel.FontWeight;
                labelValue.Foreground = ValueLabel.Foreground;

                if (val > 0)
                {
                    sp.Children.Add(labelValue);
                    sp.Children.Add(column);
                    borderUp.Child = sp;
                }
                else
                {
                    sp.Children.Add(column);
                    sp.Children.Add(labelValue);
                    borderDown.Child = sp;
                }

                ChartArea.Children.Add(borderUp);
                Grid.SetRow(borderUp, 0);
                Grid.SetColumn(borderUp, i);

                ChartArea.Children.Add(borderDown);
                Grid.SetRow(borderDown, 1);
                Grid.SetColumn(borderDown, i);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }



    }
}
