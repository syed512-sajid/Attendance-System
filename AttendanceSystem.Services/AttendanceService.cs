using AttendanceSystem.Contracts.Repository;
using AttendanceSystem.Contracts.Services;
using AttendanceSystem.Models.Entities;

namespace AttendanceSystem.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _repo;

        public AttendanceService(IAttendanceRepository repo)
        {
            _repo = repo;
        }

        public AttendanceRecord GetTodayStatus(int employeeId)
            => _repo.GetToday(employeeId, DateTime.Today);

        public (bool Success, string Message) CheckIn(int employeeId)
        {
            var existing = _repo.GetToday(employeeId, DateTime.Today);

            if (existing != null)
                return (false, "You have already checked in today.");

            _repo.CheckIn(employeeId, DateTime.Today, DateTime.Now);

            return (true, "Check-in successful.");
        }

        public (bool Success, string Message) CheckOut(int employeeId)
        {
            var existing = _repo.GetToday(employeeId, DateTime.Today);

            if (existing == null)
                return (false, "No check-in record was found for today.");

            if (existing.CheckOutTime != null)
                return (false, "You have already checked out today.");

            _repo.CheckOut(employeeId, DateTime.Today, DateTime.Now);

            return (true, "Check-out successful.");
        }

        public List<AttendanceRecord> GetByEmployeeMonth(
            int employeeId,
            int year,
            int month)
            => _repo.GetByEmployeeMonth(employeeId, year, month);

        public List<AttendanceRecord> GetAllMonth(
            int year,
            int month)
            => _repo.GetAllMonth(year, month);

        // Auto-checkout: closes all open attendance records
        // at the configured fixed checkout time.
        //public Task RunAutoCheckoutAsync(
        //    DateTime attendanceDate,
        //    DateTime checkoutTime)
        //{
        //    var openRecords = _repo.GetOpenForAutoCheckout(attendanceDate);

        //    foreach (var rec in openRecords)
        //    {
        //        _repo.MarkAutoCheckout(
        //            rec.AttendanceID,
        //            checkoutTime);
        //    }

        //    return Task.CompletedTask;
        //}
        // Aaj tak ke sab open (bina-checkout) records check karta hai - chahe app us waqt
        // band hi kyun na thi (agle din on ki ho ya employee login ke waqt bhi yehi chalta hai).
        // Har record apni hi date ke 10PM (config) par close hota hai, lekin sirf tab jab
        // us record ka scheduled time guzar chuka ho (aaj ka record raat 10 se pehle skip hota hai).
        public Task RunAutoCheckoutAsync(int hour, int minute)
        {
            var openRecords = _repo.GetOpenUpToDate(DateTime.Today);
            var now = DateTime.Now;

            foreach (var rec in openRecords)
            {
                var scheduledCheckout = rec.AttendanceDate.Date.AddHours(hour).AddMinutes(minute);
                if (now >= scheduledCheckout)
                {
                    _repo.MarkAutoCheckout(rec.AttendanceID, scheduledCheckout);
                }
            }
            return Task.CompletedTask;
        }
    }
}