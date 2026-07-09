using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirearmsDealershipSystem.Models
{
    public class Sale : Customer
    {
        public int SaleID { get; set; }
        public string SaleDate { get; set; }
        public double TotalAmount { get; set; }
    }
}
