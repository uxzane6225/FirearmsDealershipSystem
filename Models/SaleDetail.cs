using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirearmsDealershipSystem.Models
{
    public class SaleDetail : Sale
    {
        public int SaleDetailID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
    }
}
