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
    public partial class DayMonitor : UserControl, INotifyPropertyChanged
    {
        private static DayMonitor instance;
        public static DayMonitor GetInstance()
        {
            if (instance == null)
                instance = new DayMonitor();
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

        private DayMonitor()
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
            modelCC = new FilterSectionCostCentersViewModel("ЦЗ:", TreeInitType.All);
            modelCC.onChange += FilterCCOnChangeHandler;
            FilterCC = new FilterSection(modelCC);
            modelCC.Init();
            FilterCCPanel.Content = FilterCC;
            //modelER.PopupHorisontalOffset = 65;
            modelER = new FilterSectionEnergyResourcesViewModel("Ресурсы:", TreeInitType.All);
            modelER.onChange += FilterEROnChangeHandler;
            FilterER = new FilterSection(modelER);
            modelER.Init();
            FilterERPanel.Content = FilterER;
            //modelER.PopupHorisontalOffset = 65;
            FilterDatePanel.Text = Period.LastPeriodDay;
            //PRSectionFilterActivate(isMonth);
        }
        protected void FilterEROnChangeHandler()
        {
            FiltersIsChanged = true;
            filterEnergyResourcesList = modelER.FilterList;
        }
        protected void FilterCCOnChangeHandler()
        {
            FiltersIsChanged = true;
            filterCostCentersList = modelCC.FilterList;
        }

        public void FormRefresh()
        {
            if (isChange)
            {
                Dashboard.Children.Clear();
                AddChart(RetPieER(filterCostCentersList, filterEnergyResourcesList), ChartWidthType.Slim);
                AddChart(RetColumnER(filterCostCentersList, filterEnergyResourcesList), ChartWidthType.Slim);
                AddChart(RetPieCC(filterCostCentersList, filterEnergyResourcesList), ChartWidthType.Slim);
                AddChart(RetColumnCC(filterCostCentersList, filterEnergyResourcesList), ChartWidthType.Slim);
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

        private PieChart RetPieER(List<Person> ccSel, List<Person> erSel)
        {
            if (erSel.Count == 1) return null;
            List<FullFields> table = FullFields.RetUseDayFromER(ccSel, erSel);

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
            string capt = "Потребление энергоресурсов. Всего: " + table.Sum(m => m.FactCost).ToString("N0") + ", тыс.руб.";
            return new(new(ChartType.UseER, table, tstruct, capt));
        }
        private ColumnChart RetColumnER(List<Person> ccSel, List<Person> erSel)
        {
            if (erSel.Count == 1) return null;
            List<FullFields> table = FullFields.RetUseDayFromER(filterCostCentersList, filterEnergyResourcesList);
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
        private PieChart RetPieCC(List<Person> ccSel, List<Person> erSel)
        {
            if (ccSel.Count == 1) return null;
            List<FullFields> table = FullFields.RetUseDayFromCC(filterCostCentersList, filterEnergyResourcesList);

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
            string capt = "Потребление энергоресурсов. Всего: " + table.Sum(m => m.FactCost).ToString("N0") + ", тыс.руб.";
            return new(new(ChartType.UseCC, table, tstruct, capt));
        }
        private ColumnChart RetColumnCC(List<Person> ccSel, List<Person> erSel)
        {
            if (ccSel.Count == 1) return null;
            {
                List<FullFields> table = FullFields.RetUseDayFromCC(filterCostCentersList, filterEnergyResourcesList);
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
