using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using OfficeOpenXml;
using SimpleCrm.Contexts;
using SimpleCrm.IRepository;
using SimpleCrm.Models;
using SimpleCrm.Services;
using SimpleCrm.Specification;
using System.ComponentModel;
using System.Security.Claims;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace SimpleCrm.Controllers
{
    public class AttendancesController(IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManager) : Controller
    {
        [Authorize(Roles = "ProductionMangerA,ProductionMangerB,OperationManger")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            return View(await unitOfWork.Repository<Attendance>().GetAllWithSpecAsync( new GetAttendanceSpec()));
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Index([FromForm] string userId)
        {
            var user = await userManager.FindByIdAsync(User.Claims.FirstOrDefault(e=>e.Type==ClaimTypes.NameIdentifier)!.Value);
            var attendances = (await unitOfWork.Repository<Attendance>().GetBYPropAsync(new GetUserAttendanceSpec(user.Email, DateTime.Now)));
            
            ViewBag.Attendance = attendances is null ? new TimeSpan(): attendances.CheckIn;
            var egyptTimeZoneId = "Egypt Standard Time";
            var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById(egyptTimeZoneId);
            var egyptTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone);
            if (attendances is null)
            {
                await unitOfWork.Repository<Attendance>().
                Add(new Attendance
                {
                    Date = egyptTime.Date,
                    ApplicationUserId = userId,
                    CheckIn = egyptTime.TimeOfDay,
                });
            }
            else
            {
                if (attendances.CheckOut == new TimeSpan())
                    attendances.CheckOut = egyptTime.TimeOfDay;
            } 
            await unitOfWork.Complete();
            return Ok();
        }
        [Authorize]
        public async Task<ActionResult> GetUserAttendance(string UserId)
        {
            var attendances = await unitOfWork.Repository<Attendance>().
                GetAllWithSpecAsync(new GetMonthlyAttendances(UserId));
            return View(attendances);
        }
        [HttpPost]
        public async Task<ActionResult> ExportExcelSheet(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return NotFound();
            var attendances = await unitOfWork.Repository<Attendance>().
                GetAllWithSpecAsync(new GetMonthlyAttendances(userId));
            var dt = GenerateExcel.GenerateAttendanceSheet(attendances.ToList());
            // Create a new Excel workbook and add the data
            using (var workbook = new XLWorkbook())
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var worksheet = workbook.Worksheets.Add(dt);

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    workbook.SaveAs(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/AttendanceSheets", "sheet.xlsx"));
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "sheet.xlsx");
                }
            }
        }
    }
}
