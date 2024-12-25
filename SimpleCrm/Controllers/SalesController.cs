using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    public class SalesController(IUnitOfWork _unitOfWork,UserManager<ApplicationUser> _userManager) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var sales = await _unitOfWork.Repository<Sale>().GetAllWithSpecAsync(new GetAllSalesSpec());
            var mappedSales = sales.Select(z => new AllSalesRequestsVM
            {
                Id = z.Id,
                User = z.User.Name,
                Description = z.Description??string.Empty,
                Status = z.Status.ToString(),
                Date = z.Date.ToString("yyyy-MMM-dd",CultureInfo.InvariantCulture).ToUpper(),
            }).ToList();
            return View(mappedSales);
        }
        public async Task<IActionResult> GetUserSales()
        {
            string userId = User.Claims.FirstOrDefault(z=>z.Type==ClaimTypes.NameIdentifier)!.Value;
            ViewBag.Name = _userManager.FindByIdAsync(userId).Result?.Name??string.Empty;
            var sales = await _unitOfWork.Repository<Sale>().GetAllWithSpecAsync(new GetAllSalesSpec(userId));

            var mappedSales = sales.Select(z => new AllSalesRequestsVM
            {
                Id = z.Id,
                User = z.User.Name,
                Description = z.Description ?? string.Empty,
                Status = z.Status.ToString(),
                Date = z.Date.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture).ToUpper(),
            }).ToList();
            return View(mappedSales);


        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AddSalesRequestDto dto)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var sale = new Sale
            {
                Date = DateTime.Now,
                Status = SalesRequestEnum.Pending,
                Description = dto.Description,
                UserId = userId,
            };
            try
            {
                await _unitOfWork.Repository<Sale>().Add(sale);
                await _unitOfWork.Complete();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return View(sale);
            }
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Accept(string Id)
        {
            var request = await _unitOfWork.Repository<Sale>().GetBYIdAsync(Guid.Parse(Id));
            try
            {
                request.Status = SalesRequestEnum.Accepted;
                _unitOfWork.Repository<Sale>().Update(request);
                await _unitOfWork.Complete();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return View(request);
            }
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Reject(string Id)
        {
            var request = await _unitOfWork.Repository<Sale>().GetBYIdAsync(Guid.Parse(Id));
            try
            {
                request.Status = SalesRequestEnum.Rejected;
                _unitOfWork.Repository<Sale>().Update(request);
                await _unitOfWork.Complete();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return View(request);
            }
        }
    }
}
