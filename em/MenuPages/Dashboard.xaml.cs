using em.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using em.Helpers;
using em.DBAccess;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;
using Kant.Wpf.Controls.Chart;
using em.MyCharts;

namespace em.MenuPages
{
    /// <summary>
    /// Логика взаимодействия для Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window, INotifyPropertyChanged
    {
        private static Dashboard instance;
        public static Dashboard GetInstance()
        {
            if (instance == null)
                instance = new Dashboard();
            return instance;
        }

        public class PeriodId
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public List<PeriodId> Months { get; set; }
        public List<PeriodId> Years { get; set; }

        private PeriodId monthStartOld;
        private PeriodId monthEndOld;
        private PeriodId yearStartOld;
        private PeriodId yearEndOld;

        private PeriodId monthStart;
        public PeriodId MonthStart
        {
            get => monthStart;
            set
            {
                monthStartOld = monthStart;
                monthStart = value;
                OnPropertyChanged("MonthStart");
            }
        }
        private PeriodId monthEnd;
        public PeriodId MonthEnd
        {
            get => monthEnd;
            set
            {
                monthEndOld = monthEnd;
                monthEnd = value;
                OnPropertyChanged("MonthEnd");
            }
        }
        private PeriodId yearStart;
        public PeriodId YearStart
        {
            get => yearStart;
            set
            {
                yearStartOld = yearStart;
                yearStart = value;
                OnPropertyChanged("YearStart");
            }
        }
        private PeriodId yearEnd;
        public PeriodId YearEnd
        {
            get => yearEnd;
            set
            {
                yearEndOld = yearEnd;
                yearEnd = value;
                OnPropertyChanged("YearEnd");
            }
        }


        private int beginPeriod;
        private int endPeriod;
        private List<DataChart> tableData = new List<DataChart>();

        private List<FullFields> erUseList;
        private List<FullFields> ccUseList;
        private List<FullFields> diffPeriodList;
        private List<FullFields> erCompareList;
        private List<FullFields> ccCompareList;
        private List<FullFields> lossFactList;

        private double totalPrimeUse;
        private double totalLossPrime;
        private double totalProducedSecondary;
        private double totalLossSecondary;
        //private double totalUse;
        private double totalDiff;
        private double totalCompare;
        private double totalLosses;

        private void SetTotal()
        {
            SetTotalUse();
            SetTotalLossPrime();
            SetTotalBuy();
            SetTotalProducedSecondary();
            SetTotalLossSecondary();
            SetTotalDiff();
            SetTotalDiffCompare();
            SetTotalLosses();
        }
        private void SetTotalUse()
        {
            totalPrimeUse = (from o in erUseList
                             where o.IsERPrime
                             select new FullFields
                             {
                                 FactCost = o.FactCost
                             }).Sum(m => m.FactCost);

            TotalUse.Content = totalPrimeUse.ToString("N0");
        }
        private void SetTotalLossPrime()
        {
            totalLossPrime = (from o in lossFactList
                              where o.IsERPrime
                              select new FullFields
                              {
                                  FactCost = o.FactCost
                              }).Sum(m => m.FactCost);

            TotalLossPrime.Content = totalLossPrime.ToString("N0");
        }
        private void SetTotalBuy()
        {

            TotalBuy.Content = (totalPrimeUse + totalLossPrime).ToString("N0");
        }
        private void SetTotalProducedSecondary()
        {
            double totalUseSecondary = (from o in erUseList
                                        where !o.IsERPrime
                                        select new FullFields
                                        {
                                            FactCost = o.FactCost
                                        }).Sum(m => m.FactCost);
            totalProducedSecondary = totalUseSecondary + totalLossSecondary;
            TotalProducedSecondary.Content = totalProducedSecondary.ToString("N0");
        }
        private void SetTotalLossSecondary()
        {
            totalLossSecondary = (from o in lossFactList
                                  where !o.IsERPrime
                                  select new FullFields
                                  {
                                      FactCost = o.FactCost
                                  }).Sum(m => m.FactCost);
            TotalLossSecondary.Content = totalLossSecondary.ToString("N0");
        }
        private void SetTotalDiff()
        {
            totalDiff = TotalUseDiffFromERType();
            TotalDiff.Content = totalDiff.ToString("N0");
        }
        private void SetTotalDiffCompare()
        {
            totalCompare = TotalCompareFromERType();
            TotalDiffCompare.Content = totalCompare.ToString("N0"); ;
        }
        private void SetTotalLosses()
        {
            totalLosses = totalLossSecondary + totalLossPrime;
            TotalLosses.Content = totalLosses.ToString("N0");
        }

        public Dashboard()
        {
            Months = new()
            {
                new PeriodId() { Id = 1, Name = "янв" },
                new PeriodId() { Id = 2, Name = "фев" },
                new PeriodId() { Id = 3, Name = "мар" },
                new PeriodId() { Id = 4, Name = "апр" },
                new PeriodId() { Id = 5, Name = "май" },
                new PeriodId() { Id = 6, Name = "июн" },
                new PeriodId() { Id = 7, Name = "июл" },
                new PeriodId() { Id = 8, Name = "авг" },
                new PeriodId() { Id = 9, Name = "сен" },
                new PeriodId() { Id = 10, Name = "окт" },
                new PeriodId() { Id = 11, Name = "ноя" },
                new PeriodId() { Id = 12, Name = "дек" },

            };
            Years = new();
            for (int y = 0; y <= Period.LastYear - Period.FirstYear; y++)
            {
                PeriodId yy = new();
                yy.Id = y + 1;
                yy.Name = (Period.FirstYear + y).ToString();
                Years.Add(yy);
            }

            beginPeriod = Period.LastPeriod;
            endPeriod = Period.LastPeriod;

            MonthStart = Months[Period.LastMonth - 1];
            MonthEnd = Months[Period.LastMonth - 1];
            YearStart = Years[Years.Count - 1];
            YearEnd = Years[Years.Count - 1];

            InitializeComponent();
            DataContext = this;
        }

        public void ChartSankeyShow()
        {
            List<FullFields> prime = FullFields.UsePrime(FullFields.SelectPeriodList(beginPeriod, endPeriod));
            List<FullFields> second = FullFields.UseSecondary(FullFields.SelectPeriodList(beginPeriod, endPeriod));
            double total = prime.Sum(s => s.FactCost) + totalLossPrime;
            string name23 = "";
            List<SankeyData> datas = new();
            foreach (var r in prime)
            {
                SankeyData n = new();
                n.From = "Всего\n" + total.ToString("N0");
                n.To = r.CCName;
                if(r.CCName == "ЦЗ-023")
                {
                    name23 = n.To;
                }    
                n.Weight = r.FactCost;
                datas.Add(n);
            }
            foreach (var r in second)
            {
                SankeyData n = new();
                n.From = name23;
                n.To = r.CCName;
                n.Weight = r.FactCost;
                datas.Add(n);
            }
            datas.Add(new SankeyData() { From = "Всего\n" + total.ToString("N0"), To = "потери", Weight = Math.Round(totalLossPrime,0) });
            datas.Add(new SankeyData() { From = name23, To = "потери", Weight = Math.Round(totalLossSecondary, 0) });

            PanelSankey.Content = new MySankeyChart(datas, "Распределение энергоресурсов");
        }

        private static int RetBeginDynamic(int beginPeriod, int endPeriod)
        {
            int year = endPeriod / 100;
            int month = endPeriod - year * 100;
            int endP = (endPeriod - beginPeriod) < 2 ? (year - 1) * 100 + month + 1 : beginPeriod;
            return endP;
        }
        private void Refresh()
        {
            erUseList = FullFields.RetUseFromER(FullFields.SelectPeriodList(beginPeriod, endPeriod), CostCenter.AllToList(), EResource.AllToList(), "все");
            ccUseList = FullFields.RetUseFromCC(FullFields.SelectPeriodList(beginPeriod, endPeriod), CostCenter.AllToList(), EResource.AllToList(), "все");
            erCompareList = FullFields.RetUseCompare(ChartDataType.ER, FullFields.SelectPeriodList(beginPeriod, endPeriod), CostCenter.AllToList(), EResource.AllToList(), "все");
            ccCompareList = FullFields.RetUseCompare(ChartDataType.CC, FullFields.SelectPeriodList(beginPeriod, endPeriod), CostCenter.AllToList(), EResource.AllToList(), "все");
            lossFactList = FactLosse.ToList(FullFields.SelectPeriodList(beginPeriod, endPeriod));
            diffPeriodList = FullFields.RetUsePeriodFromER(FullFields.SelectPeriodList(RetBeginDynamic(beginPeriod, endPeriod), endPeriod), CostCenter.AllToList(), EResource.AllToList(), "все");

            TotalUseDiffFromCCType();
            TotalUseDiffFromNormType();
            TotalDiffFromPeriod();
            TotalCompareFromCCType();
            TotalCompareFromNormType();

            //UseFromER();
            DiffFromER();
            UseFromCC();
            DiffFromCC();
            CompareFromER();
            CompareFromCC();

            SetTotal();
            ChartSankeyShow();

            string totalType = "";
            totalType = totalDiff >= 0 ? "Перерасход " : "Экономия ";
            NameBlock1.Text = "Отклонения от нормативов. " + totalType + "всего: " + Math.Abs(totalDiff).ToString("N0") + ", тыс.руб.";

            totalType = totalCompare >= 0 ? "Перерасход " : "Экономия ";
            NameBlock3.Text = "Сравнение с базовым периодом. " + totalType + "всего: " + Math.Abs(totalCompare).ToString("N0") + ", тыс.руб.";


        }
        private double TotalUseDiffFromERType()
        {
            tableData.Clear();
            foreach (var r in FullFields.RetTotalUseDiffFromERType(FullFields.SelectPeriodList(beginPeriod, endPeriod)))
            {
                DataChart n = new();
                n.YParam1 = r.DiffCost;
                n.XParam = r.TotalParamX;
                tableData.Add(n);
            }
            TotalDiagramShow(PanelDiffFromTypeER, tableData, "по типу ресурсов");
            return tableData.Sum(s => s.YParam1);
        }
        private void TotalUseDiffFromCCType()
        {
            tableData.Clear();
            foreach (var r in FullFields.RetTotalUseDiffFromCCType(FullFields.SelectPeriodList(beginPeriod, endPeriod)))
            {
                DataChart n = new();
                n.YParam1 = r.DiffCost;
                n.XParam = r.TotalParamX;
                tableData.Add(n);
            }
            TotalDiagramShow(PanelDiffFromTypeCC, tableData, "по типу центров затрат");
        }
        private void TotalUseDiffFromNormType()
        {
            tableData.Clear();
            foreach (var r in FullFields.RetTotalUseDiffFromNormType(FullFields.SelectPeriodList(beginPeriod, endPeriod)))
            {
                DataChart n = new();
                n.YParam1 = r.DiffCost;
                n.XParam = r.TotalParamX;
                tableData.Add(n);
            }
            TotalDiagramShow(PanelDiffFromTypeNorm, tableData, "по типу нормирования");
        }
        private void TotalDiffFromPeriod()
        {
            tableData.Clear();
            foreach (var r in diffPeriodList)
            {
                DataChart n = new();
                n.YParam1 = r.DiffCost;
                n.XParam = r.PeriodStr;
                tableData.Add(n);
            }
            PanelDiffFromPeriod.Content = new MyLineChart(tableData, "Динамика отклонений от нормативов");
        }

        private double TotalCompareFromERType()
        {
            tableData.Clear();
            var qry = (from o in erCompareList
                       group o by new { o.IsERPrime } into gr
                       select new FullFields
                       {
                           IsERPrime = gr.Key.IsERPrime,
                           dFactCost = gr.Sum(m => m.dFactCost)
                       }).ToList();


            foreach (var r in qry)
            {
                DataChart n = new();
                n.YParam1 = r.dFactCost;
                n.XParam = r.IsERPrime ? "первичные" : "вторичные";
                tableData.Add(n);
            }
            TotalDiagramShow(PanelDiffCompareFromERType, tableData, "по типу ресурсов");
            return qry.Sum(s => s.dFactCost);
        }
        private void TotalCompareFromCCType()
        {
            tableData.Clear();
            var qry = (from o in ccCompareList
                       group o by new { o.IsCCTechnology } into gr
                       select new FullFields
                       {
                           IsCCTechnology = gr.Key.IsCCTechnology,
                           dFactCost = gr.Sum(m => m.dFactCost)
                       }).ToList();


            foreach (var r in qry)
            {
                DataChart n = new();
                n.YParam1 = r.dFactCost;
                n.XParam = r.IsCCTechnology ? "технологические" : "вспомогательные";
                tableData.Add(n);
            }
            TotalDiagramShow(PanelDiffCompareFromCCType, tableData, "по типу цехов");
        }
        private void TotalCompareFromNormType()
        {
            tableData.Clear();
            var qry = (from o in erCompareList
                       group o by new { o.IsNorm } into gr
                       select new FullFields
                       {
                           IsNorm = gr.Key.IsNorm,
                           dFactCost = gr.Sum(m => m.dFactCost)
                       }).ToList();


            foreach (var r in qry)
            {
                DataChart n = new();
                n.YParam1 = r.dFactCost;
                n.XParam = r.IsNorm ? "нормируемые" : "лимитируемые";
                tableData.Add(n);
            }
            TotalDiagramShow(PanelDiffCompareFromNormType, tableData, "по типу нормирования");
        }


        //private void TotalLossFromFactNorm()
        //{
        //    //tableData.Clear();
        //    //foreach (var r in FullFields.RetTotalLossFromFactNorm(FullFields.SelectPeriodList(beginPeriod, endPeriod)))
        //    //{
        //    //    DataChart n = new();
        //    //    n.YParam1 = r.FactCost;
        //    //    n.XParam = r.TotalParamX;
        //    //    tableData.Add(n);
        //    //}
        //    //TotalDiagramFactNormShow(PanelDiffLossFromFactNorm, tableData, "Факт - План");
        //}
        //private void TotalLossDiffFromERType()
        //{
        //    //tableData.Clear();
        //    //foreach (var r in FullFields.RetTotalLossDiffFromERType(FullFields.SelectPeriodList(beginPeriod, endPeriod)))
        //    //{
        //    //    DataChart n = new();
        //    //    n.YParam1 = r.DiffCost;
        //    //    n.XParam = r.TotalParamX;
        //    //    tableData.Add(n);
        //    //}
        //    //TotalDiagramShow(PanelDiffLossFromTypeER, tableData, "");
        //}
        //private void UseFromER()
        //{
        //    tableData.Clear();
        //    foreach (var r in erUseList)
        //    {
        //        DataChart n = new();
        //        n.YParam1 = r.FactCost;
        //        n.XParam = r.IdER.ToString() + " " + r.ERShortName;
        //        tableData.Add(n);
        //    }
        //    PanelUseFromER.Content = new Dashboards.DashboardPieChart(tableData);
        //}
        private void DiffFromER()
        {
            tableData.Clear();
            foreach (var r in erUseList)
            {
                DataChart n = new();
                n.YParam1 = r.DiffCost;
                n.XParam = r.IdER.ToString() + " " + r.ERShortName;
                tableData.Add(n);
            }
            DiagramPanelShow(PanelDiffFromER, tableData, "Отклонения от нормативов");
        }
        private void UseFromCC()
        {
            //tableData.Clear();
            //foreach (var r in ccUseList)
            //{
            //    DataChart n = new();
            //    n.YParam1 = r.FactCost;
            //    n.XParam = r.IdCC.ToString();
            //    tableData.Add(n);
            //}
            //PanelUseFromCC.Content = new RowChart(tableData);
        }
        private void DiffFromCC()
        {
            tableData.Clear();
            foreach (var r in ccUseList)
            {
                DataChart n = new();
                n.YParam1 = r.DiffCost;
                n.XParam = r.IdCC.ToString();
                tableData.Add(n);
            }
            DiagramPanelShow(PanelDiffFromCC, tableData, "Отклонения от нормативов");
        }
        private void CompareFromER()
        {
            tableData.Clear();
            foreach (var r in erCompareList)
            {
                DataChart n = new();
                n.YParam1 = r.dFactCost;
                n.XParam = r.IdER.ToString() + " " + r.ERShortName;
                tableData.Add(n);
            }
            DiagramPanelShow(PanelCompareER, tableData, "Отклонения от показателей базового периода");
        }

        private void CompareFromCC()
        {
            tableData.Clear();
            foreach (var r in ccCompareList)
            {
                DataChart n = new();
                n.YParam1 = r.dFactCost;
                n.XParam = r.IdCC.ToString();
                tableData.Add(n);
            }
            DiagramPanelShow(PanelCompareCC, tableData, "Отклонения от базовых показателей");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbMonthStart.SelectedItem = MonthStart;
            cbMonthStart.Text = MonthStart.Name;
            cbMonthEnd.SelectedItem = MonthEnd;
            cbMonthEnd.Text = MonthEnd.Name;

            cbYearStart.SelectedItem = YearStart;
            cbYearStart.Text = YearStart.Name;
            cbYearEnd.SelectedItem = YearEnd;
            cbYearEnd.Text = YearEnd.Name;

            Refresh();
        }

        #region Panel Show
        private void DiagramPanelShow(UserControl panel, List<DataChart> data, string caption = "")
        {
            double totalAbs;
            //double totalMain;
            double totalOther;

            totalAbs = data.Sum(n => Math.Abs(n.YParam1));
            //totalMain = data.Sum(n => n.YParam1);

            var qry1 = from o in data
                       where Math.Abs(o.YParam1) >= Math.Abs(totalAbs * 2 / 100)
                       orderby o.YParam1 descending
                       select new
                       {
                           o.YParam1,
                           o.XParam
                       };
            var temp = qry1.ToList();

            var qry2 = from o in data
                       where Math.Abs(o.YParam1) < Math.Abs(totalAbs * 2 / 100)
                       select new
                       {
                           o.YParam1,
                           o.XParam
                       };

            totalOther = qry2.Count() > 0 ? qry2.Sum(n => n.YParam1) : 0;

            MyColumnChart myChart = new MyColumnChart()
            {
                //PositiveColor = Brushes.OrangeRed,
                //NegativeColor = Brushes.CadetBlue,
                //SeparatorShow = true,
                //Background = Brushes.AliceBlue,

                Caption = new MyColumnChart.Captions()
                {
                    Text = caption, // + ". Всего: " + totalMain.ToString("N0"),
                    FontSize = 14,
                    //Foreground = Brushes.Red,
                    //FontWeight = FontWeights.Bold
                },
                Label = new MyColumnChart.Labels()
                {
                    FontSize = 10,
                    TextWrapping = TextWrapping.WrapWithOverflow,
                    PanelHeight = 30,
                    //Foreground = Brushes.Red,
                    //FontWeight = FontWeights.Bold
                },
                ValueLabel = new MyColumnChart.ValueLabels()
                {
                    FontSize = 10,
                    TextWrapping = TextWrapping.WrapWithOverflow,
                    //Foreground = Brushes.Red,
                    //FontWeight = FontWeights.Bold,
                    StringFormat = "N0"
                },
            };

            myChart.ColumnSeries = new();
            myChart.ColumnSeries.Clear();
            foreach (var r in qry1)
            {
                myChart.ColumnSeries.Add(new MyColumnChart.ColumnSerie { Name = r.XParam, Value = r.YParam1, Fill = r.YParam1 > 0 ? Brushes.Crimson : Brushes.CadetBlue });
            }
            myChart.ColumnSeries.Add(new MyColumnChart.ColumnSerie { Name = "Прочие", Value = totalOther, Fill = totalOther > 0 ? Brushes.Crimson : Brushes.CadetBlue });

            panel.Content = myChart;
            myChart.Show(panel.ActualHeight, panel.ActualWidth);

        }
        private void TotalDiagramShow(UserControl panel, List<DataChart> data, string caption = "")
        {
            double totalMain;
            totalMain = data.Sum(n => n.YParam1);

            MyColumnChart myChart = new MyColumnChart()
            {
                //SeparatorShow = true,
                //Background = Brushes.AliceBlue,

                Caption = new MyColumnChart.Captions()
                {
                    Text = caption, // + "Перерасход: " + totalMain.ToString("N0"),
                    FontSize = 12,
                    //Foreground = Brushes.Red,
                    //FontWeight = FontWeights.Bold
                },
                Label = new MyColumnChart.Labels()
                {
                    FontSize = 10,
                    TextWrapping = TextWrapping.WrapWithOverflow,
                    PanelHeight = 20,
                    //Foreground = Brushes.Red,
                    //FontWeight = FontWeights.Bold
                },
                ValueLabel = new MyColumnChart.ValueLabels()
                {
                    FontSize = 10,
                    TextWrapping = TextWrapping.WrapWithOverflow,
                    //Foreground = Brushes.Red,
                    //FontWeight = FontWeights.Bold,
                    StringFormat = "N0"
                },
            };

            myChart.ColumnSeries = new();
            myChart.ColumnSeries.Clear();

            foreach (var r in data)
            {
                myChart.ColumnSeries.Add(new MyColumnChart.ColumnSerie { Name = r.XParam, Value = r.YParam1, Fill = r.YParam1 > 0 ? Brushes.Crimson : Brushes.CadetBlue });
            }
            panel.Content = myChart;
            myChart.Show(panel.ActualHeight, panel.ActualWidth);

        }

        #endregion

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            beginPeriod = Convert.ToInt32(YearStart.Name) * 100 + MonthStart.Id;
            endPeriod = Convert.ToInt32(YearEnd.Name) * 100 + MonthEnd.Id;
            if (beginPeriod > endPeriod)
            {
                MonthStart = monthStartOld;
                MonthEnd = monthEndOld;
                YearStart = yearStartOld;
                YearEnd = yearEndOld;
                return;
            }
            Refresh();
        }

        public List<FullFields> RetTotalDiffCompareFromERType(List<Person> dateSel)
        {
            List<FullFields> rez = new List<FullFields>();
            //var qry = from o in erCompareList

            return rez;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //Dashboard.GetInstance().Close();
        }

        private void BtnPhoto_Click(object sender, RoutedEventArgs e)
        {
            ScreenSave();
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
                RenderTargetBitmap bmp = new RenderTargetBitmap(1840, 1028, 96, 96, PixelFormats.Pbgra32);

                bmp.Render(this);
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Interlace = PngInterlaceOption.On;
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                encoder.Save(stream);
            }
        }
    }
}
