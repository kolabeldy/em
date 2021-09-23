using System.Windows.Controls;

namespace em.ViewModel
{
    public class SubItem
    {
        public SubItem(string name, UserControl screen = null, string idFunc = null)
        {
            Name = name;
            Screen = screen;
            IdFunc = idFunc;
        }
        public string Name { get; private set; }
        public UserControl Screen { get; private set; }
        public string IdFunc { get; set; }
    }
}