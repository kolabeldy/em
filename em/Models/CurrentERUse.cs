using System.Collections.Generic;
using System.Linq;

namespace em.Models
{
    public class CurrentERUse
    {
        public int Id { get; set; }
        public string Period { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int IdOrg { get; set; }
        public int IdCC { get; set; }
        public int IdER { get; set; }
        public double ERPlan { get; set; }
        public double ERFact { get; set; }
        public int Season { get; set; }
    }
}
