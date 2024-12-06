using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoStartApplication.Models
{
    public class PunchRecordRequestModel
    {
        public PunchRecordRequestModel()
        {
            List<AttendanceRecordModel> attandanceLogs = new List<AttendanceRecordModel>();
        }
        public List<AttendanceRecordModel> attandanceLogs { get; set; }
    }
    
}
