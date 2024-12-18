
using Microsoft.EntityFrameworkCore;
using SimpleCrm.Contexts;
using SimpleCrm.Enums;
using SimpleCrm.Models;
using SimpleCrm.Service;
using SimpleCrm.Services;
using System.Data;

namespace ashal.Services
{
    public class UserService(PointsDbContext context, IEmailService emailService) : IUserService
    {
        private readonly PointsDbContext _context = context;
        private readonly IEmailService _emailService = emailService;

        public async Task SendVerificationnCode(string userId, string userEmail,string body)
        {
            // generate random 6 digits...
            Random generator = new Random();
            string code = generator.Next(0, 1000000).ToString("D6");

            // insert the code to the database
            var validationCode = new ValidationCode()
            {
                UserId = userId,
                Code = code,
                GeneratedDate = DateTime.Now,
                ExpirationDate = DateTime.Now.AddMinutes(5)
            };

            await _context.ValidationCodes.AddAsync(validationCode);
            await _context.SaveChangesAsync();

            // send the code to the user
            var subject = "Verification Code";

            await _emailService.SendEmailAsync(new List<string> { userEmail }, subject, body);

        }

        public async Task<VerificationCodeValidationResult> ValidateVerificationCode(string userId, string verificationCode)
        {
            var validationCode = await _context.ValidationCodes
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.GeneratedDate)
                .FirstOrDefaultAsync();

            if (validationCode == null)
            {
                return VerificationCodeValidationResult.Invalid;
            }
            else if (validationCode.ExpirationDate < DateTime.Now)
            {
                return VerificationCodeValidationResult.Expired;
            }
            else if (validationCode.Code != verificationCode)
            {
                return VerificationCodeValidationResult.Invalid;
            }

            return VerificationCodeValidationResult.Valid;
        }


    }
}

