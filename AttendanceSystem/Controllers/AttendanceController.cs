using AttendanceSystem.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AttendanceSystem.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;
        public AttendanceController(IAttendanceService attendanceService) => _attendanceService = attendanceService;

        private int CurrentEmployeeId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        public IActionResult Index()
        {
            var today = _attendanceService.GetTodayStatus(CurrentEmployeeId);
            var now = DateTime.Now;
            var history = _attendanceService.GetByEmployeeMonth(CurrentEmployeeId, now.Year, now.Month);
            ViewBag.Today = today;
            return View(history);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckIn()
        {
            var result = _attendanceService.CheckIn(CurrentEmployeeId);
            TempData["Message"] = result.Message;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckOut()
        {
            var result = _attendanceService.CheckOut(CurrentEmployeeId);
            TempData["Message"] = result.Message;
            return RedirectToAction("Index");
        }
    }
}
