using AttendanceSystem.Contracts.Repository;
using AttendanceSystem.Contracts.Services;
using AttendanceSystem.Models.Entities;

namespace AttendanceSystem.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _repo;
        private readonly ILeaveBalanceRepository _balanceRepo;

        public LeaveService(ILeaveRepository repo, ILeaveBalanceRepository balanceRepo)
        {
            _repo = repo;
            _balanceRepo = balanceRepo;
        }

        public (bool Success, string Message) RequestLeave(int employeeId, DateTime fromDate, DateTime toDate, string reason, string leaveType, string halfDaySession, TimeSpan? fromTime, TimeSpan? toTime)
        {
            decimal? hours = null;

            var validation = ValidateType(fromDate, toDate, leaveType, halfDaySession, fromTime, toTime, out hours);
            if (!validation.Success) return validation;

            if (HasOverlap(employeeId, fromDate, toDate))
                return (false, "You already have a leave request that overlaps with these dates.");

            var request = new LeaveRequest
            {
                EmployeeID = employeeId,
                FromDate = fromDate,
                ToDate = toDate,
                LeaveType = leaveType,
                HalfDaySession = leaveType == "Half Day" ? halfDaySession : null,
                Hours = leaveType == "Short Leave" ? hours : null,
                FromTime = leaveType == "Short Leave" ? fromTime : null,
                ToTime = leaveType == "Short Leave" ? toTime : null
            };

            if (request.DaysForBalance > 0)
            {
                var balance = _balanceRepo.GetByEmployee(employeeId, fromDate.Year);
                if (request.DaysForBalance > balance.RemainingLeaves)
                    return (false, $"Insufficient leave balance. Remaining: {balance.RemainingLeaves:0.##}, Requested: {request.DaysForBalance:0.##}.");
            }

            request.Reason = reason;
            request.RequestedBy = "Employee";
            request.CreatedBy = employeeId;
            request.Status = "Pending";

            _repo.Save(request);
            return (true, "Leave request submitted successfully.");
        }

        public void AssignLeave(int employeeId, DateTime fromDate, DateTime toDate, string reason, int adminId, string leaveType, string halfDaySession, decimal? hours ,TimeSpan? fromTime, TimeSpan? toTime)
        {
            _repo.Save(new LeaveRequest
            {
                EmployeeID = employeeId,
                FromDate = fromDate,
                ToDate = toDate,
                Reason = reason,
                RequestedBy = "Admin",
                CreatedBy = adminId,
                Status = "Approved",
                LeaveType = leaveType,
                HalfDaySession = leaveType == "Half Day" ? halfDaySession : null,
                Hours = leaveType == "Short Leave" ? hours : null,
                FromTime = leaveType == "Short Leave" ? fromTime : null,
                ToTime = leaveType == "Short Leave" ? toTime : null
            });
        }

        public void Approve(int leaveId, int adminId) => _repo.UpdateStatus(leaveId, "Approved", adminId);
        public void Reject(int leaveId, int adminId) => _repo.UpdateStatus(leaveId, "Rejected", adminId);
        public void Cancel(int leaveId, int employeeId) => _repo.Cancel(leaveId, employeeId);
        public List<LeaveRequest> GetAll() => _repo.GetAll();
        public List<LeaveRequest> GetByEmployee(int employeeId) => _repo.GetByEmployee(employeeId);
        public void AssignLeaveBalance(int employeeId, int year, int totalLeaves) => _balanceRepo.AssignBalance(employeeId, year, totalLeaves);
        public LeaveBalance GetBalance(int employeeId, int year) => _balanceRepo.GetByEmployee(employeeId, year);
        public List<LeaveBalance> GetAllBalances(int year) => _balanceRepo.GetAll(year);

        private static (bool Success, string Message) ValidateType(DateTime fromDate, DateTime toDate, string leaveType, string halfDaySession, TimeSpan? fromTime, TimeSpan? toTime, out decimal? hours)
        {
            hours = null;

            if (toDate.Date < fromDate.Date) return (false, "To Date cannot be before From Date.");

            if (leaveType == "Half Day")
            {
                if (fromDate.Date != toDate.Date) return (false, "Half Day leave must be for a single date.");
                if (string.IsNullOrWhiteSpace(halfDaySession)) return (false, "Please select Morning or Afternoon for Half Day leave.");
            }
            else if (leaveType == "Short Leave")
            {
                if (fromDate.Date != toDate.Date) return (false, "Short Leave must be for a single date.");

                if (fromTime == null || toTime == null)
                    return (false, "Please select both From Time and To Time for Short Leave.");

                if (toTime <= fromTime)
                    return (false, "To Time must be after From Time.");

                if ((int)fromTime.Value.TotalMinutes % 30 != 0 || (int)toTime.Value.TotalMinutes % 30 != 0)
                    return (false, "Time must be selected in 30-minute steps (e.g. 9:00 or 9:30).");

                var duration = (decimal)(toTime.Value - fromTime.Value).TotalHours;

                if (duration > 8)
                    return (false, "Short Leave duration cannot exceed a full working day (8 hours).");

                hours = duration;
            }

            return (true, string.Empty);
        }

        private bool HasOverlap(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var existing = _repo.GetByEmployee(employeeId).Where(l => l.Status == "Pending" || l.Status == "Approved");
            return existing.Any(l => fromDate.Date <= l.ToDate.Date && toDate.Date >= l.FromDate.Date);
        }
    }
}