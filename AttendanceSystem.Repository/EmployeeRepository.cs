using AttendanceSystem.Contracts.Repository;
using AttendanceSystem.Models.Entities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AttendanceSystem.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DbHelper _db;
        public EmployeeRepository(DbHelper db) => _db = db;

        public Employee GetByUsername(string username)
        {
            var dt = _db.ExecuteDataTable("att_Employee_GetByUsername", new[]
            {
                new SqlParameter("@Username", username)
            });
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public int Save(Employee employee)
        {
            using var conn = new SqlConnection(_db.ConnectionString);
            var idParam = new SqlParameter("@EmployeeID", SqlDbType.Int)
            {
                Direction = ParameterDirection.InputOutput,
                Value = employee.EmployeeID == 0 ? (object)DBNull.Value : employee.EmployeeID
            };
            var cmd = _db.PrepareCommand(conn, "att_Employee_Save", new[]
            {
                idParam,
                new SqlParameter("@FullName", employee.FullName),
                new SqlParameter("@Username", employee.Username),
                new SqlParameter("@PasswordHash", employee.PasswordHash),
                new SqlParameter("@Role", employee.Role)
            });
            conn.Open();
            cmd.ExecuteNonQuery();
            return idParam.Value == DBNull.Value ? 0 : Convert.ToInt32(idParam.Value);
        }

        public List<Employee> GetAll()
        {
            var dt = _db.ExecuteDataTable("att_Employee_GetAll");
            return dt.Rows.Cast<DataRow>().Select(Map).ToList();
        }

        public Employee GetById(int employeeId)
        {
            var dt = _db.ExecuteDataTable("att_Employee_GetById", new[] { new SqlParameter("@EmployeeID", employeeId) });
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public void ToggleActive(int employeeId, bool isActive)
        {
            _db.ExecuteNonQuery("att_Employee_ToggleActive", new[]
            {
                new SqlParameter("@EmployeeID", employeeId),
                new SqlParameter("@IsActive", isActive)
            });
        }

        public void UpdateProfile(int employeeId, string jobTitle, string profilePicturePath)
        {
            _db.ExecuteNonQuery("att_Employee_UpdateProfile", new[]
            {
                new SqlParameter("@EmployeeID", employeeId),
                new SqlParameter("@JobTitle", (object)jobTitle ?? DBNull.Value),
                new SqlParameter("@ProfilePicturePath", (object)profilePicturePath ?? DBNull.Value)
            });
        }

        private static Employee Map(DataRow r) => new()
        {
            EmployeeID = Convert.ToInt32(r["EmployeeID"]),
            FullName = r["FullName"].ToString(),
            Username = r["Username"].ToString(),
            Role = r["Role"].ToString(),
            IsActive = Convert.ToBoolean(r["IsActive"]),
            AnnualLeaveQuota = r.Table.Columns.Contains("AnnualLeaveQuota") && r["AnnualLeaveQuota"] != DBNull.Value ? Convert.ToDecimal(r["AnnualLeaveQuota"]) : 14,
            PasswordHash = r.Table.Columns.Contains("PasswordHash") && r["PasswordHash"] != DBNull.Value ? r["PasswordHash"].ToString() : null,
            JobTitle = r.Table.Columns.Contains("JobTitle") && r["JobTitle"] != DBNull.Value ? r["JobTitle"].ToString() : null,
            ProfilePicturePath = r.Table.Columns.Contains("ProfilePicturePath") && r["ProfilePicturePath"] != DBNull.Value ? r["ProfilePicturePath"].ToString() : null
        };
    }
}