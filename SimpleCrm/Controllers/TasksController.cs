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

        public async Task<IActionResult> GetUserTasksForAdmins(string UserId)
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
        [HttpPut]       
        public async Task<IActionResult> UnderReview(string TaskId)
        {

             var task = await _unitOfWork.Repository<Tasks>().GetBYIdAsync(Guid.Parse(TaskId));
            try
            {
                task.Status = StatusEnums.UnderReview;
                _unitOfWork.Repository<Tasks>().Update(task);
                await _unitOfWork.Complete();
                return RedirectToAction("UserTasks", "Tasks");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();

                return RedirectToAction("UserTasks", "Tasks");
            }

        }
        [HttpPut]       
        public async Task<IActionResult> InProgress(string TaskId)
        {

             var task = await _unitOfWork.Repository<Tasks>().GetBYIdAsync(Guid.Parse(TaskId));
            try
            {
                task.Status = StatusEnums.InProgress;
                _unitOfWork.Repository<Tasks>().Update(task);
                await _unitOfWork.Complete();
                return RedirectToAction("UserTasks", "Tasks");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();

                return RedirectToAction("UserTasks", "Tasks");
            }

        }
        [HttpPut]       
        public async Task<IActionResult> CompleteTask(string TaskId)
        {

             var task = await _unitOfWork.Repository<Tasks>().GetBYIdAsync(Guid.Parse(TaskId));
            var UserTasks = await _unitOfWork.Repository<Tasks>().GetCountWithSpecAsync(new BaseSpecification<Tasks>(z=>z.UserId==task.UserId&&z.StartAt.Date==DateTime.Now.Date));
            var Taskpoint = 170.0m / UserTasks;
            try
            {
                task.Status = StatusEnums.Completed;
                _unitOfWork.Repository<Tasks>().Update(task);
                var userPoint = new UserPoint
                {
                    DateTime = DateTime.Now,
                    UserId = task.UserId,
                    TaskId = task.Id,
                    value = Taskpoint,
                    PointType = PointsTypeEnum.Production,
                };
                await _unitOfWork.Repository<UserPoint>().Add(userPoint);

                await _unitOfWork.Complete();
                return RedirectToAction("UserTasks", "Tasks");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();

                return RedirectToAction("UserTasks", "Tasks");
            }

        }
        [HttpPost]
        public async Task<ActionResult> Delete(string Id)
        {
            var task = await _unitOfWork.Repository<Tasks>().GetBYIdAsync(Guid.Parse(Id));
            if (task == null)
            {
                return View();
            }
            try
            {
                var userpoints = await _unitOfWork.Repository<UserPoint>().GetAllWithSpecAsync(new BaseSpecification<UserPoint>(z=>z.TaskId==Guid.Parse(Id)));
                _unitOfWork.Repository<UserPoint>().DeleteRange(userpoints.ToList());
                _unitOfWork.Repository<Tasks>().Delete(task);
                await _unitOfWork.Complete();
                return RedirectToAction("Index", "Tasks");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return RedirectToAction("Index", "Tasks");
            }
        }
    }
}
