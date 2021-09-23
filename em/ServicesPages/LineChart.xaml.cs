using em.Helpers;
using em.Models;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace em.ServicesPages
{
    /// <summary>
    /// Логика взаимодействия для TablePage.xaml
    /// </summary>
    public partial class LineChart : UserControl, IChart
    {
        private LineViewModel model;

        public LineChart(LineViewModel model)
        {
            this.model = model;
            InitializeComponent();
            model.myChart = this;
            DataContext = model;
        }

    }
}
