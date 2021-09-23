using System.Windows.Controls;

namespace em.FiltersSections
{
    public partial class FilterSection : UserControl
    {
        public FilterSectionViewModel model;
        public FilterSection(FilterSectionViewModel model)
        {
            this.model = model;
            InitializeComponent();
            DataContext = model;
        }

    }
}
