using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirearmsDealershipSystem.Models
{
    public class Session
    {
        public static Employee CurrentEmployee { get; set; }
        public static bool isLoggedIn => CurrentEmployee != null;
    }
}
