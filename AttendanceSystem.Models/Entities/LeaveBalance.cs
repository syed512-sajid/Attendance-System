using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystem.Models.Entities
{
    public class LeaveBalance
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public int Year { get; set; }
        public int TotalLeaves { get; set; }
        public decimal UsedLeaves { get; set; }
        public decimal RemainingLeaves => TotalLeaves - UsedLeaves;
    }
}