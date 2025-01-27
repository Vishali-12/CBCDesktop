using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoStartApplication.Models
{
    public class ExcetionViewModel
    {
        public string Message { get; set; }
        public string InnerException { get; set; }
        public string StackTrace { get; set; }
        public DateTime OccuredOn { get; set; }
    }
}
