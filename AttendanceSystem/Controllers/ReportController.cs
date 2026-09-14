using AttendanceSystem.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IEmployeeService _employeeService;
        public ReportController(IReportService reportService, IEmployeeService employeeService)
        {
            _reportService = reportService;
            _employeeService = employeeService;
        }

        public IActionResult Index(int? year, int? month, int? employeeId)
        {
            var y = year ?? DateTime.Today.Year;
            var m = month ?? DateTime.Today.Month;

            ViewBag.Employees = _employeeService.GetAll();
            ViewBag.Year = y;
            ViewBag.Month = m;
            ViewBag.EmployeeId = employeeId;

            var report = _reportService.GetMonthlyReport(y, m, employeeId);
            return View(report);
        }

        public IActionResult Export(int year, int month, int? employeeId) { var bytes = _reportService.ExportMonthlyReportToExcel(year, month, employeeId); var fileName = $"Attendance_Report_{year}_{month:D2}.xlsx"; return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName); }
    }
}
