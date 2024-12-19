using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata;
using SimpleCrm.Enums;
using SimpleCrm.Models;
using System.ComponentModel.DataAnnotations;

namespace SimpleCrm.VM
{
    public class CreateTaskVM
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public StatusEnums Status { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string MinDate { get; set; } = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
        [Display(Name = "User")]
        public string UserId { get; set; }
        public ICollection<SelectListItem>? StatusList { get; set; }

        public ICollection<SelectListItem>? UsersList { get; set; }
    }
}
