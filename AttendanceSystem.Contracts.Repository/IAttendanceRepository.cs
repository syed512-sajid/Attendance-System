using AttendanceSystem.Models.Entities;

namespace AttendanceSystem.Contracts.Repository
{
    public interface IAttendanceRepository
    {
        void CheckIn(int employeeId, DateTime attendanceDate, DateTime checkInTime);
        void CheckOut(int employeeId, DateTime attendanceDate, DateTime checkOutTime);
        AttendanceRecord GetToday(int employeeId, DateTime attendanceDate);
        List<AttendanceRecord> GetByEmployeeMonth(int employeeId, int year, int month);
        List<AttendanceRecord> GetAllMonth(int year, int month);
        //List<AttendanceRecord> GetOpenForAutoCheckout(DateTime attendanceDate);
        List<AttendanceRecord> GetOpenUpToDate(DateTime uptoDate);
        void MarkAutoCheckout(int attendanceId, DateTime checkOutTime);
    }
}
