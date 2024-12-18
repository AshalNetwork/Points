using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleCrm.Contexts;
using SimpleCrm.IRepository;
using SimpleCrm.Models;
using SimpleCrm.Specification;
using SimpleCrm.VM;
using System.Security.Claims;

namespace SimpleCrm.Controllers
{
    public class FollowedClientsController(IUnitOfWork unitOfWork, PointsDbContext context, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<ActionResult> Index(string searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            var ClientsList = new List<FollowedClientsDto>();
            var FollowedClients =  await _unitOfWork.Repository<FollowedClient>().GetAllWithSpecAsync(new GetFollowedClientsSpec());
 
            foreach (var followed in FollowedClients)
            {
                var from = await _userManager.FindByIdAsync(followed.FromId);
                var to = await _userManager.FindByIdAsync(followed.ToId);
                ClientsList.Add(new FollowedClientsDto
                {
                    Id = followed.Id,
                    Name = followed.Client.ClientName,
                    Phone = followed.Client.ClientPhone,
                    From =from?.Name ??string.Empty,
                    To = to?.Name??string.Empty,
                });
            }

            int totalItems = ClientsList.Count();
            var paginatedItems = new List<FollowedClientsDto>();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                paginatedItems = ClientsList.Where(z => z.Phone.Contains(searchQuery)).Skip((pageNumber - 1) * pageSize)
                               .Take(pageSize)
                               .ToList();
            }
            else
            {
                paginatedItems = ClientsList.Skip((pageNumber - 1) * pageSize)
                               .Take(pageSize)
                               .ToList();
            }

            var viewModel = new PaginatedViewModel<FollowedClientsDto>
            {
                Items = paginatedItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems
            };
            return View(viewModel);

        }
        [HttpPut]
        public async Task<ActionResult> Delete(Guid Id)
        {
            var followedClient = await _unitOfWork.Repository<FollowedClient>().GetBYIdAsync(Id);

            try
            {
                _unitOfWork.Repository<FollowedClient>().Delete(followedClient);
                await _unitOfWork.Complete();
                return RedirectToAction("Index", "Clients");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return RedirectToAction("Index", "Clients");

            }

        }
    }
}
