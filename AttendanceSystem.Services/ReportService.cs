using AttendanceSystem.Contracts.Repository;
using AttendanceSystem.Contracts.Services;
using AttendanceSystem.Models.Entities;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml;
using System;
using System.Reflection;

namespace AttendanceSystem.Services
{
    public class ReportService : IReportService
    {
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IAttendanceRepository _attendanceRepo;
        private readonly ILeaveRepository _leaveRepo;

        public ReportService(IEmployeeRepository employeeRepo, IAttendanceRepository attendanceRepo, ILeaveRepository leaveRepo)
        {
            _employeeRepo = employeeRepo;
            _attendanceRepo = attendanceRepo;
            _leaveRepo = leaveRepo;
        }

        public List<MonthlyReportItem> GetMonthlyReport(int year, int month, int? employeeId = null)
        {
            var employees = employeeId.HasValue
                ? new List<Employee> { _employeeRepo.GetById(employeeId.Value) }
                : _employeeRepo.GetAll();

            var allAttendance = _attendanceRepo.GetAllMonth(year, month);
            var approvedLeaves = _leaveRepo.GetApprovedForMonth(year, month);

            var daysInMonth = DateTime.DaysInMonth(year, month);
            var lastDay = new DateTime(year, month, daysInMonth);
            var today = DateTime.Today;
            var uptoDay = lastDay < today ? lastDay : today; // future days ko absent nahi ginna

            var result = new List<MonthlyReportItem>();

            foreach (var emp in employees.Where(e => e != null))
            {
                var item = new MonthlyReportItem { EmployeeID = emp.EmployeeID, EmployeeName = emp.FullName, Year = year, Month = month };
                var empAttendance = allAttendance.Where(a => a.EmployeeID == emp.EmployeeID).ToDictionary(a => a.AttendanceDate.Date);
                var empLeaves = approvedLeaves.Where(l => l.EmployeeID == emp.EmployeeID).ToList();

                for (var day = new DateTime(year, month, 1);
      day <= uptoDay;
      day = day.AddDays(1))
                {
                    // =========================================================
                    // PRESENT
                    // =========================================================
                    if (empAttendance.TryGetValue(day.Date, out var att))
                    {
                        var totalHours = att.TotalHours ?? 0;

                        item.PresentDays++;
                        item.TotalHours += totalHours;

                        item.DailyBreakdown.Add(new DailyReportItem
                        {
                            Date = day,
                            Status = "Present",
                            Hours = totalHours,

                            CheckInTime = att.CheckInTime,
                            CheckOutTime = att.CheckOutTime
                        });
                    }

                    // =========================================================
                    // LEAVE
                    // =========================================================
                    else
                    {
                        var leave = empLeaves.FirstOrDefault(l =>
                            day.Date >= l.FromDate.Date &&
                            day.Date <= l.ToDate.Date);

                        if (leave != null)
                        {
                            item.LeaveDays++;

                            item.DailyBreakdown.Add(new DailyReportItem
                            {
                                Date = day,
                                Status = "Leave",
                                Hours = 0,

                                LeaveType = leave.LeaveType,

                                LeaveFromTime = leave.FromTime,
                                LeaveToTime = leave.ToTime,

                                LeaveHours = leave.Hours,

                                HalfDaySession = leave.HalfDaySession,

                                LeaveReason = leave.Reason,

                                RequestedBy = leave.RequestedBy
                            });
                        }

                        // =====================================================
                        // WEEKEND
                        // =====================================================
                        else if (day.DayOfWeek == DayOfWeek.Saturday ||
                                 day.DayOfWeek == DayOfWeek.Sunday)
                        {
                            item.DailyBreakdown.Add(new DailyReportItem
                            {
                                Date = day,
                                Status = "Weekend",
                                Hours = 0
                            });
                        }

                        // =====================================================
                        // ABSENT
                        // =====================================================
                        else
                        {
                            item.AbsentDays++;

                            item.DailyBreakdown.Add(new DailyReportItem
                            {
                                Date = day,
                                Status = "Absent",
                                Hours = 0
                            });
                        }
                    }
                }

                result.Add(item);
            }

            return result;
        }

        public byte[] ExportMonthlyReportToExcel(
            int year,
            int month,
            int? employeeId = null)
        {
            // Get monthly report
            var report = GetMonthlyReport(year, month, employeeId);

            // --new: pehle sirf Approved leaves aati thi, ab saari (Pending/Approved/Rejected/Cancelled)
            // us mahine ki leaves aayengi taake "kis ne kab kis time kislia leave li" sab dikh sake
            var allLeaves = _leaveRepo.GetAllForMonth(year, month);

            // Get employees
            var employees = employeeId.HasValue
                ? new List<Employee>
                {
            _employeeRepo.GetById(employeeId.Value)
                }
                : _employeeRepo.GetAll();


            using var workbook = new XLWorkbook();

            // =========================================================
            // 1. MONTHLY SUMMARY
            // =========================================================

            var summary = workbook.Worksheets.Add("Monthly Summary");

            summary.Cell(1, 1).Value = "Employee";
            summary.Cell(1, 2).Value = "Present Days";
            summary.Cell(1, 3).Value = "Leave Days";
            summary.Cell(1, 4).Value = "Absent Days";
            summary.Cell(1, 5).Value = "Total Hours";

            var summaryHeader = summary.Range("A1:E1");
            summaryHeader.Style.Font.Bold = true;
            summaryHeader.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            int summaryRow = 2;

            foreach (var item in report)
            {
                summary.Cell(summaryRow, 1).Value = item.EmployeeName;
                summary.Cell(summaryRow, 2).Value = item.PresentDays;
                summary.Cell(summaryRow, 3).Value = item.LeaveDays;
                summary.Cell(summaryRow, 4).Value = item.AbsentDays;

                summary.Cell(summaryRow, 5).Value = item.TotalHours;
                summary.Cell(summaryRow, 5).Style.NumberFormat.Format = "0.00";

                summaryRow++;
            }

            summary.Range(
                1,
                1,
                Math.Max(summaryRow - 1, 1),
                5
            ).Style.Border.OutsideBorder =
                XLBorderStyleValues.Thin;

            summary.Range(
                1,
                1,
                Math.Max(summaryRow - 1, 1),
                5
            ).Style.Border.InsideBorder =
                XLBorderStyleValues.Thin;

            summary.SheetView.FreezeRows(1);
            summary.Range("A1:E1").SetAutoFilter();

            summary.Column(1).Width = 25;
            summary.Column(2).Width = 15;
            summary.Column(3).Width = 15;
            summary.Column(4).Width = 15;
            summary.Column(5).Width = 15;


            // =========================================================
            // 2. DAILY DETAILS
            // =========================================================

            var detail = workbook.Worksheets.Add("Daily Details");

            detail.Cell(1, 1).Value = "Employee";
            detail.Cell(1, 2).Value = "Date";
            detail.Cell(1, 3).Value = "Day";
            detail.Cell(1, 4).Value = "Status";
            detail.Cell(1, 5).Value = "Check In";
            detail.Cell(1, 6).Value = "Check Out";
            detail.Cell(1, 7).Value = "Total Hours";
            detail.Cell(1, 8).Value = "Leave Type";
            detail.Cell(1, 9).Value = "Leave From Time";
            detail.Cell(1, 10).Value = "Leave To Time";
            detail.Cell(1, 11).Value = "Leave Hours";
            detail.Cell(1, 12).Value = "Half Day Session";
            detail.Cell(1, 13).Value = "Leave Reason";
            detail.Cell(1, 14).Value = "Requested By";

            var detailHeader = detail.Range("A1:N1");

            detailHeader.Style.Font.Bold = true;
            detailHeader.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            int detailRow = 2;

            foreach (var employeeReport in report)
            {
                if (employeeReport.DailyBreakdown == null)
                    continue;

                foreach (var d in employeeReport.DailyBreakdown)
                {
                    detail.Cell(detailRow, 1).Value =
                        employeeReport.EmployeeName;

                    detail.Cell(detailRow, 2).Value = d.Date;
                    detail.Cell(detailRow, 2).Style.NumberFormat.Format =
                        "dd-MMM-yyyy";

                    detail.Cell(detailRow, 3).Value =
                        d.DayName;

                    detail.Cell(detailRow, 4).Value =
                        d.Status;

                    // Check In
                    if (d.CheckInTime.HasValue)
                    {
                        detail.Cell(detailRow, 5).Value =
                            d.CheckInTime.Value;

                        detail.Cell(detailRow, 5).Style.NumberFormat.Format =
                            "hh:mm AM/PM";
                    }

                    // Check Out
                    if (d.CheckOutTime.HasValue)
                    {
                        detail.Cell(detailRow, 6).Value =
                            d.CheckOutTime.Value;

                        detail.Cell(detailRow, 6).Style.NumberFormat.Format =
                            "hh:mm AM/PM";
                    }

                    // Total Hours
                    detail.Cell(detailRow, 7).Value = d.Hours;
                    detail.Cell(detailRow, 7).Style.NumberFormat.Format =
                        "0.00";

                    // Leave Type
                    detail.Cell(detailRow, 8).Value =
                        d.LeaveType ?? "";

                    // Leave From Time
                    if (d.LeaveFromTime.HasValue)
                    {
                        detail.Cell(detailRow, 9).Value =
                            DateTime.Today.Add(d.LeaveFromTime.Value);

                        detail.Cell(detailRow, 9).Style.NumberFormat.Format =
                            "hh:mm AM/PM";
                    }

                    // Leave To Time
                    if (d.LeaveToTime.HasValue)
                    {
                        detail.Cell(detailRow, 10).Value =
                            DateTime.Today.Add(d.LeaveToTime.Value);

                        detail.Cell(detailRow, 10).Style.NumberFormat.Format =
                            "hh:mm AM/PM";
                    }

                    // Leave Hours
                    if (d.LeaveHours.HasValue)
                    {
                        detail.Cell(detailRow, 11).Value =
                            d.LeaveHours.Value;

                        detail.Cell(detailRow, 11).Style.NumberFormat.Format =
                            "0.00";
                    }

                    // Half Day Session
                    detail.Cell(detailRow, 12).Value =
                        d.HalfDaySession ?? "";

                    // Reason
                    detail.Cell(detailRow, 13).Value =
                        d.LeaveReason ?? "";

                    // Requested By
                    detail.Cell(detailRow, 14).Value =
                        d.RequestedBy ?? "";

                    detailRow++;
                }
            }

            if (detailRow > 2)
            {
                var detailRange = detail.Range(
                    1,
                    1,
                    detailRow - 1,
                    14
                );

                detailRange.Style.Border.OutsideBorder =
                    XLBorderStyleValues.Thin;

                detailRange.Style.Border.InsideBorder =
                    XLBorderStyleValues.Thin;

                detail.Range("A1:N1").SetAutoFilter();
            }

            detail.SheetView.FreezeRows(1);

            detail.Column(1).Width = 25;
            detail.Column(2).Width = 15;
            detail.Column(3).Width = 15;
            detail.Column(4).Width = 15;
            detail.Column(5).Width = 15;
            detail.Column(6).Width = 15;
            detail.Column(7).Width = 15;
            detail.Column(8).Width = 18;
            detail.Column(9).Width = 18;
            detail.Column(10).Width = 18;
            detail.Column(11).Width = 15;
            detail.Column(12).Width = 20;
            detail.Column(13).Width = 40;
            detail.Column(14).Width = 20;


            // =========================================================
            // 3. LEAVE DETAILS
            // =========================================================

            var leaveSheet = workbook.Worksheets.Add("Leave Details");

            leaveSheet.Cell(1, 1).Value = "Employee";
            leaveSheet.Cell(1, 2).Value = "Leave Type";
            leaveSheet.Cell(1, 3).Value = "From Date";
            leaveSheet.Cell(1, 4).Value = "To Date";
            leaveSheet.Cell(1, 5).Value = "From Time";
            leaveSheet.Cell(1, 6).Value = "To Time";
            leaveSheet.Cell(1, 7).Value = "Hours";
            leaveSheet.Cell(1, 8).Value = "Half Day Session";
            leaveSheet.Cell(1, 9).Value = "Reason";
            leaveSheet.Cell(1, 10).Value = "Requested By";
            leaveSheet.Cell(1, 11).Value = "Status"; // --new: ab yahan Pending/Approved/Rejected sab aayega

            var leaveHeader = leaveSheet.Range("A1:K1");
            leaveHeader.Style.Font.Bold = true;
            leaveHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int leaveRow = 2;

            // --new: approvedLeaves ki jagah ab allLeaves loop ho raha hai
            foreach (var leave in allLeaves)
            {
                // If specific employee selected
                if (employeeId.HasValue && leave.EmployeeID != employeeId.Value)
                {
                    continue;
                }

                var employee = employees.FirstOrDefault(
                    e => e != null && e.EmployeeID == leave.EmployeeID
                );

                leaveSheet.Cell(leaveRow, 1).Value = employee?.FullName ?? leave.EmployeeName ?? "";
                leaveSheet.Cell(leaveRow, 2).Value = leave.LeaveType ?? "";

                leaveSheet.Cell(leaveRow, 3).Value = leave.FromDate;
                leaveSheet.Cell(leaveRow, 3).Style.NumberFormat.Format = "dd-MMM-yyyy";

                leaveSheet.Cell(leaveRow, 4).Value = leave.ToDate;
                leaveSheet.Cell(leaveRow, 4).Style.NumberFormat.Format = "dd-MMM-yyyy";

                if (leave.FromTime.HasValue)
                {
                    leaveSheet.Cell(leaveRow, 5).Value = DateTime.Today.Add(leave.FromTime.Value);
                    leaveSheet.Cell(leaveRow, 5).Style.NumberFormat.Format = "hh:mm AM/PM";
                }

                if (leave.ToTime.HasValue)
                {
                    leaveSheet.Cell(leaveRow, 6).Value = DateTime.Today.Add(leave.ToTime.Value);
                    leaveSheet.Cell(leaveRow, 6).Style.NumberFormat.Format = "hh:mm AM/PM";
                }

                if (leave.Hours.HasValue)
                {
                    leaveSheet.Cell(leaveRow, 7).Value = leave.Hours.Value;
                    leaveSheet.Cell(leaveRow, 7).Style.NumberFormat.Format = "0.00";
                }

                leaveSheet.Cell(leaveRow, 8).Value = leave.HalfDaySession ?? "";
                leaveSheet.Cell(leaveRow, 9).Value = leave.Reason ?? "";
                leaveSheet.Cell(leaveRow, 10).Value = leave.RequestedBy ?? "";
                leaveSheet.Cell(leaveRow, 11).Value = leave.Status ?? "";

                leaveRow++;
            }

            if (leaveRow > 2)
            {
                var leaveRange = leaveSheet.Range(1, 1, leaveRow - 1, 11);
                leaveRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                leaveRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                leaveSheet.Range("A1:K1").SetAutoFilter();
            }

            leaveSheet.SheetView.FreezeRows(1);

            leaveSheet.Column(1).Width = 25;
            leaveSheet.Column(2).Width = 18;
            leaveSheet.Column(3).Width = 15;
            leaveSheet.Column(4).Width = 15;
            leaveSheet.Column(5).Width = 15;
            leaveSheet.Column(6).Width = 15;
            leaveSheet.Column(7).Width = 12;
            leaveSheet.Column(8).Width = 20;
            leaveSheet.Column(9).Width = 40;
            leaveSheet.Column(10).Width = 20;
            leaveSheet.Column(11).Width = 15;

            // =========================================================
            // SAVE EXCEL
            // =========================================================

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
