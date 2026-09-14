using AttendanceSystem.Contracts.Repository;
using AttendanceSystem.Models.Entities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AttendanceSystem.Repository
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly DbHelper _db;
        public LeaveRepository(DbHelper db) => _db = db;

        public void Save(LeaveRequest leave)
        {
            _db.ExecuteNonQuery("att_Leave_Save", new[]
            {
                new SqlParameter("@EmployeeID", leave.EmployeeID),
                new SqlParameter("@FromDate", leave.FromDate.Date),
                new SqlParameter("@ToDate", leave.ToDate.Date),
                new SqlParameter("@Reason", (object)leave.Reason ?? DBNull.Value),
                new SqlParameter("@RequestedBy", leave.RequestedBy),
                new SqlParameter("@CreatedBy", leave.CreatedBy),
                new SqlParameter("@Status", leave.Status),
                new SqlParameter("@LeaveType", leave.LeaveType),
                new SqlParameter("@HalfDaySession", (object)leave.HalfDaySession ?? DBNull.Value),
                new SqlParameter("@Hours", (object)leave.Hours ?? DBNull.Value),
                new SqlParameter("@FromTime", (object)leave.FromTime ?? DBNull.Value),
                new SqlParameter("@ToTime", (object)leave.ToTime ?? DBNull.Value)
            });
        }

        public void UpdateStatus(int leaveId, string status, int actionBy)
        {
            _db.ExecuteNonQuery("att_Leave_UpdateStatus", new[]
            {
                new SqlParameter("@LeaveID", leaveId),
                new SqlParameter("@Status", status),
                new SqlParameter("@ActionBy", actionBy)
            });
        }

        public void Cancel(int leaveId, int employeeId)
        {
            _db.ExecuteNonQuery("att_Leave_Cancel", new[]
            {
                new SqlParameter("@LeaveID", leaveId),
                new SqlParameter("@EmployeeID", employeeId)
            });
        }

        public List<LeaveRequest> GetAll()
        {
            var dt = _db.ExecuteDataTable("att_Leave_GetAll");
            return dt.Rows.Cast<DataRow>().Select(Map).ToList();
        }

        public List<LeaveRequest> GetByEmployee(int employeeId)
        {
            var dt = _db.ExecuteDataTable("att_Leave_GetByEmployee", new[] { new SqlParameter("@EmployeeID", employeeId) });
            return dt.Rows.Cast<DataRow>().Select(Map).ToList();
        }

        public List<LeaveRequest> GetApprovedForMonth(int year, int month)
        {
            var dt = _db.ExecuteDataTable("att_Leave_GetApprovedForMonth", new[]
            {
                new SqlParameter("@Year", year),
                new SqlParameter("@Month", month)
            });
            return dt.Rows.Cast<DataRow>().Select(Map).ToList();
        }

        // --new: att_Leave_GetAll (jo pehle se sab status + EmployeeName deta hai) reuse karke
        // sirf us mahine ke saath overlap karne wali leaves nikal rahe hain — koi naya SP nahi chahiye
        public List<LeaveRequest> GetAllForMonth(int year, int month)
        {
            var monthStart = new DateTime(year, month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            return GetAll()
                .Where(l => l.FromDate.Date <= monthEnd && l.ToDate.Date >= monthStart)
                .OrderBy(l => l.FromDate)
                .ToList();
        }

        private static LeaveRequest Map(DataRow r) => new()
        {
            LeaveID = Convert.ToInt32(r["LeaveID"]),
            EmployeeID = Convert.ToInt32(r["EmployeeID"]),
            EmployeeName = r.Table.Columns.Contains("EmployeeName") ? r["EmployeeName"].ToString() : null,
            FromDate = Convert.ToDateTime(r["FromDate"]),
            ToDate = Convert.ToDateTime(r["ToDate"]),
            Reason = r["Reason"] == DBNull.Value ? null : r["Reason"].ToString(),
            Status = r["Status"].ToString(),
            RequestedBy = r["RequestedBy"].ToString(),
            CreatedBy = Convert.ToInt32(r["CreatedBy"]),
            ActionBy = r["ActionBy"] == DBNull.Value ? null : Convert.ToInt32(r["ActionBy"]),
            ActionByName = r.Table.Columns.Contains("ActionByName") && r["ActionByName"] != DBNull.Value ? r["ActionByName"].ToString() : null,
            ActionDate = r["ActionDate"] == DBNull.Value ? null : Convert.ToDateTime(r["ActionDate"]),
            CreatedDate = Convert.ToDateTime(r["CreatedDate"]),
            LeaveType = r.Table.Columns.Contains("LeaveType") && r["LeaveType"] != DBNull.Value ? r["LeaveType"].ToString() : "Full Day",
            HalfDaySession = r.Table.Columns.Contains("HalfDaySession") && r["HalfDaySession"] != DBNull.Value ? r["HalfDaySession"].ToString() : null,
            Hours = r.Table.Columns.Contains("Hours") && r["Hours"] != DBNull.Value ? Convert.ToDecimal(r["Hours"]) : null,
            FromTime = r.Table.Columns.Contains("FromTime") && r["FromTime"] != DBNull.Value ? (TimeSpan?)r["FromTime"] : null,
            ToTime = r.Table.Columns.Contains("ToTime") && r["ToTime"] != DBNull.Value ? (TimeSpan?)r["ToTime"] : null
        };
    }
}