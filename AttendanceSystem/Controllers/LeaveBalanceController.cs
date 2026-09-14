using AttendanceSystem.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LeaveBalanceController : Controller
    {
        private readonly ILeaveService _leaveService;
        private readonly IEmployeeService _employeeService;

        public LeaveBalanceController(ILeaveService leaveService, IEmployeeService employeeService)
        {
            _leaveService = leaveService;
            _employeeService = employeeService;
        }

        public IActionResult Index(int? year)
        {
            var y = year ?? DateTime.Today.Year;
            ViewBag.Year = y;
            ViewBag.Employees = _employeeService.GetAll();
            return View(_leaveService.GetAllBalances(y));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Assign(int employeeId, int year, int totalLeaves)
        {
            _leaveService.AssignLeaveBalance(employeeId, year, totalLeaves);
            TempData["Message"] = "Leave balance updated successfully.";
            return RedirectToAction("Index", new { year });
        }
        [HttpGet]
        public IActionResult GetBalance(int employeeId, int year)
        {
            var balance = _leaveService.GetBalance(employeeId, year);
            return Json(new { totalLeaves = balance.TotalLeaves, usedLeaves = balance.UsedLeaves, remainingLeaves = balance.RemainingLeaves });
        }
    }
}