using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleCrm.IRepository;
using SimpleCrm.Models;
using System.Diagnostics;

namespace SimpleCrm.Controllers
{
    [Authorize]
    public class HomeController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public ActionResult Index()
        {

            return View();
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
