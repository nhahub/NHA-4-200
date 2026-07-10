using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZoneSync.Core.Data;
using ZoneSync.Core.Entities.Sensors;

namespace ZoneSync.Web.Controllers
{
    [Authorize]
    public class MeasurementTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MeasurementTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var measurementTypes = await _context.MeasurementTypes
                .OrderBy(measurementType => measurementType.Name)
                .ToListAsync();

            return View(measurementTypes);
        }

        public async Task<IActionResult> Details(int id)
        {
            var measurementType = await _context.MeasurementTypes
                .FirstOrDefaultAsync(type => type.Id == id);

            return measurementType is null ? NotFound() : View(measurementType);
        }

        public IActionResult Create()
        {
            return View(new MeasurementType());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MeasurementType measurementType)
        {
            if (!ModelState.IsValid)
            {
                return View(measurementType);
            }

            _context.MeasurementTypes.Add(measurementType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = measurementType.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var measurementType = await _context.MeasurementTypes.FindAsync(id);
            return measurementType is null ? NotFound() : View(measurementType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MeasurementType measurementType)
        {
            if (id != measurementType.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(measurementType);
            }

            _context.Update(measurementType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = measurementType.Id });
        }
    }
}
