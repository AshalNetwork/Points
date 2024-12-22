using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using SimpleCrm.Contexts;
using SimpleCrm.IRepository;
using SimpleCrm.Models;
using SimpleCrm.Specification;
using System.Security.Claims;

namespace SimpleCrm.Controllers
{
    public class AttendancesController(IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManager) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            return View(await unitOfWork.Repository<Attendance>().GetAllWithSpecAsync( new GetAttendanceSpec()));
        }

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
        public async Task<ActionResult> GetUserAttendance(string UserId)
        {
            var attendances = await unitOfWork.Repository<Attendance>().
                GetAllWithSpecAsync(new GetMonthlyAttendances(UserId));
            return View(attendances);
        }
    }
}
