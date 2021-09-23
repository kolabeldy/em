using System.Windows;
using System.Windows.Controls;

namespace em.MenuPages
{
    /// <summary>
    /// Логика взаимодействия для PageTariffsEdit.xaml
    /// </summary>
    public partial class ReportFactorAnalysis : Window
    {
        public ReportFactorAnalysis()
        {
            DataContext = this;
            InitializeComponent();
        }


        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
