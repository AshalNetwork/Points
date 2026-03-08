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
using SimpleCrm.VM;
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
            var egyptTimeZoneId = "Egypt Standard Time";
            var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById(egyptTimeZoneId);
            var egyptTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone);

            // Find today's record that has check-in but no check-out (same record we update on "Leave")
            var openAttendance = await unitOfWork.Repository<Attendance>()
                .GetEntityWithSpecAsync(new GetTodayOpenAttendanceSpec(userId, egyptTime));

            if (openAttendance != null)
            {
                // User is checking out: update the same record with check-out time
                openAttendance.CheckOut = egyptTime.TimeOfDay;
                unitOfWork.Repository<Attendance>().Update(openAttendance);
            }
            else
            {
                // User is checking in: create one new record with check-in only
                await unitOfWork.Repository<Attendance>().Add(new Attendance
                {
                    Date = egyptTime.Date,
                    ApplicationUserId = userId,
                    CheckIn = egyptTime.TimeOfDay,
                });
            }

            await unitOfWork.Complete();
            return RedirectToAction("UserTasks", "Tasks");
        }
        [Authorize]
        public async Task<ActionResult> GetUserAttendance(string UserId)
        {
            ViewBag.Name = userManager.FindByIdAsync(UserId).Result?.Name ?? string.Empty;
            ViewBag.userId=UserId;
            var attendances = await unitOfWork.Repository<Attendance>().
                GetAllWithSpecAsync(new GetMonthlyAttendances(UserId));
            return View(attendances);
        }

        [Authorize(Roles = "ProductionMangerA,ProductionMangerB,OperationManger")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var attendance = await unitOfWork.Repository<Attendance>().GetBYIdAsync(id);
            if (attendance is null)
            {
                return NotFound();
            }

            var vm = new AttendanceEditVM
            {
                Id = attendance.Id,
                ApplicationUserId = attendance.ApplicationUserId,
                Date = attendance.Date,
                CheckIn = attendance.CheckIn,
                CheckOut = attendance.CheckOut,
            };

            return View(vm);
        }

        [Authorize(Roles = "ProductionMangerA,ProductionMangerB,OperationManger")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AttendanceEditVM model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var attendance = await unitOfWork.Repository<Attendance>().GetBYIdAsync(id);
            if (attendance is null)
            {
                return NotFound();
            }

            attendance.Date = model.Date;
            attendance.CheckIn = model.CheckIn;
            attendance.CheckOut = model.CheckOut;

            unitOfWork.Repository<Attendance>().Update(attendance);
            await unitOfWork.Complete();

            return RedirectToAction(nameof(GetUserAttendance), new { UserId = attendance.ApplicationUserId });
        }

        [Authorize(Roles = "ProductionMangerA,ProductionMangerB,OperationManger")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var attendance = await unitOfWork.Repository<Attendance>().GetBYIdAsync(id);
            if (attendance is null)
            {
                return NotFound();
            }

            var userId = attendance.ApplicationUserId;

            unitOfWork.Repository<Attendance>().Delete(attendance);
            await unitOfWork.Complete();

            return RedirectToAction(nameof(GetUserAttendance), new { UserId = userId });
        }
      
    }
}
