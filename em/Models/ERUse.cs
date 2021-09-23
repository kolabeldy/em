using System.Collections.Generic;
using System.Linq;

namespace em.Models
{
    public class ERUse
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Period { get; set; }
        public int Kvart { get; set; }
        public int Season { get; set; }
        public int IdOrg { get; set; }
        public int IdCC { get; set; }
        public int IdProduct { get; set; }
        public string ProductName { get; set; }
        public double Produced { get; set; }
        public int IdER { get; set; }
        public int IdGroup { get; set; }
        public int IdERProducer { get; set; }
        public double ERFact { get; set; }
        public double ERPlan { get; set; }
        public double ERPlanCorrected { get; set; }
        public double ERNormFact { get; set; }
        public double ERNormPlan { get; set; }
        public double SumFact { get; set; }
        public double SumPlan { get; set; }
        public bool IsNorm { get; set; }
        public bool IsTechnology { get; set; }


    }
}
