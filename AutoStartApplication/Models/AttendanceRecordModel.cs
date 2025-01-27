using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoStartApplication.Models
{
    public class AttendanceRecordModel
    {
        public string EmployeeId { get; set; }
        public string Date { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
    }
}
