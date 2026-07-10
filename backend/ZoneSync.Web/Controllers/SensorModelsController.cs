using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZoneSync.Core.Data;
using ZoneSync.Core.Entities.Sensors;
using ZoneSync.Service.Contracts;
using ZoneSync.Web.ViewModels.Sensors;

namespace ZoneSync.Web.Controllers
{
    [Authorize]
    public class SensorModelsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISensorService _sensorService;

        public SensorModelsController(ApplicationDbContext context, ISensorService sensorService)
        {
            _context = context;
            _sensorService = sensorService;
        }

        public async Task<IActionResult> Index()
        {
            var sensorModels = await _context.SensorModels
                .Include(model => model.SensorModelMeasurementTypes)
                    .ThenInclude(link => link.MeasurementType)
                .OrderBy(model => model.ModelName)
                .ToListAsync();

            return View(sensorModels);
        }

        public async Task<IActionResult> Details(int id)
        {
            var sensorModel = await _context.SensorModels
                .Include(model => model.SensorModelMeasurementTypes)
                    .ThenInclude(link => link.MeasurementType)
                .Include(model => model.SensorInstances)
                .FirstOrDefaultAsync(model => model.Id == id);

            return sensorModel is null ? NotFound() : View(sensorModel);
        }

        public async Task<IActionResult> Create()
        {
            return View(await BuildFormViewModelAsync(new SensorModelFormViewModel()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SensorModelFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(await BuildFormViewModelAsync(model));
            }

            var sensorModel = new SensorModel
            {
                Type = model.Type,
                ModelName = model.ModelName,
                OutputType = model.OutputType
            };

            await _sensorService.CreateSensorModelAsync(sensorModel, model.SelectedMeasurementTypeIds);
            return RedirectToAction(nameof(Details), new { id = sensorModel.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var sensorModel = await _context.SensorModels
                .Include(model => model.SensorModelMeasurementTypes)
                .FirstOrDefaultAsync(model => model.Id == id);

            if (sensorModel is null)
            {
                return NotFound();
            }

            var model = new SensorModelFormViewModel
            {
                Id = sensorModel.Id,
                Type = sensorModel.Type,
                ModelName = sensorModel.ModelName,
                OutputType = sensorModel.OutputType,
                SelectedMeasurementTypeIds = sensorModel.SensorModelMeasurementTypes
                    .Select(link => link.MeasurementTypeId)
                    .ToList()
            };

            return View(await BuildFormViewModelAsync(model));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SensorModelFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(await BuildFormViewModelAsync(model));
            }

            var sensorModel = await _context.SensorModels.FindAsync(id);
            if (sensorModel is null)
            {
                return NotFound();
            }

            sensorModel.Type = model.Type;
            sensorModel.ModelName = model.ModelName;
            sensorModel.OutputType = model.OutputType;

            await _context.SaveChangesAsync();
            await _sensorService.UpdateSensorModelMeasurementTypesAsync(id, model.SelectedMeasurementTypeIds);
            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task<SensorModelFormViewModel> BuildFormViewModelAsync(SensorModelFormViewModel model)
        {
            var measurementTypes = await _context.MeasurementTypes
                .OrderBy(type => type.Name)
                .ToListAsync();

            model.MeasurementTypesList = new MultiSelectList(measurementTypes, "Id", "Name", model.SelectedMeasurementTypeIds);
            return model;
        }
    }
}
