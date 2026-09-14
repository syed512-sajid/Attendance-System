using AttendanceSystem.Models.Entities;

namespace AttendanceSystem.Contracts.Repository
{
    public interface ILeaveRepository
    {
        void Save(LeaveRequest leave);
        void UpdateStatus(int leaveId, string status, int actionBy);
        void Cancel(int leaveId, int employeeId);
        List<LeaveRequest> GetAll();
        List<LeaveRequest> GetByEmployee(int employeeId);
        List<LeaveRequest> GetApprovedForMonth(int year, int month);
        List<LeaveRequest> GetAllForMonth(int year, int month);
    }
}