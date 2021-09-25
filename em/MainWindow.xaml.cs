using em.DBAccess;
using em.MenuPages;
using em.Models;
using em.ViewModel;
using MaterialDesignThemes.Wpf;
using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace em
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Global.InitMyPath();
            Period.InitPeriods();

            InitializeComponent();

            var menuMonitoring = new List<SubItem>();
            menuMonitoring.Add(new SubItem("Инфопанель", screen: null, idFunc: "DashboardShow"));
            menuMonitoring.Add(new SubItem("Месячные отчёты", MonthMonitor.GetInstance()));
            menuMonitoring.Add(new SubItem("Суточные данные", DayMonitor.GetInstance()));
            menuMonitoring.Add(new SubItem("Потери энергоресурсов", LossesMonitor.GetInstance()));
            var item1 = new ItemMenu("Мониторинг", menuMonitoring, PackIconKind.MonitorDashboard);

            var menuImportToDB = new List<SubItem>();
            menuImportToDB.Add(new SubItem("Месячные отчёты", screen: null, idFunc: "AddNewMonth"));
            menuImportToDB.Add(new SubItem("Суточные данные", screen: null, idFunc: "AddNewDay"));
            menuImportToDB.Add(new SubItem("Фыктические потери", screen: null, idFunc: "AddNewLosses"));
            menuImportToDB.Add(new SubItem("Расходные нормы", screen: null, idFunc: "AddNewNormatives"));
            var item2 = new ItemMenu("Импорт данных", menuImportToDB, PackIconKind.Import);

            var menuReports = new List<SubItem>();
            menuReports.Add(new SubItem("Месячный отчёт", screen: null, idFunc: "ReportMonthShow"));
            menuReports.Add(new SubItem("Пофакторный анализ", screen: null, idFunc: "ReportFactorAnalysisShow"));
            var item3 = new ItemMenu("Отчёты", menuReports, PackIconKind.FileReport);

            var menuAdmin = new List<SubItem>();
            menuAdmin.Add(new SubItem("Синхронизация", screen: null, idFunc: "SynchronizeDB"));
            menuAdmin.Add(new SubItem("Резервирование", screen: null, idFunc: "SaveDB"));
            menuAdmin.Add(new SubItem("Восстановление", screen: null, idFunc: "RestoreDB"));
            menuAdmin.Add(new SubItem("Удалить последние", screen: null, idFunc: "DeleteLastMonthUse"));
            var item4 = new ItemMenu("Сервис БД", menuAdmin, PackIconKind.DatabaseSettings);

            var menuReferences = new List<SubItem>();
            menuReferences.Add(new SubItem("Энергоресурсы", screen: null, idFunc: "ERShow"));
            menuReferences.Add(new SubItem("Центры затрат", screen: null, idFunc: "CCShow"));
            menuReferences.Add(new SubItem("Тарифы", screen: null, idFunc: "TariffsShow"));
            menuReferences.Add(new SubItem("Нормативные потери", screen: null, idFunc: "NormLossShow"));
            var item5 = new ItemMenu("Справочники", menuReferences, PackIconKind.Book);

            var menuSettings = new List<SubItem>();
            menuSettings.Add(new SubItem("Авторизация"));
            menuSettings.Add(new SubItem("О программе", screen: null, idFunc: "About"));

            var item6 = new ItemMenu("Настройки", menuSettings, PackIconKind.Settings);


            Menu.Children.Add(new UserControlMenuItem(item1, this));
            Menu.Children.Add(new UserControlMenuItem(item2, this));
            Menu.Children.Add(new UserControlMenuItem(item3, this));
            Menu.Children.Add(new UserControlMenuItem(item4, this));
            Menu.Children.Add(new UserControlMenuItem(item5, this));
            Menu.Children.Add(new UserControlMenuItem(item6, this));
        }

        internal void SwitchScreen(object sender, string idFunc = null)
        {
            bool rez;
            if (idFunc != null)
            {
                switch (idFunc)
                {
                    case "DashboardShow":
                        Dashboard dashboard = Dashboard.GetInstance();
                        dashboard.ShowDialog();
                        break;
                    case "ReportMonthShow":
                        rez = Report.ReportMonthShow();
                        break;
                    case "ReportFactorAnalysisShow":
                        int period = 202101;
                        int cc = 56;
                        Report.UniversalForm(period, cc, 1);
                        break;

                    case "ERShow":
                        new EREdit().ShowDialog();
                        break;

                    case "CCShow":
                        new CCEdit().ShowDialog();
                        break;
                    case "TariffsShow":
                        new TariffsEdit().ShowDialog();
                        break;
                    case "NormLossShow":
                        new NormLossEdit().ShowDialog();
                        break;

                    case "AddNewMonth":
                        rez = ExcelImport.ImportMothDataFromExcel();
                        if (rez == true)
                        {
                            Period.InitPeriods();
                            MonthMonitor.isChange = true;
                            MonthMonitor.GetInstance().FilterInit(isDateOnly: true);
                            MonthMonitor.GetInstance().FormRefresh();
                        }
                        break;
                    case "AddNewDay":
                        rez = ExcelImport.ImportDayDataFromExcel();
                        if (rez == true)
                        {
                            Period.InitPeriods();
                            DayMonitor.isChange = true;
                            DayMonitor.GetInstance().FilterInit(isDateOnly: false);
                            DayMonitor.GetInstance().FormRefresh();
                        }
                        break;
                    case "AddNewNormatives":
                        List<TempUseERTable> exportList = new();
                        exportList = ExcelImport.ImportNormsFromExcel();
                        if (exportList != null)
                        {
                            Norm.DeleteAll();
                            ERNorm.Clear();
                            Norm.InsertRec(exportList);
                            ERNorm.InsertRec();

                            MonthMonitor.isChange = true;
                            MonthMonitor.GetInstance().FormRefresh();
                            DayMonitor.isChange = true;
                            DayMonitor.GetInstance().FormRefresh();
                            LossesMonitor.GetInstance().FormRefresh();
                        }
                        break;
                    case "AddNewLosses":
                        rez = ExcelImport.ImportLossesDataFromExcel();
                        if (rez == true)
                        {
                            Period.InitPeriods();
                            LossesMonitor.GetInstance().FilterInit(isDateOnly: true);
                            LossesMonitor.GetInstance().FormRefresh();
                        }
                        break;
                    case "DeleteLastMonthUse":
                        rez = DataAccess.DeleteLastMonthUseAndLosses();
                        if (rez == true)
                        {
                            Period.InitPeriods();
                            MonthMonitor.isChange = true;
                            MonthMonitor.GetInstance().FilterInit(isDateOnly: true);
                            MonthMonitor.GetInstance().FormRefresh();
                            LossesMonitor.GetInstance().FilterInit(isDateOnly: true);
                            LossesMonitor.GetInstance().FormRefresh();
                        }
                        break;
                    case "SaveDB":
                        rez = DataAccess.SaveDB();
                        if (rez == true)
                        {
                        }
                        break;
                    case "RestoreDB":
                        rez = DataAccess.RestoreDB();
                        if (rez == true)
                        {
                        }
                        break;
                    case "SynchronizeDB":
                        DataAccess.SynchronizeDB();
                        break;

                    case "About":
                        new About().ShowDialog();
                        break;

                }

            }
            else
            {
                var screen = ((UserControl)sender);
                if (screen != null)
                {
                    StackPanelMain.Children.Clear();
                    StackPanelMain.Children.Add(screen);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UserControlMenuItem currItem;
            //StackPanelMain.Children.Clear();
            //StackPanelMain.Children.Add(MonthMonitor.GetInstance());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Dashboard.GetInstance().Close();
        }
    }
}
