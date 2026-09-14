using AttendanceSystem.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AttendanceSystem.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IWebHostEnvironment _env;

        public ProfileController(IEmployeeService employeeService, IWebHostEnvironment env)
        {
            _employeeService = employeeService;
            _env = env;
        }

        private int CurrentEmployeeId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        public IActionResult Index()
        {
            var employee = _employeeService.GetById(CurrentEmployeeId);
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(string jobTitle, IFormFile profilePicture)
        {
            string picturePath = null;

            if (profilePicture != null && profilePicture.Length > 0)
            {
                var ext = Path.GetExtension(profilePicture.FileName);
                var fileName = $"emp_{CurrentEmployeeId}{ext}";
                var folder = Path.Combine(_env.WebRootPath, "images", "profiles");
                Directory.CreateDirectory(folder);
                var fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    profilePicture.CopyTo(stream);
                }
                picturePath = "/images/profiles/" + fileName;
            }

            _employeeService.UpdateProfile(CurrentEmployeeId, jobTitle, picturePath);
            TempData["Message"] = "Profile updated successfully.";
            return RedirectToAction("Index");
        }
    }
}