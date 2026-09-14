namespace AttendanceSystem.Models.Entities
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; } // ab yeh encrypted (retrievable) hai, one-way hash nahi
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public decimal AnnualLeaveQuota { get; set; } = 14;
        public string JobTitle { get; set; }
        public string ProfilePicturePath { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}