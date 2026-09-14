using AttendanceSystem.Models.Entities;

namespace AttendanceSystem.Contracts.Services
{
    public interface IAttendanceService
    {
        AttendanceRecord GetTodayStatus(int employeeId);
        (bool Success, string Message) CheckIn(int employeeId);
        (bool Success, string Message) CheckOut(int employeeId);
        List<AttendanceRecord> GetByEmployeeMonth(int employeeId, int year, int month);
        List<AttendanceRecord> GetAllMonth(int year, int month);
        //Task RunAutoCheckoutAsync(DateTime attendanceDate, DateTime checkoutTime);
        Task RunAutoCheckoutAsync(int hour, int minute);
    }
}
