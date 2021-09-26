using em.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace em.MenuPages
{
    /// <summary>
    /// Логика взаимодействия для PageTariffsEdit.xaml
    /// </summary>
    public partial class ReportMonth : Window, INotifyPropertyChanged
    {
        public class PeriodId
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public List<PeriodId> Months { get; set; }
        public List<PeriodId> Years { get; set; }


        private PeriodId monthStart;
        public PeriodId MonthStart
        {
            get => monthStart;
            set
            {
                monthStart = value;
                OnPropertyChanged("MonthStart");
            }
        }
        private PeriodId yearStart;
        public PeriodId YearStart
        {
            get => yearStart;
            set
            {
                yearStart = value;
                OnPropertyChanged("YearStart");
            }
        }

        public int beginPeriod;

        public ReportMonth()
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

            MonthStart = Months[Period.LastMonth - 1];
            YearStart = Years[Years.Count - 1];

            InitializeComponent();
            DataContext = this;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Indicator.Visibility = Visibility.Visible;
            beginPeriod = Convert.ToInt32(YearStart.Name) * 100 + MonthStart.Id;
            Report.UniversalForm(beginPeriod);
            Close();
        }
    }
}
