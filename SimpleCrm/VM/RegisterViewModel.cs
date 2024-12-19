using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.AspNetCore.Mvc.Rendering;
using SimpleCrm.Enums;
using System.ComponentModel.DataAnnotations;

namespace SimpleCrm.VM
{
    public class RegisterViewModel
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public RolesEnum Role { get; set; }
        public ICollection<SelectListItem>? RolesList { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

}
