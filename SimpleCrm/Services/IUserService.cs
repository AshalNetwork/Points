

using SimpleCrm.Enums;

namespace SimpleCrm.Service
{
    public interface IUserService
    {      
        Task SendVerificationnCode(string userId, string userEmail,string body);
        Task<VerificationCodeValidationResult> ValidateVerificationCode(string userId, string verificationCode);

    }
}
