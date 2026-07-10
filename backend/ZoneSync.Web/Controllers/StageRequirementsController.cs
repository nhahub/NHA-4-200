using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZoneSync.Core.Data;
using ZoneSync.Core.Entities.StageRequirementModule;

namespace ZoneSync.Web.Controllers
{
    [Authorize]
    public class StageRequirementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StageRequirementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Create(int? stageId)
        {
            await FillStages(stageId);
            return View(new StageRequirement { StageId = stageId ?? 0, IsChosenByUser = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StageRequirement requirement)
        {
            if (!ModelState.IsValid)
            {
                await FillStages(requirement.StageId);
                return View(requirement);
            }

            _context.StageRequirements.Add(requirement);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Crops", new { id = await GetCropId(requirement.StageId) });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var requirement = await _context.StageRequirements.FindAsync(id);
            if (requirement is null)
            {
                return NotFound();
            }

            await FillStages(requirement.StageId);
            return View(requirement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StageRequirement requirement)
        {
            if (id != requirement.ReqId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await FillStages(requirement.StageId);
                return View(requirement);
            }

            _context.Update(requirement);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Crops", new { id = await GetCropId(requirement.StageId) });
        }

        private async Task FillStages(int? selectedId = null)
        {
            var stages = await _context.GrowthStages
                .Include(s => s.Crop)
                .OrderBy(s => s.Crop != null ? s.Crop.CropName : string.Empty)
                .ThenBy(s => s.StageOrder)
                .ToListAsync();

            ViewBag.Stages = new SelectList(stages, "StageId", "StageName", selectedId);
        }

        private async Task<int> GetCropId(int stageId)
        {
            return await _context.GrowthStages
                .Where(s => s.StageId == stageId)
                .Select(s => s.CropId)
                .FirstOrDefaultAsync();
        }
    }
}
