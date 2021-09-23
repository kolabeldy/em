using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace em.Models
{
    public class NormLossTable
    {
        public int IdER { get; set; }
        public string ERName { get; set; }
        public double LossKv1 { get; set; }
        public double LossKv2 { get; set; }
        public double LossKv3 { get; set; }
        public double LossKv4 { get; set; }
    }
}
