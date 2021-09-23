using Kant.Wpf.Controls.Chart;
using System.Collections.Generic;
using System.Windows.Controls;

namespace em.MyCharts
{
    /// <summary>
    /// Логика взаимодействия для TablePage.xaml
    /// </summary>
    public partial class MySankeyChart : UserControl
    {
        MySankeyModel model;
        public bool Result { get; private set; }

        public MySankeyChart(List<SankeyData> cdata, string title = "")
        {
            model = new MySankeyModel(cdata);
            model.ChartFill();
            DataContext = model;
            InitializeComponent();
            Header.Text = title;
            diagram.FirstAndLastLabelPosition = FirstAndLastLabelPosition.Inward;
            //Loaded += (s, e) =>
            //{
            //   //diagram.DiagramGrid.Margin = new Thickness(50);
            //};
            Result = true;
        }
    }
}
