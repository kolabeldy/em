using em.Models;
using em.ServicesPages;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace em.ServicesPages
{
    /// <summary>
    /// Логика взаимодействия для TablePage.xaml
    /// </summary>
    public partial class ColumnChart : UserControl, IChart
    {
        private ColumnViewModel model;

        public ColumnChart(ColumnViewModel model)
        {
            this.model = model;
            InitializeComponent();
            model.myChart = this;
            DataContext = model;
        }
    }
}
