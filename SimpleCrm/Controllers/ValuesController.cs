using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleCrm.IRepository;
using SimpleCrm.Specification;
using SimpleCrm.VM;
using System.Globalization;
using System.Security.Claims;
using SimpleCrm.Models;
using SimpleCrm.Enums;
using Microsoft.AspNetCore.Identity;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SimpleCrm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController(IUnitOfWork _unitOfWork,UserManager<ApplicationUser> _userManager) : ControllerBase
    {
        [HttpGet("s")]
        public async Task<IActionResult> Get()
        {
            string userId = "f9b614bb-ab17-4c6b-b895-164bfad060dc";
            var Tasks = await _unitOfWork.Repository<Tasks>().GetAllWithSpecAsync(new GetMyDayTasksSpec(userId));
            return  Ok(Tasks.Select(z => new GetMyDailyTasksVM
            {
                Id = z.Id,
                Title = z.Title,
                Description = z.Description ?? string.Empty,
                Status = z.Status.ToString(),
                StartDate = z.StartAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture).ToUpper(),
                EndDate = z.EndAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture).ToUpper(),
            }).ToList());
        }
        [HttpGet("GetUserTasksForAdmins")]
        public async Task<List<GetMyDailyTasksVM>> GetUserTasksForAdmins(string UserId,StatusEnums Status = StatusEnums.UnderReview)
        {
            var Tasks = await _unitOfWork.Repository<Tasks>().GetAllWithSpecAsync(new GetUserTasksForAdminSpec(UserId,Status));
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
            return (mappedTasks);
        }
        [HttpGet("UserTasks")]
        public async Task<List<GetMyDailyTasksVM>> UserTasks(string UserId, StatusEnums Status = StatusEnums.Pending)
        {
            //ViewBag.Name = _userManager.FindByIdAsync(UserId).Result?.Name ?? string.Empty;

            var Tasks = await _unitOfWork.Repository<Tasks>().GetAllWithSpecAsync(new GetMyDayTasksSpec(UserId, Status));
            var mappedTasks = Tasks.Select(z => new GetMyDailyTasksVM
            {
                Id = z.Id,
                Title = z.Title,
                Description = z.Description ?? string.Empty,
                Status = z.Status.ToString(),
                StartDate = z.StartAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture).ToUpper(),
                EndDate = z.EndAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture).ToUpper(),
            }).ToList();
            return(mappedTasks);
        }
    }
}
