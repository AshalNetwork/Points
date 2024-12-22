using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using SimpleCrm.Contexts;
using SimpleCrm.IRepository;
using SimpleCrm.Models;
using SimpleCrm.Specification;

namespace SimpleCrm.Controllers
{
    public class AttendancesController(IUnitOfWork unitOfWork) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            return View(await unitOfWork.Repository<Attendance>().GetAllWithSpecAsync( new GetAttendanceSpec()));
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] string userId)
        {
            var attendances = (await unitOfWork.Repository<Attendance>().GetBYPropAsync(new GetUserAttendanceSpec(userId, DateTime.Now)));
            
            ViewBag.Attendance = attendances is null ? new TimeSpan(): attendances.CheckIn;
            if (attendances is null)
            {
                await unitOfWork.Repository<Attendance>().
                Add(new Attendance
                {
                    Date = DateTime.Now.Date,
                    ApplicationUserId = userId,
                    CheckIn = DateTime.Now.TimeOfDay,
                });
            }
            else
            {
                if (attendances.CheckOut != new TimeSpan())
                {
                    return View("لا يمكن تسجيل حضور او انصراف");
                }
                else
                    attendances.CheckOut = DateTime.Now.TimeOfDay;
            } 
            await unitOfWork.Complete();
            return Ok();
        }
    }
}
