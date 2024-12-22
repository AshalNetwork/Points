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
    public class PenalitiesController(IUnitOfWork _unitOfWork, UserManager<ApplicationUser> _userManager) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var penalities = await _unitOfWork.Repository<Penality>().GetAllWithSpecAsync(new GetUserPenalitiesSpec());
            var mappedPenalities = penalities.Select(z => new UserPenalitiesVM
            {
                Id = z.Id,
                User = z.User.Name,
                Description = z.Description,
                Reason = z.Reason,
                Date = z.Date.ToString(),
            }).ToList();
            return View(mappedPenalities);
        }
        public async Task<IActionResult> UserPenalities(string? EmployeeId)
        {
            var UserId = string.Empty;
            if (EmployeeId != null)
                UserId = EmployeeId;
            else
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var penalities = await _unitOfWork.Repository<Penality>().GetAllWithSpecAsync(new GetUserPenalitiesSpec(UserId));
            var mappedPenalities = penalities.Select(z => new UserPenalitiesVM
            {
                Id = z.Id,
                Description = z.Description,
                Reason = z.Reason,
                Date = z.Date.ToString(),
            }).ToList();
            return View(mappedPenalities);
        }
        public IActionResult Create()
        {
            var model = new CreatePenalityDto
            {
                
                UsersList = _userManager.Users.Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.Name.ToString()
                }).ToList(),
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePenalityDto model)
        {

            if (ModelState.IsValid)
            {
                var penality = new Penality
                {
                    Description = model.Description,     
                    Reason = model.Reason,
                    UserId = model.UserId,
                    Date = DateTime.Now,
                };
                try
                {
                    await _unitOfWork.Repository<Penality>().Add(penality);
                    await _unitOfWork.Complete();
                    return RedirectToAction("Index", "Penalities");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Delete(string Id)
        {
            var penality = await _unitOfWork.Repository<Penality>().GetBYIdAsync(Guid.Parse(Id));
            if (penality == null)
            {
                return View();
            }
            try
            {
                _unitOfWork.Repository<Penality>().Delete(penality);
                await _unitOfWork.Complete();
                return RedirectToAction("Index", "Penalities");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return RedirectToAction("Index", "Penalities");
            }           
        }
    }
}
