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
    public class TasksController(IUnitOfWork _unitOfWork,UserManager<ApplicationUser> _userManager) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var Tasks = await _unitOfWork.Repository<Tasks>().GetAllWithSpecAsync(new GetAllTasksSpec());
            var mappedTasks = Tasks.Select(z => new AllTasksVM
            {
                Id = z.Id,
                Title = z.Title,
                Description = z.Description ??string.Empty,
                Status = z.Status.ToString(),
                User = z.User.Name,
                Role = _userManager.GetRolesAsync(z.User).Result.FirstOrDefault()??string.Empty,
                StartDate = z.StartAt.ToString("yyyy-MM-dd",CultureInfo.InvariantCulture).ToUpper(),
                EndDate = z.EndAt.ToString("yyyy-MM-dd",CultureInfo.InvariantCulture).ToUpper(),
            }).ToList();
            return View(mappedTasks);
        }
        public async Task<IActionResult> UserTasks()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var Tasks = await _unitOfWork.Repository<Tasks>().GetAllWithSpecAsync(new GetMyDayTasksSpec(userId));
            var mappedTasks = Tasks.Select(z => new GetMyDailyTasksVM
            {
                Id = z.Id,
                Title = z.Title,
                Description = z.Description ??string.Empty,
                Status = z.Status.ToString(),
                StartDate = z.StartAt.ToString("yyyy-MM-dd",CultureInfo.InvariantCulture).ToUpper(),
                EndDate = z.EndAt.ToString("yyyy-MM-dd",CultureInfo.InvariantCulture).ToUpper(),
            }).ToList();
            return View(mappedTasks);
        }
        public IActionResult Create()
        {
            var model = new CreateTaskVM
            {
                StartAt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-ddTHH:mm")),
                EndAt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-ddTHH:mm")),
                StatusList = Enum.GetValues(typeof(StatusEnums))
                   .Cast<StatusEnums>()
                   .Select(g => new SelectListItem
                   {
                       Value = ((int)g).ToString(),
                       Text = g.ToString()
                   }).ToList(),
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
        public async Task<IActionResult> Create(CreateTaskVM model)
        {
            if (ModelState.IsValid)
            {
                var task = new Tasks
                {
                    Title = model.Title,
                    Description = model.Description,
                    Status = model.Status,
                    StartAt = model.StartAt,
                    EndAt = model.EndAt,
                    UserId=model.UserId
                };
                try
                {
                    await _unitOfWork.Repository<Tasks>().Add(task);
                    await _unitOfWork.Complete();
                    return RedirectToAction("Index", "Tasks");
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
