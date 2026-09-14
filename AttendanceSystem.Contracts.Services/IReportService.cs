using AttendanceSystem.Models.Entities;

namespace AttendanceSystem.Contracts.Services
{
    public interface IReportService
    {
        List<MonthlyReportItem> GetMonthlyReport(int year, int month, int? employeeId = null);
        byte[] ExportMonthlyReportToExcel(int year, int month, int? employeeId = null);
    }
}
