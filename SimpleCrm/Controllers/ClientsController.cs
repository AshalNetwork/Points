using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleCrm.Contexts;
using SimpleCrm.IRepository;
using SimpleCrm.Models;
using SimpleCrm.Specification;
using SimpleCrm.VM;
using System.Data;
using System.Security.Claims;

namespace SimpleCrm.Controllers
{
    [Authorize]
    public class ClientsController(IUnitOfWork unitOfWork, PointsDbContext context, ILogger<HomeController> logger, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly PointsDbContext _context = context;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<ActionResult> Index(string searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var ClientsList = new List<ClientsToReturnDto>();
            var Clients = new List<Clients>();

            if (User.IsInRole("Admin"))
            {
                 Clients = _unitOfWork.Repository<Clients>().GetAllWithSpecAsync(new GetAdminFollowingClientsSpec()).Result.ToList();
            }
            if (User.IsInRole("User"))
            {
                 Clients = _unitOfWork.Repository<Clients>().GetAllWithSpecAsync(new GetUserFollowingClientsSpec(userId)).Result.ToList();
            }
            var followedClients = await _unitOfWork.Repository<FollowedClient>().GetAllWithSpecAsync(new GetFollowedClientsToMeSpec(userId));
            Clients.AddRange(followedClients.Select(z => z.Client).Where(z => z.Status == Enums.ClientStatusEnum.Following).ToList());


            var users = await _userManager.GetUsersInRoleAsync("User");
            ViewBag.users = users.Where(z => z.Id != userId); ;

            foreach (var client in Clients)
            {
                var user = await _userManager.FindByIdAsync(client.UserId);
                ClientsList.Add(new ClientsToReturnDto
                {
                    Id = client.Id,
                    Name = client.ClientName,
                    Phone = client.ClientPhone,
                    Description =client.Description ?? string.Empty,
                    Reason =client.CommingReason?.Reason == "Custom" ?client.CustomReason : client.CommingReason?.Reason,
                    CreatedAt = client.CreatedAt.ToString(),
                    UpdatedAt = client.UpdatedAt.ToString() ?? string.Empty,
                    CreatedBy = user?.Name ??string.Empty,
                    UserId = client.UserId
                });
            }

            int totalItems = ClientsList.Count();
            var paginatedItems = new List<ClientsToReturnDto>();
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

            var viewModel = new PaginatedViewModel<ClientsToReturnDto>
            {
                Items = paginatedItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems
            };
            return View(viewModel);

        }
        public async Task<ActionResult> Cancelled(string searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var ClientsList = new List<ClientsToReturnDto>();
            var Clients = new List<Clients>();
            if (User.IsInRole("Admin"))
            {
                Clients = _unitOfWork.Repository<Clients>().GetAllWithSpecAsync(new GetAdminCancelledClientsSpec()).Result.ToList();
            }
            if (User.IsInRole("User"))
            {
                Clients = _unitOfWork.Repository<Clients>().GetAllWithSpecAsync(new GetUserCancelledClientsSpec(userId)).Result.ToList();
            }
            var followedClients = await _unitOfWork.Repository<FollowedClient>().GetAllWithSpecAsync(new GetFollowedClientsToMeSpec(userId));
            Clients.AddRange(followedClients.Select(z => z.Client).Where(z => z.Status == Enums.ClientStatusEnum.Deleted).ToList());

            foreach (var client in Clients)
            {
                var user = await _userManager.FindByIdAsync(client.UserId);
                ClientsList.Add(new ClientsToReturnDto
                {
                    Id = client.Id,
                    Name = client.ClientName,
                    Phone = client.ClientPhone,
                    Description = client.Description??string.Empty,
                    Reason = client.CommingReason?.Reason == "Custom" ? client.CustomReason : client.CommingReason?.Reason,
                    CreatedAt = client.CreatedAt.ToString(),
                    UpdatedAt = client.UpdatedAt.ToString() ?? string.Empty,
                    CreatedBy = user?.Name ?? string.Empty,
                    UserId = client.UserId
                });
            }
            int totalItems = ClientsList.Count();
            var paginatedItems = new List<ClientsToReturnDto>();
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
            var viewModel = new PaginatedViewModel<ClientsToReturnDto>
            {
                Items = paginatedItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems
            };
            return View(viewModel);
        }

        public async Task<ActionResult> Add()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddClientDto dto)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var client = new Clients
            {
                Id = Guid.NewGuid(),
                ClientName = dto.Name,
                ClientPhone = dto.Phone,
                Description = dto.Description,
                Status = Enums.ClientStatusEnum.Following,
                CreatedAt = DateTime.Now,
                UserId = userId,
                CommingReasonId = dto.ReasonId,
                CustomReason = dto.CustomReason

            };
            try
            {
                await _unitOfWork.Repository<Clients>().Add(client);
                await _unitOfWork.Complete();
                return RedirectToAction("Index", "Clients");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return View(client);
            }
        }
        [HttpGet]
        public async Task<ActionResult> Edit(Guid Id)
        {
            var clients = await _unitOfWork.Repository<Clients>().GetBYIdAsync(Id);
            return View(clients);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid Id, Clients dto)
        {

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var reason = await _unitOfWork.Repository<CommingReason>().GetBYIdAsync((Guid)dto.CommingReasonId);
            var Client = await _unitOfWork.Repository<Clients>().GetBYIdAsync(Id);

            Client.ClientName = dto.ClientName;
            Client.ClientPhone = dto.ClientPhone;
            Client.Description = dto.Description;
            Client.UpdatedAt = DateTime.Now;
            Client.CommingReasonId = dto.CommingReasonId;
            if (reason.Reason == "Custom")
                Client.CustomReason = dto.CustomReason;
            else
                Client.CustomReason = null;

            try
            {
                _unitOfWork.Repository<Clients>().Update(Client);
                await _unitOfWork.Complete();
                return RedirectToAction("Index", "Clients");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return View(Client);
            }
        }
        [HttpPut]
        public async Task<ActionResult> Delete(Guid Id)
        {
            var Client = await _unitOfWork.Repository<Clients>().GetBYIdAsync(Id);
            Client.Status = Enums.ClientStatusEnum.Deleted;

            try
            {
                _unitOfWork.Repository<Clients>().Update(Client);
                await _unitOfWork.Complete();
                return RedirectToAction("Index", "Clients");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return RedirectToAction("Index", "Clients");

            }

        }
        [HttpPut]
        public async Task<ActionResult> Follow(Guid Id)
        {
            var Client = await _unitOfWork.Repository<Clients>().GetBYIdAsync(Id);
            Client.Status = Enums.ClientStatusEnum.Following;

            try
            {
                _unitOfWork.Repository<Clients>().Update(Client);
                await _unitOfWork.Complete();
                return RedirectToAction("Index", "Clients");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return View(Client);
            }

        }

        [HttpPost]
        public async Task<IActionResult> ForwardClient(Guid clientId, Guid forwardedToUserId)
        {
            string LoggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // Validate input
            if (clientId == Guid.Empty || forwardedToUserId == Guid.Empty || string.IsNullOrEmpty(LoggedUserId))
            {
                return BadRequest("Invalid client or user ID.");
            }

            var client = await _unitOfWork.Repository<Clients>().GetBYIdAsync(clientId);
            if (client == null)
            {
                return NotFound("Client not found.");
            }
            try
            {
                var model = new FollowedClient
                {
                    ClientId = clientId,
                    FromId = LoggedUserId,
                    ToId = forwardedToUserId.ToString()
                };
                await _unitOfWork.Repository<FollowedClient>().Add(model);
                await _unitOfWork.Complete();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportClientsToExcel()
        {
            // Get the client data from the database (adjust this according to your data access pattern)
            var clients = await _unitOfWork.Repository<Clients>().GetAllWithSpecAsync(new ClientsToExportSpec()); // Assume ClientRepository has a GetAll method to retrieve all clients.

            // Create a new DataTable to hold the data
            DataTable dt = new DataTable("Clients");
            dt.Columns.AddRange(new DataColumn[3]
            {
                new DataColumn("ClientName"),
                new DataColumn("ClientPhone"),
                new DataColumn("Reason")

            });

            // Add client data to the DataTable
            foreach (var client in clients)
            {
                var reason = string.Empty;
                if (client.CommingReason?.Reason == "Custom")
                    reason = client.CustomReason;
                else
                    reason = client.CommingReason?.Reason;
                dt.Rows.Add(client.ClientName, client.ClientPhone,reason);
            }

            // Create a new Excel workbook and add the data
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(dt);

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    // Return the file as an Excel download
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Clients.xlsx");
                }
            }
        }

        }
}
