using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleCrm.IRepository;
using SimpleCrm.Models;
using SimpleCrm.Specification;
using System.Diagnostics;
using System.Security.Claims;

namespace SimpleCrm.Controllers
{
    [Authorize]
    public class HomeController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<ActionResult> Index()
        {
            var user = await _userManager.FindByIdAsync(User.Claims.FirstOrDefault(e=>e.Type==ClaimTypes.NameIdentifier)!.Value);
            ViewBag.Name = user?.Name ?? string.Empty;

            var attendances = await _unitOfWork.Repository<Attendance>().
                GetAllWithSpecAsync(new GetMonthlyAttendances(user.Id));
            return View(attendances);
        }

    }
}
