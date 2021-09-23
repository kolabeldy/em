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
using System.Data;

namespace em.MenuPages
{
    public partial class MonthMonitor : UserControl, INotifyPropertyChanged
    {
        private static MonthMonitor instance;
        public static MonthMonitor GetInstance()
        {
            if (instance == null)
                instance = new MonthMonitor();
            return instance;
        }
        public static bool isChange;

        private List<Person> filterDateList;
        private List<Person> filterCostCentersList;
        private static List<Person> filterEnergyResourcesList;
        //private List<Person> filterProductsList;

        public FilterSection FilterDate { get; set; }
        public FilterSection FilterCC { get; set; }
        public FilterSection FilterER { get; set; }
        public string FilterPR { get; set; }

        private FilterSectionDateViewModel modelDate;
        private FilterSectionCostCentersViewModel modelCC;
        private FilterSectionEnergyResourcesViewModel modelER;
        //private FilterSectionCCandPRViewModel modelPR;
        //private FilterSectionProductsViewModel modelPR1;


        private bool filtersIsChanged = false;
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
        private MonthMonitor()
        {
            InitializeComponent();
            DataContext = this;
            isChange = true;
            FilterInit(isDateOnly: false);
            FilterPR = "все";
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
            if (!isDateOnly)
            {
                modelCC = new FilterSectionCostCentersViewModel("ЦЗ:", TreeInitType.All);
                modelCC.onChange += FilterCCOnChangeHandler;
                FilterCC = new FilterSection(modelCC);
                modelCC.Init();
                FilterCCPanel.Content = FilterCC;

                modelER = new FilterSectionEnergyResourcesViewModel("Ресурсы:", TreeInitType.All);
                modelER.onChange += FilterEROnChangeHandler;
                FilterER = new FilterSection(modelER);
                modelER.Init();
                FilterERPanel.Content = FilterER;
            }
        }

        private void FilterEROnChangeHandler()
        {
            FiltersIsChanged = true;
            filterEnergyResourcesList = modelER.FilterList;
        }
        private void FilterCCOnChangeHandler()
        {
            FiltersIsChanged = true;
            filterCostCentersList = modelCC.FilterList;
        }
        private void FilterDateOnChangeHandler()
        {
            FiltersIsChanged = true;
            filterDateList = modelDate.FilterList;
        }
        public void FormRefresh()
        {
            if (isChange)
            {
                Dashboard.Children.Clear();

                AddChart(RetPieER(filterDateList, filterCostCentersList, filterEnergyResourcesList, FilterPR), ChartWidthType.Slim);
                AddChart(RetColumnER(filterDateList, filterCostCentersList, filterEnergyResourcesList, FilterPR), ChartWidthType.Slim);
                AddChart(RetPieCC(filterDateList, filterCostCentersList, filterEnergyResourcesList, FilterPR), ChartWidthType.Slim);
                AddChart(RetColumnCC(filterDateList, filterCostCentersList, filterEnergyResourcesList, FilterPR), ChartWidthType.Slim);

                AddChart(RetPieProduct(filterDateList, filterCostCentersList, filterEnergyResourcesList, FilterPR), ChartWidthType.Slim);
                AddChart(RetColumnProduct(filterDateList, filterCostCentersList, filterEnergyResourcesList, FilterPR), ChartWidthType.Slim);

                AddChart(RetLineER(filterDateList, filterCostCentersList, filterEnergyResourcesList, FilterPR), ChartWidthType.Wide);

                AddChart(RetERCompare(filterDateList, filterCostCentersList, filterEnergyResourcesList, FilterPR), ChartWidthType.Slim);
                AddChart(RetCCCompare(filterDateList, filterCostCentersList, filterEnergyResourcesList, FilterPR), ChartWidthType.Slim);

                AddChart(RetLineCompare(filterDateList, filterCostCentersList, filterEnergyResourcesList, FilterPR), ChartWidthType.Wide);

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
        public static PieChart RetPieER(List<Person> dateSel, List<Person> ccSel, List<Person> erSel, string prSel)
        {
            if (erSel.Count == 1) return null;
            List<FullFields> table = FullFields.RetUseFromER(dateSel, ccSel, erSel, prSel);
            List<TableStruct> tstruct = new()
            {
                new TableStruct { Headers = "Код", Binding = "IdER", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = false },
                new TableStruct { Headers = "Ресурс", Binding = "ERName", ColWidth = 1.5, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Разм.", Binding = "UnitName", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = false },
                new TableStruct { Headers = "Факт", Binding = "Fact", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = true, TotalRowTextVisible = false },
                new TableStruct { Headers = "План", Binding = "Plan", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = true, TotalRowTextVisible = false },
                new TableStruct { Headers = "Окл.", Binding = "Diff", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = true, TotalRowTextVisible = false },
                new TableStruct { Headers = "Факт, тыс.руб.", Binding = "FactCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "План, тыс.руб.", Binding = "PlanCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Окл., тыс.руб.", Binding = "DiffCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Окл., %", Binding = "DiffProc", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N1}", ColumnVisible = true, TotalRowTextVisible = true }
            };
            string capt = "Распределение энергоресурсов"; //. Всего: " + table.Sum(m => m.FactCost).ToString("N0") + ", тыс.руб.";
            return new(new(ChartType.UseER, table, tstruct, capt));
        }
        private ColumnChart RetColumnER(List<Person> dateSel, List<Person> ccSel, List<Person> erSel, string prSel)
        {
            if (erSel.Count == 1) return null;
            List<FullFields> table = FullFields.RetUseFromER(dateSel, ccSel, erSel, prSel);
            List<TableStruct> tstruct = new()
            {
                new TableStruct { Headers = "Код", Binding = "IdER", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = false },
                new TableStruct { Headers = "Ресурс", Binding = "ERName", ColWidth = 1.5, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Разм.", Binding = "UnitName", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = false },
                new TableStruct { Headers = "Факт", Binding = "Fact", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = true, TotalRowTextVisible = false },
                new TableStruct { Headers = "План", Binding = "Plan", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = true, TotalRowTextVisible = false },
                new TableStruct { Headers = "Окл.", Binding = "Diff", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = true, TotalRowTextVisible = false },
                new TableStruct { Headers = "Факт, тыс.руб.", Binding = "FactCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "План, тыс.руб.", Binding = "PlanCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Окл., тыс.руб.", Binding = "DiffCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Окл., %", Binding = "DiffProc", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N1}", ColumnVisible = true, TotalRowTextVisible = true }
            };
            string capt = "Отклонение от расходных норм. Всего: " + table.Sum(m => m.DiffCost).ToString("N0") + ", тыс.руб.";
            return new(new(ChartType.UseER, table, tstruct, capt));
        }
        private PieChart RetPieCC(List<Person> dateSel, List<Person> ccSel, List<Person> erSel, string prSel)
        {
            if (ccSel.Count == 1) return null;
            List<FullFields> table = FullFields.RetUseFromCC(dateSel, ccSel, erSel, prSel);
            bool isEROne = erSel.Count == 1;
            List<TableStruct> tstruct = new()
            {
                new TableStruct { Headers = "Центр затрат", Binding = "CCName", ColWidth = 1, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Код", Binding = "IdER", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Ресурс", Binding = "ERName", ColWidth = 1.5, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = true },
                new TableStruct { Headers = "Разм.", Binding = "UnitName", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Факт", Binding = "Fact", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = isEROne },
                new TableStruct { Headers = "План", Binding = "Plan", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = isEROne },
                new TableStruct { Headers = "Окл.", Binding = "Diff", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = isEROne },
                new TableStruct { Headers = "Факт, тыс.руб.", Binding = "FactCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "План, тыс.руб.", Binding = "PlanCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Окл., тыс.руб.", Binding = "DiffCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Окл., %", Binding = "DiffProc", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N1}", ColumnVisible = true, TotalRowTextVisible = true }
            };
            string capt = "Распределение энергоресурсов"; //. Всего: " + table.Sum(m => m.FactCost).ToString("N0") + ", тыс.руб.";
            return new(new(ChartType.UseCC, table, tstruct, capt));
        }
        private ColumnChart RetColumnCC(List<Person> dateSel, List<Person> ccSel, List<Person> erSel, string prSel)
        {
            if (ccSel.Count == 1) return null;
            List<FullFields> table = FullFields.RetUseFromCC(dateSel, ccSel, erSel, prSel);
            bool isEROne = erSel.Count == 1;
            List<TableStruct> tstruct = new()
            {
                new TableStruct { Headers = "Центр затрат", Binding = "CCName", ColWidth = 1, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Код", Binding = "IdER", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Ресурс", Binding = "ERName", ColWidth = 1.5, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = true },
                new TableStruct { Headers = "Разм.", Binding = "UnitName", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Факт", Binding = "Fact", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = isEROne },
                new TableStruct { Headers = "План", Binding = "Plan", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = isEROne },
                new TableStruct { Headers = "Окл.", Binding = "Diff", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = isEROne },
                new TableStruct { Headers = "Факт, тыс.руб.", Binding = "FactCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "План, тыс.руб.", Binding = "PlanCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Окл., тыс.руб.", Binding = "DiffCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Окл., %", Binding = "DiffProc", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N1}", ColumnVisible = true, TotalRowTextVisible = true }
            };
            string capt = "Отклонение от расходных норм. Всего: " + table.Sum(m => m.DiffCost).ToString("N0") + ", тыс.руб.";
            return new(new(ChartType.UseCC, table, tstruct, capt));
        }
        private PieChart RetPieProduct(List<Person> dateSel, List<Person> ccSel, List<Person> erSel, string prSel)
        {
            if (ccSel.Count != 1) return null;
            List<FullFields> table = FullFields.RetUseFromPR(dateSel, ccSel, erSel, prSel);
            bool isEROne = erSel.Count == 1;

            List<TableStruct> tstruct = new()
            {
                new TableStruct { Headers = "Код", Binding = "IdER", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Ресурс", Binding = "ERName", ColWidth = 1.5, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = true },
                new TableStruct { Headers = "Разм.", Binding = "UnitName", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Код", Binding = "IdProduct", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = false },
                new TableStruct { Headers = "Продукт", Binding = "ProductName", ColWidth = 1.5, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Факт", Binding = "Fact", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = isEROne },
                new TableStruct { Headers = "План", Binding = "Plan", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = isEROne },
                new TableStruct { Headers = "Окл.", Binding = "Diff", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = isEROne },
                new TableStruct { Headers = "Факт, тыс.руб.", Binding = "FactCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "План, тыс.руб.", Binding = "PlanCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Окл., тыс.руб.", Binding = "DiffCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Окл., %", Binding = "DiffProc", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N1}", ColumnVisible = true, TotalRowTextVisible = true }
            };
            string capt = "Потребление энергоресурсов. Всего: " + table.Sum(m => m.FactCost).ToString("N0") + ", тыс.руб.";
            return new(new(ChartType.UsePR, table, tstruct, capt));
        }
        private ColumnChart RetColumnProduct(List<Person> dateSel, List<Person> ccSel, List<Person> erSel, string prSel)
        {
            if (ccSel.Count != 1) return null;
            List<FullFields> table = FullFields.RetUseFromPR(dateSel, ccSel, erSel, prSel);
            bool isEROne = erSel.Count == 1;
            List<TableStruct> tstruct = new()
            {
                new TableStruct { Headers = "Код", Binding = "IdER", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Ресурс", Binding = "ERName", ColWidth = 1.5, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = true },
                new TableStruct { Headers = "Разм.", Binding = "UnitName", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Код", Binding = "IdProduct", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = false },
                new TableStruct { Headers = "Продукт", Binding = "ProductName", ColWidth = 1.5, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Факт", Binding = "Fact", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = isEROne },
                new TableStruct { Headers = "План", Binding = "Plan", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = isEROne },
                new TableStruct { Headers = "Окл.", Binding = "Diff", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = isEROne },
                new TableStruct { Headers = "Факт, тыс.руб.", Binding = "FactCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "План, тыс.руб.", Binding = "PlanCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Окл., тыс.руб.", Binding = "DiffCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Окл., %", Binding = "DiffProc", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N1}", ColumnVisible = true, TotalRowTextVisible = true }
            };
            string capt = "Отклонение от расходных норм. Всего: " + table.Sum(m => m.DiffCost).ToString("N0") + ", тыс.руб.";
            return new(new(ChartType.UsePR, table, tstruct, capt));
        }

        private LineChart RetLineER(List<Person> dateSel, List<Person> ccSel, List<Person> erSel, string prSel)
        {
            int minCurrPeriod = dateSel.Min(m => m.Id);
            int maxCurrPeriod = dateSel.Max(m => m.Id);

            List<FullFields> table = FullFields.RetUsePeriodFromER(Global.RetDynamicTruePeriod(minCurrPeriod, maxCurrPeriod), ccSel, erSel, prSel);
            bool isEROne = erSel.Count == 1;
            List<TableStruct> tstruct = new()
            {
                new TableStruct { Headers = "Период", Binding = "PeriodStr", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Код", Binding = "IdER", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Ресурс", Binding = "ERName", ColWidth = 1.5, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = true },
                new TableStruct { Headers = "Разм.", Binding = "UnitName", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                new TableStruct { Headers = "Факт", Binding = "Fact", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = true },
                new TableStruct { Headers = "План", Binding = "Plan", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = true },
                new TableStruct { Headers = "Окл.", Binding = "Diff", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = true },
                new TableStruct { Headers = "Факт, тыс.руб.", Binding = "FactCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "План, тыс.руб.", Binding = "PlanCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Окл., тыс.руб.", Binding = "DiffCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "Окл., %", Binding = "DiffProc", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N1}", ColumnVisible = true, TotalRowTextVisible = true }
            };
            string capt = "Динамика использования энергоресурсов, тыс.руб.";
            return new(new(ChartType.UseER, table, tstruct, capt));
        }
        private ColumnChart RetERCompare(List<Person> dateSel, List<Person> ccSel, List<Person> erSel, string prSel)
        {
            if (erSel.Count == 1) return null;
            List<FullFields> table = FullFields.RetUseCompare(ChartDataType.ER, dateSel, ccSel, erSel, prSel);
            List<TableStruct> tstruct = new()
            {
                new TableStruct { Headers = "Код", Binding = "IdER", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = false },
                new TableStruct { Headers = "Энергоресурс", Binding = "ERName", ColWidth = 1.5, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = false },
                new TableStruct { Headers = "Разм.", Binding = "UnitName", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = false },
                new TableStruct { Headers = "d Нормативные", Binding = "dFactNormative", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = true, TotalRowTextVisible = false },
                new TableStruct { Headers = "d Лимитные", Binding = "dFactLimit", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = true, TotalRowTextVisible = false },
                new TableStruct { Headers = "d Всего", Binding = "dFact", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = true, TotalRowTextVisible = false },
                new TableStruct { Headers = "dНорматив., тыс.руб.", Binding = "dFactNormativeCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "dЛимитные, тыс.руб.", Binding = "dFactLimitCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "d Всего, тыс.руб.", Binding = "dFactCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true }
            };
            string capt = "Сравнение с базовым периодом. Отклонение всего: " + table.Sum(m => m.dFactCost).ToString("N0") + ", тыс.руб.";
            return new(new(ChartType.CompareER, table, tstruct, capt));
        }
        private ColumnChart RetCCCompare(List<Person> dateSel, List<Person> ccSel, List<Person> erSel, string prSel)
        {
            if (ccSel.Count == 1) return null;
            List<FullFields> table = FullFields.RetUseCompare(ChartDataType.CC, dateSel, ccSel, erSel, prSel);
            bool isEROne = erSel.Count == 1;
            List<TableStruct> tstruct = new()
            {
                new TableStruct { Headers = "Код", Binding = "IdCC", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = false },
                new TableStruct { Headers = "ЦЗ", Binding = "CCName", ColWidth = 1, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = false },
                new TableStruct { Headers = "d Нормативные", Binding = "dFactNormative", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = isEROne },
                new TableStruct { Headers = "d Лимитные", Binding = "dFactLimit", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = isEROne },
                new TableStruct { Headers = "d Всего", Binding = "dFact", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = isEROne },
                new TableStruct { Headers = "dНорматив., тыс.руб.", Binding = "dFactNormativeCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "dЛимитные, тыс.руб.", Binding = "dFactLimitCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                new TableStruct { Headers = "d Всего, тыс.руб.", Binding = "dFactCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true }
            };
            string capt = "Сравнение с базовым периодом. Отклонение всего: " + table.Sum(m => m.dFactCost).ToString("N0") + ", тыс.руб.";
            return new(new(ChartType.CompareCC, table, tstruct, capt));
        }
        private LineChart RetLineCompare(List<Person> dateSel, List<Person> ccSel, List<Person> erSel, string prSel)
        {
            int minCurrPeriod = dateSel.Min(m => m.Id);
            int maxCurrPeriod = dateSel.Max(m => m.Id);
            //if (minCurrPeriod < maxCurrPeriod)
            //{
                List<FullFields> table = FullFields.RetUseCompare(ChartDataType.Period, Global.RetDynamicTruePeriod(minCurrPeriod, maxCurrPeriod), ccSel, erSel, prSel);
                bool isEROne = erSel.Count == 1;
                List<TableStruct> tstruct = new()
                {
                    new TableStruct { Headers = "Период", Binding = "PeriodStr", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                    new TableStruct { Headers = "Код", Binding = "IdER", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                    new TableStruct { Headers = "Энергоресурс", Binding = "ERName", ColWidth = 1.5, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                    new TableStruct { Headers = "Разм.", Binding = "UnitName", ColWidth = 0.7, TextAligment = TextAlignment.Left, NumericFormat = "{0:N0}", ColumnVisible = isEROne, TotalRowTextVisible = false },
                    new TableStruct { Headers = "d Нормативные", Binding = "dFactNormative", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = isEROne },
                    new TableStruct { Headers = "d Лимитные", Binding = "dFactLimit", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = isEROne },
                    new TableStruct { Headers = "d Всего", Binding = "dFact", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N2}", ColumnVisible = isEROne, TotalRowTextVisible = isEROne },
                    new TableStruct { Headers = "dНорматив., тыс.руб.", Binding = "dFactNormativeCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                    new TableStruct { Headers = "dЛимитные, тыс.руб.", Binding = "dFactLimitCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true },
                    new TableStruct { Headers = "d Всего, тыс.руб.", Binding = "dFactCost", ColWidth = 1, TextAligment = TextAlignment.Right, NumericFormat = "{0:N0}", ColumnVisible = true, TotalRowTextVisible = true }
                };
                string capt = "Сравнение с базовым периодом, тыс.руб.";
                return new(new(ChartType.CompareER, table, tstruct, capt));
            //}
            //return null;
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
        private void TypesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ListBox)sender;
            var textBlock = (TextBlock)(((ListBoxItem)item.SelectedItem).Content);
            PopupTypesCaption.Text = textBlock.Text;
            FilterPR = textBlock.Text;
            FiltersIsChanged = true;
            PopupBox.IsPopupOpen = false;
        }

    }
}
