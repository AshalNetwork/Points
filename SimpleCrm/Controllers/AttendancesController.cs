﻿using ClosedXML.Excel;
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
            var user = await userManager.FindByIdAsync(User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier)!.Value);
            var attendances = unitOfWork.Repository<Attendance>().GetAllWithSpecAsync(new GetUserAttendanceSpec(user.Email, DateTime.Now)).Result.LastOrDefault();

            ViewBag.Attendance = attendances?.CheckIn ?? new TimeSpan();
            var egyptTimeZoneId = "Egypt Standard Time";
            var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById(egyptTimeZoneId);
            var egyptTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone);

            if (attendances == null)
            {
                await unitOfWork.Repository<Attendance>().Add(new Attendance
                {
                    Date = egyptTime.Date,
                    ApplicationUserId = userId,
                    CheckIn = egyptTime.TimeOfDay,
                });
            }
            else if (attendances.CheckOut == TimeSpan.Zero)
            {
                attendances.CheckOut = egyptTime.TimeOfDay;
            }
            else
            {
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
      
    }
}
