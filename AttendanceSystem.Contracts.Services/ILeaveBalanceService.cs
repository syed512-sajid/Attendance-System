using AttendanceSystem.Models.Entities;

namespace AttendanceSystem.Contracts.Services
{
    public interface ILeaveBalanceService
    {
        void AssignBalance(int employeeId, int year, int totalLeaves);
        LeaveBalance GetByEmployee(int employeeId, int year);
        List<LeaveBalance> GetAll(int year);
    }
}