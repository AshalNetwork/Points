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
            ViewBag.Name = user?.Name ?? string.Empty;

            if (user == null)
            {
                return NotFound();
            }

            var today = DateTime.Today;
            
            var startOfWeek = today.AddDays(-(int)(today.DayOfWeek == DayOfWeek.Saturday ? 0 : today.DayOfWeek + 1));

            var endOfWeek = startOfWeek.AddDays(6);
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

            return View(DetailedPoints);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AddUserPointDto dto)
        {
            var TodayPointsRecord = await _unitOfWork.Repository<UserPoint>().GetCountWithSpecAsync(new BaseSpecification<UserPoint>(z => z.UserId == dto.UserId && z.DateTime.Date == DateTime.Now.Date && z.PointType == (PointsTypeEnum)dto.PointType));

            
            try
            {
                if (TodayPointsRecord == 0)
                {
                    var point = new UserPoint
                    {
                        DateTime = DateTime.Now,
                        value = dto.Value,
                        PointType = (PointsTypeEnum)dto.PointType,
                        UserId = dto.UserId,
                    };
                    await _unitOfWork.Repository<UserPoint>().Add(point);
                    await _unitOfWork.Complete();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var ExistedRecord = await _unitOfWork.Repository<UserPoint>().GetEntityWithSpecAsync(new BaseSpecification<UserPoint>(z => z.UserId == dto.UserId && z.DateTime.Date == DateTime.Now.Date && z.PointType == (PointsTypeEnum)dto.PointType));
                    ExistedRecord.value += dto.Value;
                    _unitOfWork.Repository<UserPoint>().Update(ExistedRecord);
                    await _unitOfWork.Complete();
                    return RedirectToAction("Index", "Home");
                }

            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return View();
            }
        }
    }
}
