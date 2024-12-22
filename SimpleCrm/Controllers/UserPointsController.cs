using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleCrm.Enums;
using SimpleCrm.IRepository;
using SimpleCrm.Models;
using SimpleCrm.Specification;
using SimpleCrm.VM;
using System.Globalization;
using System.Security.Claims;

namespace SimpleCrm.Controllers
{
    [Authorize]
    public class UserPointsController(IUnitOfWork _unitOfWork,UserManager<ApplicationUser> _userManager) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetUserPointsInDay(string UserId)
        {
            var user =await  _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                return NotFound();
            }

            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Saturday); // Start from Saturday
            var endOfWeek = startOfWeek.AddDays(6); // End at Thursday (inclusive)

            var points = await _unitOfWork.Repository<UserPoint>().GetAllWithSpecAsync(new GetUserPointsForAdminSpec(UserId, startOfWeek, endOfWeek));
            var DetailedPoints = points.GroupBy(p => p.DateTime.Date)
                .Select(g => new DetailedPointVM
                {
                    Production = g.Where(z=>z.PointType==PointsTypeEnum.Production).Sum(p => p.value),
                    Behavior = g.Where(z => z.PointType == PointsTypeEnum.Behavior).Sum(p => p.value),
                    Lateness = g.Where(z => z.PointType == PointsTypeEnum.Lateness).Sum(p => p.value),
                    Preject = g.Where(z => z.PointType == PointsTypeEnum.Custom).Sum(p => p.value),
                    Date = g.FirstOrDefault()!.DateTime.ToString("dddd ,yyyy-MM-dd", CultureInfo.InvariantCulture).ToUpper()
                }).ToList();


            ViewBag.WeeklySummation = DetailedPoints.Sum(p => p.Production + p.Behavior + p.Lateness +p.Preject);
            return View(DetailedPoints);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AddUserPointDto dto)
        {

            var point = new UserPoint
            {
                DateTime = DateTime.Now,
                value = dto.Value,
                PointType = (PointsTypeEnum)dto.PointType,
                UserId = dto.UserId,
            };
            try
            {
                await _unitOfWork.Repository<UserPoint>().Add(point);
                await _unitOfWork.Complete();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return View(point);
            }
        }
    }
}
