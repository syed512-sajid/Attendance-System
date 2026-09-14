using AttendanceSystem.Contracts.Repository;
using AttendanceSystem.Contracts.Services;
using AttendanceSystem.Models.Entities;

namespace AttendanceSystem.Services
{
    public class LeaveBalanceService : ILeaveBalanceService
    {
        private readonly ILeaveBalanceRepository _leaveBalanceRepository;

        public LeaveBalanceService(ILeaveBalanceRepository leaveBalanceRepository)
        {
            _leaveBalanceRepository = leaveBalanceRepository;
        }

        public void AssignBalance(int employeeId, int year, int totalLeaves)
        {
            _leaveBalanceRepository.AssignBalance(employeeId, year, totalLeaves);
        }

        public LeaveBalance GetByEmployee(int employeeId, int year)
        {
            return _leaveBalanceRepository.GetByEmployee(employeeId, year);
        }

        public List<LeaveBalance> GetAll(int year)
        {
            return _leaveBalanceRepository.GetAll(year);
        }
    }
}