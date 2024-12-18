using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoStartApplication.Models
{
    public class GetEmployeesResponseModel
    {
        public List<User> data { get; set; }
        public int status { get; set; }
    }
}
