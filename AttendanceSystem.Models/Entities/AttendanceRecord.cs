namespace AttendanceSystem.Models.Entities
{
    public class AttendanceRecord
    {
        public int AttendanceID { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string Status { get; set; } // Present / Leave / Absent
        public decimal? TotalHours { get; set; }
        public bool IsAutoCheckout { get; set; }
    }
}
