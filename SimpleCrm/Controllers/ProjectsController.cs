using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SimpleCrm.IRepository;
using SimpleCrm.Models;
using SimpleCrm.Specification;
using SimpleCrm.VM;
using System.Security.Claims;

namespace SimpleCrm.Controllers
{
    public class ProjectsController(IUnitOfWork _unitOfWork) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var projects = await _unitOfWork.Repository<Project>().GetAllAsync();
            var mappedProjects = projects.Select(z => new Project
            {
                Id = z.Id,
                Title = z.Title,
                ProjectManger = z.ProjectManger,
                Ended = z.Ended,
                StartedAt = z.StartedAt,
            }).ToList();
            return View(mappedProjects);
        }
 
        public async Task<IActionResult> Details(Guid Id)
        {
            var project = await _unitOfWork.Repository<Project>().GetBYIdAsync(Id);
            var mappedProject =  new Project
            {
                Id = project.Id,
                Title = project.Title,
                ProjectManger = project.ProjectManger,
                Description = project.Description,
                Objectives = project.Objectives,
                Ended = project.Ended,
                StartedAt = project.StartedAt,
            };
            return View(mappedProject);
        }
 
        [Authorize(Roles = "ProductionMangerA,ProductionMangerB,OperationManger")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjectDto model)
        {

            if (ModelState.IsValid)
            {
                var project = new Project
                {
                    Title = model.Title,
                    Description = model.Description,
                    Objectives = model.Objectives,
                    ProjectManger = model.ProjectManger,
                    Ended = false,
                    StartedAt = model.StartedAt,
                };
                try
                {
                    await _unitOfWork.Repository<Project>().Add(project);
                    await _unitOfWork.Complete();
                    return RedirectToAction("Index", "Projects");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(model);
        }
        public async Task<IActionResult> Edit(Guid Id)
        {
            var project  = await _unitOfWork.Repository<Project>().GetBYIdAsync(Id);
            if (project == null)
                return NotFound();
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid Id,Project model)
        {
            var project = await _unitOfWork.Repository<Project>().GetBYIdAsync(Id);
         
            if (ModelState.IsValid)
            {

                try
                {
                    project.Title = model.Title;
                    project.Description = model.Description;
                    project.Objectives = model.Objectives;
                    project.ProjectManger = model.ProjectManger;
                    _unitOfWork.Repository<Project>().Update(project);
                    await _unitOfWork.Complete();
                    return RedirectToAction("Index", "Projects");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(model);
        }
        [Authorize(Roles = "ProductionMangerA,ProductionMangerB,OperationManger")]
        [HttpPut]
        public async Task<ActionResult> MarkAsEnded(string Id)
        {
            var project = await _unitOfWork.Repository<Project>().GetBYIdAsync(Guid.Parse(Id));
            if (project == null)
            {
                return View();
            }
            try
            {
                project.Ended = true;
                _unitOfWork.Repository<Project>().Update(project);
                await _unitOfWork.Complete();
                return RedirectToAction("Index", "Projects");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return RedirectToAction("Index", "Projects");
            }
        }
    }
}
