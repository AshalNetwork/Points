using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SimpleCrm.Contexts;
using SimpleCrm.IRepository;
using SimpleCrm.Models;
using SimpleCrm.Specification;
using SimpleCrm.VM;
using System.Buffers;
using System.Diagnostics;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SimpleCrm.Controllers
{
    [Authorize]
    public class HomeController(IUnitOfWork unitOfWork,  UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager=userManager;

        public async Task<ActionResult> Index()
        {
            var modal = new StaticticsVM
            {
                UsersCount = _userManager.Users.Count(),
                ActiveClients = await _unitOfWork.Repository<Clients>().CountAsync(z => z.Status == Enums.ClientStatusEnum.Following),
                DeletedClients = await _unitOfWork.Repository<Clients>().CountAsync(z => z.Status == Enums.ClientStatusEnum.Deleted),
                Forwards = await _unitOfWork.Repository<FollowedClient>().CountAsync(z=>z.Id!=Guid.Empty)
            };
            return View(modal);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
