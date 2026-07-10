using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ZoneSync.Core.Data;
using ZoneSync.Core.Entities.CropPlanModule;
using ZoneSync.Core.Entities.FarmZone;
using ZoneSync.Service.Contracts;
using ZoneSync.Web.ViewModels;

namespace ZoneSync.Web.Controllers
{
    [Authorize]
    public class CropPlansController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICropPlanService _cropPlanService;

        public CropPlansController(ApplicationDbContext context, ICropPlanService cropPlanService)
        {
            _context = context;
            _cropPlanService = cropPlanService;
        }

        public async Task<IActionResult> Index()
        {
            var plans = await _context.CropPlans
                .Include(p => p.Crop)
                .Include(p => p.Zone)
                .OrderByDescending(p => p.PlantingDate)
                .ToListAsync();

            return View(plans);
        }

        public async Task<IActionResult> Details(int id)
        {
            var plan = await _context.CropPlans
                .Include(p => p.Crop)
                    .ThenInclude(c => c!.GrowthStages.OrderBy(s => s.StageOrder))
                    .ThenInclude(s => s.StageRequirements)
                .Include(p => p.Zone)
                .Include(p => p.StageInformations)
                .Include(p => p.CheckRequirements)
                    .ThenInclude(c => c.StageRequirement)
                .FirstOrDefaultAsync(p => p.CropPlanId == id);

            if (plan is null)
            {
                return NotFound();
            }

            return View(plan);
        }

        
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateCropPlanViewModel
            {
                ZonesList = new SelectList(await _context.Zones.ToListAsync(), "ZoneId", "ZoneName"),
                CropsList = new SelectList(await _context.Crops.ToListAsync(), "CropId", "CropName")
            };

            return View(viewModel);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCropPlanViewModel model)
        {
            if (ModelState.IsValid)
            {
                var cropPlan = await _cropPlanService.CreateCropPlanAsync(model.CropId, model.ZoneId, model.PlantingDate);

                
                var zone = await _context.Zones.FindAsync(model.ZoneId);
                if (zone != null)
                {
                    zone.ZoneStatus = ZoneSync.Core.Enums.ZoneStatus.Planted;
                }

                await _context.SaveChangesAsync();

               
                return RedirectToAction(nameof(Details), new { id = cropPlan.CropPlanId });
            }

            model.ZonesList = new SelectList(await _context.Zones.ToListAsync(), "ZoneId", "ZoneName");
            model.CropsList = new SelectList(await _context.Crops.ToListAsync(), "CropId", "CropName");

            return View(model);
        }
    }
}
