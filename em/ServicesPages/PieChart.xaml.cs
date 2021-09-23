using System.Windows.Controls;

namespace em.ServicesPages
{
    public partial class PieChart : UserControl, IChart
    {
        private PieViewModel model;

        public PieChart(PieViewModel model)
        {
            this.model = model;
            InitializeComponent();
            model.myChart = this;
            DataContext = model;
        }

    }
}
