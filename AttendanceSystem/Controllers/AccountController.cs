using AttendanceSystem.Contracts.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AttendanceSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IAttendanceService _attendanceService;
        private readonly IConfiguration _configuration;

        public AccountController(IEmployeeService employeeService, IAttendanceService attendanceService, IConfiguration configuration)
        {
            _employeeService = employeeService;
            _attendanceService = attendanceService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login() => View();

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            username = username?.Trim();
            password = password?.Trim();
            var employee = _employeeService.Login(username, password);
            if (employee == null)
            {
                ViewBag.Error = "Username ya password ghalat hai.";
                return View();
            }

            // Is employee ke purane open (bina-checkout) records ho to abhi mark kar do
            var hour = _configuration.GetValue<int>("AutoCheckout:Hour", 22);
            var minute = _configuration.GetValue<int>("AutoCheckout:Minute", 0);
            await _attendanceService.RunAutoCheckoutAsync(hour, minute);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, employee.EmployeeID.ToString()),
                new(ClaimTypes.Name, employee.FullName),
                new(ClaimTypes.Role, employee.Role)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword() => View();
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}