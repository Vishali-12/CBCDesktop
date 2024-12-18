using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoStartApplication.Models
{
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string employee_no { get; set; }
        public int in_machine_status { get; set; }
        public int out_machine_status { get; set; }
    }
}
