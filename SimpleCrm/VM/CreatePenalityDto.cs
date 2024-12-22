using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SimpleCrm.VM
{
    public class CreatePenalityDto
    {
        public string Description { get; set; }
        public string Reason { get; set; }
        [Display(Name = "User")]
        public string UserId { get; set; }
        public ICollection<SelectListItem>? UsersList { get; set; }

    }
}
