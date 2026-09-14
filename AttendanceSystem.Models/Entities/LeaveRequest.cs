namespace AttendanceSystem.Models.Entities
{
    public class LeaveRequest
    {
        public int LeaveID { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string RequestedBy { get; set; }
        public int CreatedBy { get; set; }
        public int? ActionBy { get; set; }
        public string ActionByName { get; set; }
        public DateTime? ActionDate { get; set; }
        public DateTime CreatedDate { get; set; }

        public string LeaveType { get; set; } = "Full Day";
        public string HalfDaySession { get; set; }
        public decimal? Hours { get; set; }
        public TimeSpan? FromTime { get; set; }
        public TimeSpan? ToTime { get; set; }

        public int TotalDays => (ToDate.Date - FromDate.Date).Days + 1;

        public decimal DaysForBalance => LeaveType switch
        {
            "Half Day" => 0.5m,
            "Short Leave" => 0m,
            _ => TotalDays
        };
    }
}