using AttendanceSystem.Models.Entities;

namespace AttendanceSystem.Contracts.Services
{
    public interface IEmployeeService
    {
        Employee Login(string username, string password);
        int CreateEmployee(string fullName, string username, string password, string role);
        void UpdateEmployee(int employeeId, string fullName, string username, string password, string role);
        string GetDecryptedPassword(int employeeId);
        List<Employee> GetAll();
        Employee GetById(int employeeId);
        void ToggleActive(int employeeId, bool isActive);
        void UpdateProfile(int employeeId, string jobTitle, string profilePicturePath);
    }
}