using AttendanceSystem.Contracts.Repository;
using AttendanceSystem.Models.Entities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AttendanceSystem.Repository
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly DbHelper _db;
        public AttendanceRepository(DbHelper db) => _db = db;

        public void CheckIn(int employeeId, DateTime attendanceDate, DateTime checkInTime)
        {
            _db.ExecuteNonQuery("att_Attendance_CheckIn", new[]
            {
                new SqlParameter("@EmployeeID", employeeId),
                new SqlParameter("@AttendanceDate", attendanceDate.Date),
                new SqlParameter("@CheckInTime", checkInTime)
            });
        }

        public void CheckOut(int employeeId, DateTime attendanceDate, DateTime checkOutTime)
        {
            _db.ExecuteNonQuery("att_Attendance_CheckOut", new[]
            {
                new SqlParameter("@EmployeeID", employeeId),
                new SqlParameter("@AttendanceDate", attendanceDate.Date),
                new SqlParameter("@CheckOutTime", checkOutTime)
            });
        }

        public AttendanceRecord GetToday(int employeeId, DateTime attendanceDate)
        {
            var dt = _db.ExecuteDataTable("att_Attendance_GetToday", new[]
            {
                new SqlParameter("@EmployeeID", employeeId),
                new SqlParameter("@AttendanceDate", attendanceDate.Date)
            });
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public List<AttendanceRecord> GetByEmployeeMonth(int employeeId, int year, int month)
        {
            var dt = _db.ExecuteDataTable("att_Attendance_GetByEmployeeMonth", new[]
            {
                new SqlParameter("@EmployeeID", employeeId),
                new SqlParameter("@Year", year),
                new SqlParameter("@Month", month)
            });
            return dt.Rows.Cast<DataRow>().Select(Map).ToList();
        }

        public List<AttendanceRecord> GetAllMonth(int year, int month)
        {
            var dt = _db.ExecuteDataTable("att_Attendance_GetAllMonth", new[]
            {
                new SqlParameter("@Year", year),
                new SqlParameter("@Month", month)
            });
            return dt.Rows.Cast<DataRow>().Select(Map).ToList();
        }

        //public List<AttendanceRecord> GetOpenForAutoCheckout(DateTime attendanceDate)
        //{
        //    var dt = _db.ExecuteDataTable("att_Attendance_GetOpenForAutoCheckout", new[]
        //    {
        //        new SqlParameter("@AttendanceDate", attendanceDate.Date)
        //    });
        //    return dt.Rows.Cast<DataRow>().Select(r => new AttendanceRecord
        //    {
        //        AttendanceID = Convert.ToInt32(r["AttendanceID"]),
        //        EmployeeID = Convert.ToInt32(r["EmployeeID"]),
        //        CheckInTime = r["CheckInTime"] == DBNull.Value ? null : Convert.ToDateTime(r["CheckInTime"])
        //    }).ToList();
        //}
        public List<AttendanceRecord> GetOpenUpToDate(DateTime uptoDate)
        {
            var dt = _db.ExecuteDataTable("att_Attendance_GetOpenUpToDate", new[]
            {
                new SqlParameter("@UpToDate", uptoDate.Date)
            });
            return dt.Rows.Cast<DataRow>().Select(r => new AttendanceRecord
            {
                AttendanceID = Convert.ToInt32(r["AttendanceID"]),
                EmployeeID = Convert.ToInt32(r["EmployeeID"]),
                AttendanceDate = Convert.ToDateTime(r["AttendanceDate"]),
                CheckInTime = r["CheckInTime"] == DBNull.Value ? null : Convert.ToDateTime(r["CheckInTime"])
            }).ToList();
        }
        public void MarkAutoCheckout(int attendanceId, DateTime checkOutTime)
        {
            _db.ExecuteNonQuery("att_Attendance_MarkAutoCheckout", new[]
            {
                new SqlParameter("@AttendanceID", attendanceId),
                new SqlParameter("@CheckOutTime", checkOutTime)
            });
        }

        private static AttendanceRecord Map(DataRow r) => new()
        {
            AttendanceID = Convert.ToInt32(r["AttendanceID"]),
            EmployeeID = Convert.ToInt32(r["EmployeeID"]),
            EmployeeName = r.Table.Columns.Contains("EmployeeName") ? r["EmployeeName"].ToString() : null,
            AttendanceDate = Convert.ToDateTime(r["AttendanceDate"]),
            CheckInTime = r["CheckInTime"] == DBNull.Value ? null : Convert.ToDateTime(r["CheckInTime"]),
            CheckOutTime = r["CheckOutTime"] == DBNull.Value ? null : Convert.ToDateTime(r["CheckOutTime"]),
            Status = r["Status"].ToString(),
            TotalHours = r["TotalHours"] == DBNull.Value ? null : Convert.ToDecimal(r["TotalHours"]),
            IsAutoCheckout = Convert.ToBoolean(r["IsAutoCheckout"])
        };
    }
}
