using System.ComponentModel.DataAnnotations;

namespace SimpleCrm.VM
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
