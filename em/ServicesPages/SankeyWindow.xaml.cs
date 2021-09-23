using em.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace em.ServicesPages
{
    /// <summary>
    /// Логика взаимодействия для TablePage.xaml
    /// </summary>
    public partial class SankeyWindow : Window
    {
        SankeyModel model;
        public bool Result { get; private set; }

        public SankeyWindow(List<DataChart> cdata, string ctype, string tittle = "")
        {
            model = new SankeyModel(cdata);
            model.cdata = cdata;
            model.ctype = ctype;
            model.ChartFill();
            DataContext = model;
            InitializeComponent();
            ChartTittle.Text = tittle;
            diagram.FirstAndLastLabelPosition = Kant.Wpf.Controls.Chart.FirstAndLastLabelPosition.Outward;
            Loaded += (s, e) =>
            {
               //diagram.DiagramGrid.Margin = new Thickness(50);
            };
            Result = true;
        }
        private void btnScreen_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog() == true)
            {
                dialog.PrintVisual(this.DiagramArea, "Диаграмма");
                this.Show();
            }
            else
            {
                this.Show();
            }
        }
    }
}
