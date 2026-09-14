using AttendanceSystem.Contracts.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AttendanceSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IEmployeeService _employeeService;
        public AdminController(IEmployeeService employeeService) => _employeeService = employeeService;

        private int CurrentEmployeeId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        public IActionResult Employees() => View(_employeeService.GetAll());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string fullName, string username, string password, string role)
        {
            _employeeService.CreateEmployee(fullName, username, password, role);
            TempData["Message"] = $"Employee created successfully. Username: {username} | Password: {password} - please share these credentials with the employee.";
            return RedirectToAction("Employees");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int employeeId, string fullName, string username, string password, string role)
        {
            _employeeService.UpdateEmployee(employeeId, fullName, username, password, role);

            // Admin ne khud ko Employee bana diya - session ab invalid hai, logout kar do
            if (employeeId == CurrentEmployeeId && role != "Admin")
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["Message"] = "Your role was changed to Employee - please log in again.";
                return RedirectToAction("Login", "Account");
            }

            TempData["Message"] = "Employee updated successfully.";
            return RedirectToAction("Employees");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleActive(int employeeId, bool isActive)
        {
            _employeeService.ToggleActive(employeeId, isActive);
            return RedirectToAction("Employees");
        }

        // Row edit ke waqt current password AJAX se laane ke liye
        [HttpGet]
        public IActionResult GetPassword(int employeeId)
        {
            var password = _employeeService.GetDecryptedPassword(employeeId);
            return Json(new { password });
        }
    }
}