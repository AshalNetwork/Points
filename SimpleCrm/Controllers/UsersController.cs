using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SimpleCrm.Contexts;
using SimpleCrm.Enums;
using SimpleCrm.IRepository;
using SimpleCrm.Models;
using SimpleCrm.VM;

namespace SimpleCrm.Controllers
{
    //[Authorize]
    public class UsersController(SignInManager<ApplicationUser> signInManager,UserManager<ApplicationUser> userManager,IUnitOfWork unitOfWork, PointsDbContext context, ILogger<UsersController> logger) : Controller
    {
        private readonly ILogger<UsersController> _logger = logger;
        private readonly PointsDbContext _context = context;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager=userManager;
        private readonly SignInManager<ApplicationUser> _signInManager=signInManager;
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new List<UsersVm>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var mappedUsers = new UsersVm
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    Name = user.Name ?? string.Empty,
                    Phone = user.PhoneNumber ?? string.Empty,
                    Role = roles.FirstOrDefault()??string.Empty
                };
                userRoles.Add(mappedUsers); 
            }
            return View(userRoles);
        }
        public IActionResult Register()
        {
            var model = new RegisterViewModel
            {
                RolesList = Enum.GetValues(typeof(RolesEnum))
                   .Cast<RolesEnum>()
                   .Select(g => new SelectListItem
                   {
                       Value = ((int)g).ToString(),
                       Text = g.ToString()
                   }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { Name = model.Name,UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role.ToString());
                    return RedirectToAction("Index", "Users");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
    }
}
