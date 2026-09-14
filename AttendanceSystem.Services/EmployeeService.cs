using AttendanceSystem.Contracts.Repository;
using AttendanceSystem.Contracts.Services;
using AttendanceSystem.Models.Entities;

namespace AttendanceSystem.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repo;
        public EmployeeService(IEmployeeRepository repo) => _repo = repo;

        public Employee Login(string username, string password)
        {
            var employee = _repo.GetByUsername(username);
            if (employee == null) return null;

            string decrypted;
            try { decrypted = PasswordHelper.Decrypt(employee.PasswordHash); }
            catch { return null; } // purana hash-based account - admin se password reset karwayen

            return decrypted == password ? employee : null;
        }

        public int CreateEmployee(string fullName, string username, string password, string role)
        {
            var employee = new Employee
            {
                FullName = fullName,
                Username = username,
                PasswordHash = PasswordHelper.Encrypt(password),
                Role = role
            };
            return _repo.Save(employee);
        }

        public void UpdateEmployee(int employeeId, string fullName, string username, string password, string role)
        {
            var existing = _repo.GetById(employeeId);
            if (existing == null) return;

            var employee = new Employee
            {
                EmployeeID = employeeId,
                FullName = fullName,
                Username = username,
                PasswordHash = string.IsNullOrWhiteSpace(password)
                    ? existing.PasswordHash
                    : PasswordHelper.Encrypt(password),
                Role = role
            };
            _repo.Save(employee);
        }

        public string GetDecryptedPassword(int employeeId)
        {
            var existing = _repo.GetById(employeeId);
            if (existing == null || string.IsNullOrEmpty(existing.PasswordHash)) return null;
            try { return PasswordHelper.Decrypt(existing.PasswordHash); }
            catch { return null; }
        }

        public List<Employee> GetAll() => _repo.GetAll();
        public Employee GetById(int employeeId) => _repo.GetById(employeeId);
        public void ToggleActive(int employeeId, bool isActive) => _repo.ToggleActive(employeeId, isActive);
        public void UpdateProfile(int employeeId, string jobTitle, string profilePicturePath) => _repo.UpdateProfile(employeeId, jobTitle, profilePicturePath);
    }
}