using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZoneSync.Core.Data;
using ZoneSync.Core.Entities.GrowthStageModule;

namespace ZoneSync.Web.Controllers
{
    [Authorize]
    public class GrowthStagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GrowthStagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Create(int? cropId)
        {
            await FillCrops(cropId);
            return View(new GrowthStage { CropId = cropId ?? 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GrowthStage stage)
        {
            if (!ModelState.IsValid)
            {
                await FillCrops(stage.CropId);
                return View(stage);
            }

            _context.GrowthStages.Add(stage);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Crops", new { id = stage.CropId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var stage = await _context.GrowthStages.FindAsync(id);
            if (stage is null)
            {
                return NotFound();
            }

            await FillCrops(stage.CropId);
            return View(stage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GrowthStage stage)
        {
            if (id != stage.StageId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await FillCrops(stage.CropId);
                return View(stage);
            }

            _context.Update(stage);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Crops", new { id = stage.CropId });
        }

        private async Task FillCrops(int? selectedId = null)
        {
            ViewBag.Crops = new SelectList(await _context.Crops.OrderBy(c => c.CropName).ToListAsync(), "CropId", "CropName", selectedId);
        }
    }
}
