using AttendanceSystem.Models.Entities;
using Microsoft.VisualBasic;
using System;

namespace AttendanceSystem.Models.Entities
{
    public class MonthlyReportItem
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int PresentDays { get; set; }
        public int LeaveDays { get; set; }
        public int AbsentDays { get; set; }
        public decimal TotalHours { get; set; }
        public List<DailyReportItem> DailyBreakdown { get; set; } = new();
    }


        public class DailyReportItem
        {
            public DateTime Date { get; set; }

            public string DayName => Date.ToString("dddd");

            public string Status { get; set; }

            public decimal Hours { get; set; }

            public DateTime? CheckInTime { get; set; }

            public DateTime? CheckOutTime { get; set; }

            public string LeaveType { get; set; }

            public TimeSpan? LeaveFromTime { get; set; }

            public TimeSpan? LeaveToTime { get; set; }

            public decimal? LeaveHours { get; set; }

            public string HalfDaySession { get; set; }

            public string LeaveReason { get; set; }

            public string RequestedBy { get; set; }
        }
    

}
