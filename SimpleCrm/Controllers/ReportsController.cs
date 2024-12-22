using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class ReportsController(IUnitOfWork _unitOfWork) : Controller
    {
        public async Task<IActionResult> Index(string UserId)
        {
            var reports = await _unitOfWork.Repository<Report>().GetAllWithSpecAsync(new GetUserReportsSpec(UserId));
            var mappedReports = reports.Select(z => new UserReportsVM
            {
                Id = z.Id,
                Description = z.Description,
                Date = z.Date.ToString(),
            }).ToList();
            return View(mappedReports);
        }
        public async Task<IActionResult> UserReports()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var reports = await _unitOfWork.Repository<Report>().GetAllWithSpecAsync(new GetUserReportsSpec(userId));
            var mappedReports = reports.Select(z => new UserReportsVM
            {
                Id = z.Id,
                Description = z.Description,
                Date = z.Date.ToString(),
            }).ToList();
            return View(mappedReports);
        }
        public IActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReportDto model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            if (ModelState.IsValid)
            {
                var report = new Report
                {
                    Description = model.Description,                    
                    UserId = userId,
                    Date = DateTime.Now,
                };
                try
                {
                    await _unitOfWork.Repository<Report>().Add(report);
                    await _unitOfWork.Complete();
                    return RedirectToAction("UserReports", "Reports");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(model);
        }

    }
}
