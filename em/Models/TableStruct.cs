using System.Windows;

namespace em.Models
{
    public class TableStruct
    {
        public string Headers { get; set; }
        public string Binding { get; set; }
        public double ColWidth { get; set; }
        public TextAlignment TextAligment { get; set; }
        public string NumericFormat { get; set; }
        public bool ColumnVisible { get; set; }
        public bool TotalRowTextVisible { get; set; }

    }
}
