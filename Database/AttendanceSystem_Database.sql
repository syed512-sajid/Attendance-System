-- =====================================================================
-- Attendance System Database Script
-- Naya database aap khud banayen phir yeh poora script us database par
-- RUN karen (SSMS me database select karke "Execute" karen).
-- =====================================================================

-- Agar naya database banana ho to niche wali line uncomment kar den:
-- CREATE DATABASE AttendanceSystemDB;
-- GO
-- USE AttendanceSystemDB;
-- GO

-- ---------------------------------------------------------------------
-- Tables
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.Employees','U') IS NULL
BEGIN
    CREATE TABLE dbo.Employees
    (
        EmployeeID   INT IDENTITY(1,1) PRIMARY KEY,
        FullName     NVARCHAR(150)   NOT NULL,
        Username     NVARCHAR(50)    NOT NULL UNIQUE,
        PasswordHash NVARCHAR(200)   NOT NULL,
        Role         VARCHAR(20)     NOT NULL DEFAULT 'Employee', -- Admin / Employee
        IsActive     BIT             NOT NULL DEFAULT 1,
        CreatedDate  DATETIME        NOT NULL DEFAULT GETDATE()
    );
END
GO

IF OBJECT_ID('dbo.Attendance','U') IS NULL
BEGIN
    CREATE TABLE dbo.Attendance
    (
        AttendanceID   INT IDENTITY(1,1) PRIMARY KEY,
        EmployeeID     INT NOT NULL FOREIGN KEY REFERENCES dbo.Employees(EmployeeID),
        AttendanceDate DATE NOT NULL,
        CheckInTime    DATETIME NULL,
        CheckOutTime   DATETIME NULL,
        Status         VARCHAR(20) NOT NULL DEFAULT 'Present', -- Present / Leave / Absent
        TotalHours     DECIMAL(5,2) NULL,
        IsAutoCheckout BIT NOT NULL DEFAULT 0,
        CONSTRAINT UQ_Attendance_Emp_Date UNIQUE (EmployeeID, AttendanceDate)
    );
END
GO

IF OBJECT_ID('dbo.Leaves','U') IS NULL
BEGIN
    CREATE TABLE dbo.Leaves
    (
        LeaveID       INT IDENTITY(1,1) PRIMARY KEY,
        EmployeeID    INT NOT NULL FOREIGN KEY REFERENCES dbo.Employees(EmployeeID),
        FromDate      DATE NOT NULL,
        ToDate        DATE NOT NULL,
        Reason        NVARCHAR(500) NULL,
        Status        VARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending / Approved / Rejected
        RequestedBy   VARCHAR(20) NOT NULL DEFAULT 'Employee', -- Employee / Admin
        CreatedBy     INT NOT NULL FOREIGN KEY REFERENCES dbo.Employees(EmployeeID),
        ActionBy      INT NULL FOREIGN KEY REFERENCES dbo.Employees(EmployeeID),
        ActionDate    DATETIME NULL,
        CreatedDate   DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO

-- ---------------------------------------------------------------------
-- Seed default Admin (Username: admin | Password: Admin@123)
-- Login ke baad chahen to naya employee bana ke iski jagah use karen.
-- ---------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.Employees WHERE Username = 'admin')
BEGIN
    INSERT INTO dbo.Employees (FullName, Username, PasswordHash, Role, IsActive)
    VALUES ('Administrator', 'admin', 'e86f78a8a3caf0b60d8e74e5942aa6d86dc150cd3c03338aef25b7d2d7e3acc7', 'Admin', 1);
END
GO

-- =====================================================================
-- Stored Procedures
-- =====================================================================

-- ---------- Employee ----------
CREATE OR ALTER PROCEDURE dbo.att_Employee_Login
    @Username NVARCHAR(50),
    @PasswordHash NVARCHAR(200)
AS
BEGIN
    SELECT EmployeeID, FullName, Username, Role, IsActive
    FROM dbo.Employees
    WHERE Username = @Username AND PasswordHash = @PasswordHash AND IsActive = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.att_Employee_Save
    @EmployeeID INT = NULL OUTPUT,
    @FullName NVARCHAR(150),
    @Username NVARCHAR(50),
    @PasswordHash NVARCHAR(200),
    @Role VARCHAR(20)
AS
BEGIN
    IF (@EmployeeID IS NULL OR @EmployeeID = 0)
    BEGIN
        INSERT INTO dbo.Employees (FullName, Username, PasswordHash, Role, IsActive)
        VALUES (@FullName, @Username, @PasswordHash, @Role, 1);
        SET @EmployeeID = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        UPDATE dbo.Employees
        SET FullName = @FullName, Role = @Role
        WHERE EmployeeID = @EmployeeID;
    END
END
GO

CREATE OR ALTER PROCEDURE dbo.att_Employee_GetAll
AS
BEGIN
    SELECT EmployeeID, FullName, Username, Role, IsActive, CreatedDate
    FROM dbo.Employees
    ORDER BY FullName;
END
GO

CREATE OR ALTER PROCEDURE dbo.att_Employee_GetById
    @EmployeeID INT
AS
BEGIN
    SELECT EmployeeID, FullName, Username, Role, IsActive, CreatedDate
    FROM dbo.Employees
    WHERE EmployeeID = @EmployeeID;
END
GO

CREATE OR ALTER PROCEDURE dbo.att_Employee_ToggleActive
    @EmployeeID INT,
    @IsActive BIT
AS
BEGIN
    UPDATE dbo.Employees SET IsActive = @IsActive WHERE EmployeeID = @EmployeeID;
END
GO

-- ---------- Attendance ----------
CREATE OR ALTER PROCEDURE dbo.att_Attendance_CheckIn
    @EmployeeID INT,
    @AttendanceDate DATE,
    @CheckInTime DATETIME
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.Attendance WHERE EmployeeID = @EmployeeID AND AttendanceDate = @AttendanceDate)
    BEGIN
        INSERT INTO dbo.Attendance (EmployeeID, AttendanceDate, CheckInTime, Status)
        VALUES (@EmployeeID, @AttendanceDate, @CheckInTime, 'Present');
    END
END
GO

CREATE OR ALTER PROCEDURE dbo.att_Attendance_CheckOut
    @EmployeeID INT,
    @AttendanceDate DATE,
    @CheckOutTime DATETIME
AS
BEGIN
    UPDATE dbo.Attendance
    SET CheckOutTime = @CheckOutTime,
        TotalHours = ROUND(CAST(DATEDIFF(MINUTE, CheckInTime, @CheckOutTime) AS DECIMAL(9,2)) / 60.0, 2)
    WHERE EmployeeID = @EmployeeID AND AttendanceDate = @AttendanceDate AND CheckOutTime IS NULL;
END
GO

CREATE OR ALTER PROCEDURE dbo.att_Attendance_GetToday
    @EmployeeID INT,
    @AttendanceDate DATE
AS
BEGIN
    SELECT TOP 1 * FROM dbo.Attendance
    WHERE EmployeeID = @EmployeeID AND AttendanceDate = @AttendanceDate;
END
GO

CREATE OR ALTER PROCEDURE dbo.att_Attendance_GetByEmployeeMonth
    @EmployeeID INT,
    @Year INT,
    @Month INT
AS
BEGIN
    SELECT a.*, e.FullName AS EmployeeName
    FROM dbo.Attendance a
    INNER JOIN dbo.Employees e ON e.EmployeeID = a.EmployeeID
    WHERE a.EmployeeID = @EmployeeID
      AND YEAR(a.AttendanceDate) = @Year AND MONTH(a.AttendanceDate) = @Month
    ORDER BY a.AttendanceDate;
END
GO

CREATE OR ALTER PROCEDURE dbo.att_Attendance_GetAllMonth
    @Year INT,
    @Month INT
AS
BEGIN
    SELECT a.*, e.FullName AS EmployeeName
    FROM dbo.Attendance a
    INNER JOIN dbo.Employees e ON e.EmployeeID = a.EmployeeID
    WHERE YEAR(a.AttendanceDate) = @Year AND MONTH(a.AttendanceDate) = @Month
    ORDER BY e.FullName, a.AttendanceDate;
END
GO

CREATE OR ALTER PROCEDURE dbo.att_Attendance_GetOpenForAutoCheckout
    @AttendanceDate DATE
AS
BEGIN
    SELECT AttendanceID, EmployeeID, CheckInTime
    FROM dbo.Attendance
    WHERE AttendanceDate = @AttendanceDate AND CheckOutTime IS NULL;
END
GO

CREATE OR ALTER PROCEDURE dbo.att_Attendance_MarkAutoCheckout
    @AttendanceID INT,
    @CheckOutTime DATETIME
AS
BEGIN
    UPDATE dbo.Attendance
    SET CheckOutTime = @CheckOutTime,
        IsAutoCheckout = 1,
        TotalHours = ROUND(CAST(DATEDIFF(MINUTE, CheckInTime, @CheckOutTime) AS DECIMAL(9,2)) / 60.0, 2)
    WHERE AttendanceID = @AttendanceID;
END
GO

-- ---------- Leaves ----------
CREATE OR ALTER PROCEDURE dbo.att_Leave_Save
    @EmployeeID INT,
    @FromDate DATE,
    @ToDate DATE,
    @Reason NVARCHAR(500),
    @RequestedBy VARCHAR(20),
    @CreatedBy INT,
    @Status VARCHAR(20) = 'Pending'
AS
BEGIN
    INSERT INTO dbo.Leaves (EmployeeID, FromDate, ToDate, Reason, Status, RequestedBy, CreatedBy)
    VALUES (@EmployeeID, @FromDate, @ToDate, @Reason, @Status, @RequestedBy, @CreatedBy);
END
GO

CREATE OR ALTER PROCEDURE dbo.att_Leave_UpdateStatus
    @LeaveID INT,
    @Status VARCHAR(20),
    @ActionBy INT
AS
BEGIN
    UPDATE dbo.Leaves
    SET Status = @Status, ActionBy = @ActionBy, ActionDate = GETDATE()
    WHERE LeaveID = @LeaveID;
END
GO

CREATE OR ALTER PROCEDURE dbo.att_Leave_GetAll
AS
BEGIN
    SELECT l.*, e.FullName AS EmployeeName, a.FullName AS ActionByName
    FROM dbo.Leaves l
    INNER JOIN dbo.Employees e ON e.EmployeeID = l.EmployeeID
    LEFT JOIN dbo.Employees a ON a.EmployeeID = l.ActionBy
    ORDER BY l.CreatedDate DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.att_Leave_GetByEmployee
    @EmployeeID INT
AS
BEGIN
    SELECT l.*, e.FullName AS EmployeeName, a.FullName AS ActionByName
    FROM dbo.Leaves l
    INNER JOIN dbo.Employees e ON e.EmployeeID = l.EmployeeID
    LEFT JOIN dbo.Employees a ON a.EmployeeID = l.ActionBy
    WHERE l.EmployeeID = @EmployeeID
    ORDER BY l.CreatedDate DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.att_Leave_GetApprovedForMonth
    @Year INT,
    @Month INT
AS
BEGIN
    SELECT * FROM dbo.Leaves
    WHERE Status = 'Approved'
      AND FromDate <= EOMONTH(DATEFROMPARTS(@Year, @Month, 1))
      AND ToDate   >= DATEFROMPARTS(@Year, @Month, 1);
END
GO

PRINT 'AttendanceSystem database objects created successfully.';
