using AttendanceSystem.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AttendanceSystem.Controllers
{
    [Authorize]
    public class LeaveController : Controller
    {
        private readonly ILeaveService _leaveService;
        private readonly IEmployeeService _employeeService;

        public LeaveController(ILeaveService leaveService, IEmployeeService employeeService)
        {
            _leaveService = leaveService;
            _employeeService = employeeService;
        }

        private int CurrentEmployeeId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        public IActionResult Index()
        {
            var leaves = _leaveService.GetByEmployee(CurrentEmployeeId);
            ViewBag.Balance = _leaveService.GetBalance(CurrentEmployeeId, DateTime.Today.Year);
            return View(leaves);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitRequest(DateTime fromDate, DateTime toDate, string reason, string leaveType, string halfDaySession, TimeSpan? fromTime, TimeSpan? toTime)
        {
            var result = _leaveService.RequestLeave(CurrentEmployeeId, fromDate, toDate, reason, leaveType, halfDaySession, fromTime, toTime);
            TempData["Message"] = result.Message;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(int leaveId)
        {
            _leaveService.Cancel(leaveId, CurrentEmployeeId);
            TempData["Message"] = "Leave request cancelled.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            ViewBag.Employees = _employeeService.GetAll();
            return View(_leaveService.GetAll());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int leaveId)
        {
            _leaveService.Approve(leaveId, CurrentEmployeeId);
            return RedirectToAction("Admin");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int leaveId)
        {
            _leaveService.Reject(leaveId, CurrentEmployeeId);
            return RedirectToAction("Admin");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Assign(int employeeId, DateTime fromDate, DateTime toDate, string reason, string leaveType, string halfDaySession, decimal? hours, TimeSpan? fromTime, TimeSpan? toTime)
        {
            _leaveService.AssignLeave(employeeId, fromDate, toDate, reason, CurrentEmployeeId, leaveType, halfDaySession, hours, fromTime, toTime);
            TempData["Message"] = "Leave assigned successfully.";
            return RedirectToAction("Admin");
        }
    }
}