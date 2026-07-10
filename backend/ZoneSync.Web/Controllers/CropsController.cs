using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZoneSync.Core.Data;
using ZoneSync.Core.Entities.CropModule;

namespace ZoneSync.Web.Controllers
{
    [Authorize]
    public class CropsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CropsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var crops = await _context.Crops
                .Include(c => c.GrowthStages)
                .OrderBy(c => c.CropName)
                .ToListAsync();

            return View(crops);
        }

        public async Task<IActionResult> Details(int id)
        {
            var crop = await _context.Crops
                .Include(c => c.GrowthStages.OrderBy(g => g.StageOrder))
                    .ThenInclude(g => g.StageRequirements)
                .FirstOrDefaultAsync(c => c.CropId == id);

            return crop is null ? NotFound() : View(crop);
        }

        public IActionResult Create()
        {
            return View(new Crop());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Crop crop)
        {
            if (!ModelState.IsValid)
            {
                return View(crop);
            }

            _context.Crops.Add(crop);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = crop.CropId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var crop = await _context.Crops.FindAsync(id);
            return crop is null ? NotFound() : View(crop);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Crop crop)
        {
            if (id != crop.CropId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(crop);
            }

            _context.Update(crop);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = crop.CropId });
        }
    }
}
