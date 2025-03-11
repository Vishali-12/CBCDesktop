using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoStartApplication.Models
{
    public class UpdateEmployeeStatusRequestModel
    {
        public int id { get; set; }
        public int out_machine_status { get; set; }
        public int in_machine_status { get; set; }
    }
}
