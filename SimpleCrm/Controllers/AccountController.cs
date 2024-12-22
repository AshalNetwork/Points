using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleCrm.Enums;
using SimpleCrm.IRepository;
using SimpleCrm.Models;
using SimpleCrm.Service;
using SimpleCrm.Services;
using SimpleCrm.Specification;
using SimpleCrm.VM;
using System.Security.Claims;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace SimpleCrm.Controllers
{
    public class AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IEmailService emailService, IUserService userService, IUnitOfWork unitOfWork) : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IEmailService _emailService = emailService;
        private readonly IUserService _userService = userService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;


        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var attendance = (await unitOfWork.Repository<Attendance>().GetBYPropAsync(new GetUserAttendanceSpec(model.Email, DateTime.Now)));
                    var user = await _unitOfWork.Repository<ApplicationUser>().GetBYPropAsync(new GetUserByEmail(model.Email));
                    if (attendance is null)
                    {
                        var egyptTimeZoneId = "Egypt Standard Time";
                        var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById(egyptTimeZoneId);
                        var egyptTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone);
                        await unitOfWork.Repository<Attendance>().
                        Add(new Attendance
                        {   
                            Date = DateTime.Now.Date,
                            ApplicationUserId = user.Id,
                            CheckIn = egyptTime.TimeOfDay,
                        });
                    }
                  
                    await unitOfWork.Complete();
                    if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                    {
                        if (!User.IsInRole(RolesEnum.Employee.ToString()))
                        {
                            return RedirectToAction("Index", "Users");
                        }
                        else
                        {
                            return RedirectToAction("UserTasks", "Tasks");
                        }
                    }
                    return Redirect(returnUrl);    
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "InCorrect Email Or Password");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // User not found, display a generic message
                return View("ForgotPasswordConfirmation");
            }

            // Generate a password reset token
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Send the code via email
            var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
            await _userService.SendVerificationnCode(user.Id, user.Email, $"Please reset your password by <a href='{callbackUrl}'>clicking here</a>.");

            return View("ForgotPasswordConfirmation");
        }

        // Render the Reset Password form
        [HttpGet]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
                return BadRequest("A code must be supplied for password reset.");

            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        // Handle the Reset Password form submission
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            // Reset the password
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            // Show errors if password reset failed
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
        public IActionResult ResetPasswordConfirmation()
        {
            ViewBag.callbackUrl = Url.Action("Login", "Account", new { }, protocol: HttpContext.Request.Scheme);

            return View();
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        public async Task<ActionResult> ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ChangePassword(ChangePasswordVm model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View();
            }

            var currentResult = await _signInManager.CheckPasswordSignInAsync(user, model.OldPassword, false);
            if (!currentResult.Succeeded) return ViewBag.message = "Your Password In InCorrect";



            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public async Task<ActionResult> DeleteAccount(string UserId)
        {

            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                return View();
            }
           // var follower = await _unitOfWork.Repository<FollowedClient>().GetAllWithSpecAsync(new BaseSpecification<FollowedClient>(z => z.ToId == UserId || z.FromId == UserId));
           // _unitOfWork.Repository<FollowedClient>().DeleteRange(follower.ToList());
           // await _unitOfWork.Complete();
            // Delete user account
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                ModelState.AddModelError(string.Empty, result.Errors.ToString());
            return RedirectToAction("Index", "Users");

        }
        [HttpPost]
        public async Task Checkout()
        {
            var user = await _userManager.FindByIdAsync( User.Claims.FirstOrDefault()!.Value);
            if (user != null)
            {
                  var egyptTimeZoneId = "Egypt Standard Time";
                var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById(egyptTimeZoneId);
                var egyptTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone);
                var attendance=await _unitOfWork.Repository<Attendance>().GetBYPropAsync(new GetUserAttendanceSpec(user.Email, egyptTime.Date));
                attendance.CheckOut = egyptTime.TimeOfDay;
                await _unitOfWork.Complete();
            }
        }
    }
}
