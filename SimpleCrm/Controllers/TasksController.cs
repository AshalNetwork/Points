﻿using DocumentFormat.OpenXml.Spreadsheet;
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
        [Authorize(Roles = "ProductionMangerA,ProductionMangerB,OperationManger")]
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
        public async Task<IActionResult> UserTasks(string UserId)
        {
            ViewBag.Name = _userManager.FindByIdAsync(UserId).Result?.Name ?? string.Empty;

            var Tasks = await _unitOfWork.Repository<Tasks>().GetAllWithSpecAsync(new GetMyDayTasksSpec(UserId, StatusEnums.Pending));
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
        [Authorize(Roles = "ProductionMangerA,ProductionMangerB,OperationManger")]
        [HttpGet]
        public async Task<IActionResult> GetUserTasksForAdmins(string UserId, StatusEnums Status = StatusEnums.UnderReview)
        {
            ViewBag.Name = _userManager.FindByIdAsync(UserId).Result?.Name ?? string.Empty;

            var Tasks = await _unitOfWork.Repository<Tasks>().GetAllWithSpecAsync(new GetUserTasksForAdminSpec(UserId, Status));
            var mappedTasks = Tasks.Select(z => new GetMyDailyTasksVM
            {
                Id = z.Id,
                Title = z.Title,
                Description = z.Description ?? string.Empty,
                Status = z.Status.ToString(),
                CompletedBy = z.CompletedBy != null ? (_userManager.FindByIdAsync(z.CompletedBy).Result?.Name ?? string.Empty) : string.Empty,
                StartDate = z.StartAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture).ToUpper(),
                EndDate = z.EndAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture).ToUpper(),
            }).ToList();
            return View();
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
        public async Task<IActionResult> Shift(string TaskId)
        {

             var task = await _unitOfWork.Repository<Tasks>().GetBYIdAsync(Guid.Parse(TaskId));
            try
            {
                task.StartAt =task.StartAt.AddDays(1);
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
        [Authorize(Roles = "ProductionMangerA,ProductionMangerB,OperationManger")]
        [HttpPut]       
        public async Task<IActionResult> CompleteTask(string TaskId)
        {
            var UserId = User.Claims.FirstOrDefault(z=>z.Type==ClaimTypes.NameIdentifier)!.Value;
            var task = await _unitOfWork.Repository<Tasks>().GetBYIdAsync(Guid.Parse(TaskId));
            var UserTasksCount = await _unitOfWork.Repository<Tasks>().GetCountWithSpecAsync(new BaseSpecification<Tasks>(z=>z.UserId==task.UserId&&z.StartAt.Date==DateTime.Now.Date));
            var UserCompletedTasksCount = await _unitOfWork.Repository<Tasks>().GetCountWithSpecAsync(new BaseSpecification<Tasks>(z=>z.UserId==task.UserId&&z.StartAt.Date==DateTime.Now.Date&&z.Status==StatusEnums.Completed));
            var Taskpoint = 170.0m / UserTasksCount;
            var CompletedTaskspoint = Taskpoint * ++UserCompletedTasksCount;
            var TodayPointsRecord = await _unitOfWork.Repository<UserPoint>().GetCountWithSpecAsync(new BaseSpecification<UserPoint>(z => z.UserId == task.UserId && z.DateTime.Date == DateTime.Now.Date&&z.PointType==PointsTypeEnum.Production));
           
            try
            {

                if (TodayPointsRecord == 0)
                {
                    task.Status = StatusEnums.Completed;
                    _unitOfWork.Repository<Tasks>().Update(task);
                    var userPoint = new UserPoint
                    {
                        DateTime = DateTime.Now,
                        UserId = task.UserId,
                        value = CompletedTaskspoint,
                        PointType = PointsTypeEnum.Production,
                    };
                    await _unitOfWork.Repository<UserPoint>().Add(userPoint);

                    await _unitOfWork.Complete();
                    return RedirectToAction("UserTasks", "Tasks");
                }
                else
                {
                    task.CompletedBy = UserId;
                    task.Status = StatusEnums.Completed;
                    _unitOfWork.Repository<Tasks>().Update(task);

                    var ExistedPoint = await _unitOfWork.Repository<UserPoint>().GetEntityWithSpecAsync(new BaseSpecification<UserPoint>(z => z.UserId == task.UserId && z.DateTime.Date == DateTime.Now.Date && z.PointType == PointsTypeEnum.Production));
                    ExistedPoint.value = CompletedTaskspoint;
                    _unitOfWork.Repository<UserPoint>().Update(ExistedPoint);
                    await _unitOfWork.Complete();
                    return RedirectToAction("UserTasks", "Tasks");
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();

                return RedirectToAction("UserTasks", "Tasks");
            }

        }
        [Authorize(Roles = "ProductionMangerA,ProductionMangerB,OperationManger")]
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
                //var userpoints = await _unitOfWork.Repository<UserPoint>().GetAllWithSpecAsync(new BaseSpecification<UserPoint>(z=>z.TaskId==Guid.Parse(Id)));
               // _unitOfWork.Repository<UserPoint>().DeleteRange(userpoints.ToList());
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
        [HttpGet("yu")]
        public async Task<List<GetMyDailyTasksVM>> ss()
        {
            var Tasks = await _unitOfWork.Repository<Tasks>().GetAllWithSpecAsync(new GetMyDayTasksSpec("f9b614bb-ab17-4c6b-b895-164bfad060dc"));
            var mappedTasks = Tasks.Select(z => new GetMyDailyTasksVM
            {
                Id = z.Id,
                Title = z.Title,
                Description = z.Description ?? string.Empty,
                Status = z.Status.ToString(),
                StartDate = z.StartAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture).ToUpper(),
                EndDate = z.EndAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture).ToUpper(),
            }).ToList();
            return (mappedTasks);
        }
    }
}
