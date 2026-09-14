# Attendance System - Setup

## 1. Database
`Database/AttendanceSystem_Database.sql` file apne `testdb` database par SSMS me RUN karen.
Isse Employees, Attendance, Leaves tables + saare stored procedures ban jayenge, aur ek default
Admin bhi ban jayega:

- Username: `admin`
- Password: `Admin@123`

(Login ke baad naya admin bana kar chahen to isse deactivate kar den.)

## 2. Connection String
`AttendanceSystem/appsettings.json` me connection string check/update karen:

```
"AttendanceDb": "Server=.;Database=testdb;Trusted_Connection=True;TrustServerCertificate=True;"
```

Agar SQL Server named instance hai (jaise `.\SQLEXPRESS`) to `Server=.` ko us se replace karen.

## 3. Office WiFi Restriction
`appsettings.json` me `OfficeNetwork:AllowedIpPrefixes` list update karen - apne office
router/wifi ka IP range daalen (kisi bhi office PC par `ipconfig` chala kar IPv4 Address ka
pehla 3 hisse copy kar len, jaise `192.168.1.`). Isse bahar se koi access nahi kar sakega
(localhost hamesha allow hai, taake aap development me test kar saken).

## 4. Auto-Checkout
Har roz raat 10:00 PM par jo employee checkout nahi karta, uska checkout khud lag jata hai
(`AutoCheckout:Hour` / `Minute` appsettings.json me change ho sakte hain).

## 5. Run
Visual Studio me `AttendanceSystem.sln` khol kar `AttendanceSystem` project ko Startup Project
set karen aur Run karen (F5). Pehli baar NuGet packages restore honge (internet chahiye):
- Microsoft.Data.SqlClient
- ClosedXML (Excel export ke liye)

## 6. Roles
- **Admin**: Employees add/manage kar sakta hai, leaves approve/reject/assign kar sakta hai,
  monthly report dekh/export kar sakta hai.
- **Employee**: Sirf apni attendance (check-in/out) aur apni leave requests dekh/bhej sakta hai.
  Employee kabhi bhi kisi doosre ka data ya database change nahi kar sakta - sab kuch
  role-based [Authorize] se locked hai, aur database ka access sirf app ke through hota hai.

## Folder Structure
```
AttendanceSystem.sln
Database/AttendanceSystem_Database.sql
AttendanceSystem.Models/            -> Entities (Employee, AttendanceRecord, LeaveRequest, MonthlyReportItem)
AttendanceSystem.Contracts.Repository/  -> Repository interfaces
AttendanceSystem.Contracts.Services/    -> Service interfaces
AttendanceSystem.Repository/        -> ADO.NET + Stored Procedure implementation
AttendanceSystem.Services/          -> Business logic + Excel export
AttendanceSystem/                   -> MVC Web app (Controllers, Views, Program.cs)
```
