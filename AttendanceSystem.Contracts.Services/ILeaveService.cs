using AttendanceSystem.Models.Entities;

namespace AttendanceSystem.Contracts.Services
{
    public interface ILeaveService
    {
        (bool Success, string Message) RequestLeave(int employeeId, DateTime fromDate, DateTime toDate, string reason, string leaveType, string halfDaySession, TimeSpan? fromTime, TimeSpan? toTime);
        void AssignLeave(int employeeId, DateTime fromDate, DateTime toDate, string reason, int adminId, string leaveType, string halfDaySession, decimal? hours, TimeSpan? fromTime, TimeSpan? toTime);
        void Approve(int leaveId, int adminId);
        void Reject(int leaveId, int adminId);
        void Cancel(int leaveId, int employeeId);
        List<LeaveRequest> GetAll();
        List<LeaveRequest> GetByEmployee(int employeeId);
        void AssignLeaveBalance(int employeeId, int year, int totalLeaves);
        LeaveBalance GetBalance(int employeeId, int year);
        List<LeaveBalance> GetAllBalances(int year);
    }
}