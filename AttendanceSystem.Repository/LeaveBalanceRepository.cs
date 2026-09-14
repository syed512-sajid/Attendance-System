    using AttendanceSystem.Contracts.Repository;
    using AttendanceSystem.Models.Entities;
    using Microsoft.Data.SqlClient;
    using System.Data;

namespace AttendanceSystem.Repository
{
    public class LeaveBalanceRepository : ILeaveBalanceRepository
    {
        private readonly DbHelper _db;
        public LeaveBalanceRepository(DbHelper db) => _db = db;

        public void AssignBalance(int employeeId, int year, int totalLeaves)
        {
            _db.ExecuteNonQuery("att_LeaveBalance_Assign", new[]
            {
                    new SqlParameter("@EmployeeID", employeeId),
                    new SqlParameter("@Year", year),
                    new SqlParameter("@TotalLeaves", totalLeaves)
                });
        }

        public LeaveBalance GetByEmployee(int employeeId, int year)
        {
            var dt = _db.ExecuteDataTable("att_LeaveBalance_GetByEmployee", new[]
            {
        new SqlParameter("@EmployeeID", employeeId),
        new SqlParameter("@Year", year)
    });
            if (dt.Rows.Count == 0) return new LeaveBalance { EmployeeID = employeeId, Year = year, TotalLeaves = 0, UsedLeaves = 0 };

            var r = dt.Rows[0];
            return new LeaveBalance
            {
                EmployeeID = employeeId,
                Year = year,
                TotalLeaves = Convert.ToInt32(r["TotalLeaves"]),
                UsedLeaves = Convert.ToDecimal(r["UsedLeaves"])
            };
        }

        public List<LeaveBalance> GetAll(int year)
        {
            var dt = _db.ExecuteDataTable("att_LeaveBalance_GetAll", new[] { new SqlParameter("@Year", year) });
            return dt.Rows.Cast<DataRow>().Select(r => new LeaveBalance
            {
                EmployeeID = Convert.ToInt32(r["EmployeeID"]),
                EmployeeName = r["EmployeeName"].ToString(),
                Year = year,
                TotalLeaves = Convert.ToInt32(r["TotalLeaves"]),
                UsedLeaves = Convert.ToDecimal(r["UsedLeaves"])
            }).ToList();
        }
    }
}