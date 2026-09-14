using AttendanceSystem.Models.Entities;

namespace AttendanceSystem.Contracts.Repository
{
    public interface IEmployeeRepository
    {
        Employee GetByUsername(string username);
        int Save(Employee employee);
        List<Employee> GetAll();
        Employee GetById(int employeeId);
        void ToggleActive(int employeeId, bool isActive);
        void UpdateProfile(int employeeId, string jobTitle, string profilePicturePath);
    }
}