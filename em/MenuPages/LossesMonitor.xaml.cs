using em.Models;
using em.FiltersSections;
using em.ServicesPages;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using em.Helpers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using em.DBAccess;

namespace em.MenuPages
{
    public partial class LossesMonitor : UserControl, INotifyPropertyChanged
    {
        private static LossesMonitor instance;
        public static LossesMonitor GetInstance()
        {
            if (instance == null)
                instance = new LossesMonitor();
            return instance;
        }
        public static bool isChange;

        protected List<Person> filterDateList;
        protected List<Person> filterCostCentersList = new();
        protected static List<Person> filterEnergyResourcesList = new();
        //protected List<Person> filterProductsList;

        public FilterSection FilterDate { get; set; }
        public FilterSection FilterCC { get; set; }
        public FilterSection FilterER { get; set; }
        //public FilterSection FilterPR { get; set; }

        protected FilterSectionDateViewModel modelDate;
        protected FilterSectionCostCentersViewModel modelCC;
        protected FilterSectionEnergyResourcesViewModel modelER;
        //protected FilterSectionCCandPRViewModel modelPR;
        //protected FilterSectionProductsViewModel modelPR1;


        protected bool filtersIsChanged = false;
        public bool FiltersIsChanged
        {
            get
            {
                return filtersIsChanged;
            }
            set
            {
                filtersIsChanged = value;
                OnPropertyChanged("FiltersIsChanged");
            }
        }

        private LossesMonitor()
        {
            InitializeComponent();
            DataContext = this;
            isChange = true;
            FilterInit(isDateOnly: false);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FormRefresh();
        }
        public void FilterInit(bool isDateOnly = false)
        {
            modelDate = new FilterSectionDateViewModel("Период:", TreeInitType.Last);
            modelDate.onChange += FilterDateOnChangeHandler;
            FilterDate = new FilterSection(modelDate);
            modelDate.Init();
            FilterDatePanel.Content = FilterDate;
            //modelER.PopupHorisontalOffset = 65;
            if (!isDateOnly)
            {
                modelER = new FilterSectionEnergyResourcesViewModel("Ресурсы:", TreeInitType.Losses);
                modelER.onChange += FilterEROnChangeHandler;
                FilterER = new FilterSection(modelER);
                modelER.Init();
                FilterERPanel.Content = FilterER;
                //modelER.PopupHorisontalOffset = 65;
                //PRSectionFilterActivate(isMonth);
            }
        }
        protected void FilterEROnChangeHandler()
        {
            FiltersIsChanged = true;
            filterEnergyResourcesList = modelER.FilterList;
        }
        protected void FilterDateOnChangeHandler()
        {
            FiltersIsChanged = true;
            filterDateList = modelDate.FilterList;
        }

        public void FormRefresh()
        {
            if (isChange)
            {
                Dashboard.Children.Clear();
                AddChart(RetPieER(filterDateList, filterEnergyResourcesList), ChartWidthType.Slim);
                AddChart(RetColumnER(filterDateList, filterEnergyResourcesList), ChartWidthType.Slim);
                AddChart(RetLineER(filterDateList, filterEnergyResourcesList), ChartWidthType.Wide);
                isChange = false;
            }
        }
        private void AddChart(IChart chart, ChartWidthType width)
        {
            if (chart != null) Dashboard.Children.Add(new UserControl
            {
                MinWidth = width == ChartWidthType.Slim ? 700 : 1410,
                Height = 450,
                Background = Brushes.WhiteSmoke,
                Margin = new Thickness(5, 0, 5, 10),
                Padding = new Thickness(0),
                Content = chart
            });
        }

        private PieChart RetPieER(List<Person> dateSel, List<Person> erSel)
        {
            if (erSel.Count == 1) return null;
            List<FullFields> table = DataAccess.RetLosses(ChartDataType.FactLoss, dateSel, erSel);
            bool isEROne = true;
            List<TableStruct> tstruct = new()
            {
                new TableStruct { Headers = "Код", Binding = "IdER", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Ресурс", Binding = "ERName", ColWidth = 1.5, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Разм.", Binding = "UnitName", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Факт", Binding = "FactLoss", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Норм.", Binding = "NormLoss", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Окл.", Binding = "DiffLoss", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Факт, тыс.руб.", Binding = "FactLossCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Норм., тыс.руб.", Binding = "NormLossCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Откл., тыс.руб.", Binding = "DiffLossCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "% Факт.", Binding = "FactLossProc", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N1}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "% Норм.", Binding = "NormLossProc", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N1}", ColumnVisible = isEROne, TotalRowTextVisible = false }
            };

            string capt = "Потери энергоресурсов. Всего: " + table.Sum(m => m.FactLossCost).ToString("N0") + ", тыс.руб.";
            return new(new(ChartType.LossesER, table, tstruct, capt));
        }
        private ColumnChart RetColumnER(List<Person> dateSel, List<Person> erSel)
        {
            if (erSel.Count == 1) return null;
            List<FullFields> table = DataAccess.RetLosses(ChartDataType.FactLoss, dateSel, erSel);
            bool isEROne = true;
            List<TableStruct> tstruct = new()
            {
                new TableStruct { Headers = "Код", Binding = "IdER", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Ресурс", Binding = "ERName", ColWidth = 1.5, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Разм.", Binding = "UnitName", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Факт", Binding = "FactLoss", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Норм.", Binding = "NormLoss", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Окл.", Binding = "DiffLoss", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Факт, тыс.руб.", Binding = "FactLossCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Норм., тыс.руб.", Binding = "NormLossCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Откл., тыс.руб.", Binding = "DiffLossCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "% Факт.", Binding = "FactLossProc", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N1}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "% Норм.", Binding = "NormLossProc", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N1}", ColumnVisible = isEROne, TotalRowTextVisible = false }
            };
            string capt = "Отклонение от нормативов. Всего: " + table.Sum(m => m.DiffLossCost).ToString("N0") + ", тыс.руб.";
            return new(new(ChartType.LossesER, table, tstruct, capt));
        }
        private LineChart RetLineER(List<Person> dateSel, List<Person> erSel)
        {
            int minCurrPeriod = dateSel.Min(m => m.Id);
            int maxCurrPeriod = dateSel.Max(m => m.Id);
            List<Person> dynamicDateList = Global.RetDynamicTruePeriod(minCurrPeriod, maxCurrPeriod);
            List<FullFields> table = DataAccess.RetLosses(ChartDataType.Period, dynamicDateList, erSel);
            bool isEROne = erSel.Count == 1;
            List<TableStruct> tstruct = new()
            {
                new TableStruct { Headers = "Период", Binding = "PeriodStr", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Код", Binding = "IdER", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Ресурс", Binding = "ERName", ColWidth = 1.5, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = true },
                new TableStruct { Headers = "Разм.", Binding = "UnitName", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Факт", Binding = "FactLoss", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Норм.", Binding = "NormLoss", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Окл.", Binding = "DiffLoss", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Факт, тыс.руб.", Binding = "FactLossCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Норм., тыс.руб.", Binding = "NormLossCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Откл., тыс.руб.", Binding = "DiffLossCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "% Факт.", Binding = "FactLossProc", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N1}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "% Норм.", Binding = "NormLossProc", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N1}", ColumnVisible = isEROne, TotalRowTextVisible = false }
            };
            string capt = "Динамика потерь и отклонений от норматива, тыс.руб.";
            return new(new(ChartType.LossesER, table, tstruct, capt));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            isChange = true;
            FormRefresh();
        }

    }
}
