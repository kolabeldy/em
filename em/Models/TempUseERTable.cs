using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace em.Models
{
    public class TempUseERTable
    {
        public string Period { get; set; }
        public string IdERProducer { get; set; }
        public string IdER { get; set; }
        public string IdOrg { get; set; }
        public string IdCC { get; set; }
        public string IdProduct { get; set; }
        public string ProductName { get; set; }
        public string ERFact { get; set; }
        public string ERPlan { get; set; }
        public string ERNormFact { get; set; }
        public string ERNormPlan { get; set; }
        public string Produced { get; set; }
        public string NormAverage { get; set; }
        public string NormWinter { get; set; }
        public string NormSummer { get; set; }
        public string UnitName { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }

    }
}
