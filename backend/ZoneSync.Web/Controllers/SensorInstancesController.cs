using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZoneSync.Core.Data;
using ZoneSync.Core.Entities.Sensors;
using ZoneSync.Web.ViewModels.Sensors;

namespace ZoneSync.Web.Controllers
{
    public class SensorInstancesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SensorInstancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sensorInstances = await _context.SensorInstances
                .Include(instance => instance.SensorModel)
                .OrderBy(instance => instance.SerialNumber)
                .ToListAsync();

            return View(sensorInstances);
        }

        public async Task<IActionResult> Details(int id)
        {
            var sensorInstance = await _context.SensorInstances
                .Include(instance => instance.SensorModel)
                    .ThenInclude(model => model!.SensorModelMeasurementTypes)
                    .ThenInclude(link => link.MeasurementType)
                .Include(instance => instance.ZoneConfigurations)
                    .ThenInclude(configuration => configuration.Zone)
                .Include(instance => instance.SensorReadings.OrderByDescending(reading => reading.ReadingDate).ThenByDescending(reading => reading.ReadingTime))
                    .ThenInclude(reading => reading.MeasurementType)
                .FirstOrDefaultAsync(instance => instance.Id == id);

            return sensorInstance is null ? NotFound() : View(sensorInstance);
        }

        public async Task<IActionResult> Create()
        {
            return View(await BuildFormViewModelAsync(new SensorInstanceFormViewModel()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SensorInstanceFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(await BuildFormViewModelAsync(model));
            }

            var sensorInstance = new SensorInstance
            {
                SensorModelId = model.SensorModelId,
                SerialNumber = model.SerialNumber,
                Status = model.Status
            };

            _context.SensorInstances.Add(sensorInstance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = sensorInstance.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var sensorInstance = await _context.SensorInstances.FindAsync(id);
            if (sensorInstance is null)
            {
                return NotFound();
            }

            var model = new SensorInstanceFormViewModel
            {
                Id = sensorInstance.Id,
                SensorModelId = sensorInstance.SensorModelId,
                SerialNumber = sensorInstance.SerialNumber,
                Status = sensorInstance.Status
            };

            return View(await BuildFormViewModelAsync(model));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SensorInstanceFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(await BuildFormViewModelAsync(model));
            }

            var sensorInstance = await _context.SensorInstances.FindAsync(id);
            if (sensorInstance is null)
            {
                return NotFound();
            }

            sensorInstance.SensorModelId = model.SensorModelId;
            sensorInstance.SerialNumber = model.SerialNumber;
            sensorInstance.Status = model.Status;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task<SensorInstanceFormViewModel> BuildFormViewModelAsync(SensorInstanceFormViewModel model)
        {
            var sensorModels = await _context.SensorModels
                .OrderBy(sensorModel => sensorModel.ModelName)
                .ToListAsync();

            model.SensorModelsList = new SelectList(sensorModels, "Id", "ModelName", model.SensorModelId);
            return model;
        }
    }
}
