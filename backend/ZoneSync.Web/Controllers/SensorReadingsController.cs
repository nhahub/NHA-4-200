using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZoneSync.Core.Data;
using ZoneSync.Service.Contracts;
using ZoneSync.Web.ViewModels.Sensors;

namespace ZoneSync.Web.Controllers
{
    [Authorize]
    public class SensorReadingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISensorService _sensorService;

        public SensorReadingsController(ApplicationDbContext context, ISensorService sensorService)
        {
            _context = context;
            _sensorService = sensorService;
        }

        public async Task<IActionResult> Index(int? sensorInstanceId, int? zoneId)
        {
            var query = _context.SensorReadings
                .Include(reading => reading.SensorInstance)
                    .ThenInclude(instance => instance!.SensorModel)
                .Include(reading => reading.MeasurementType)
                .AsQueryable();

            if (sensorInstanceId.HasValue)
            {
                query = query.Where(reading => reading.SensorInstanceId == sensorInstanceId.Value);
            }

            if (zoneId.HasValue)
            {
                var sensorIdsInZone = _context.ZoneConfigurations
                    .Where(configuration => configuration.ZoneId == zoneId.Value)
                    .Select(configuration => configuration.SensorInstanceId);

                query = query.Where(reading => sensorIdsInZone.Contains(reading.SensorInstanceId));
            }

            ViewBag.SensorInstancesList = await BuildSensorInstancesSelectListAsync(sensorInstanceId);
            ViewBag.ZonesList = new SelectList(await _context.Zones.OrderBy(zone => zone.ZoneName).ToListAsync(), "ZoneId", "ZoneName", zoneId);

            var readings = await query
                .OrderByDescending(reading => reading.ReadingDate)
                .ThenByDescending(reading => reading.ReadingTime)
                .ToListAsync();

            return View(readings);
        }

        public async Task<IActionResult> Create()
        {
            return View(await BuildFormViewModelAsync(new SensorReadingFormViewModel()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SensorReadingFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(await BuildFormViewModelAsync(model));
            }

            await _sensorService.AddReadingAsync(
                model.SensorInstanceId,
                model.MeasurementTypeId,
                model.ReadingValue,
                model.ReadingDate,
                model.ReadingTime);

            return RedirectToAction(nameof(Index), new { sensorInstanceId = model.SensorInstanceId });
        }

        private async Task<SensorReadingFormViewModel> BuildFormViewModelAsync(SensorReadingFormViewModel model)
        {
            model.SensorInstancesList = await BuildSensorInstancesSelectListAsync(model.SensorInstanceId);
            model.MeasurementTypesList = new SelectList(
                await _context.MeasurementTypes.OrderBy(type => type.Name).ToListAsync(),
                "Id",
                "Name",
                model.MeasurementTypeId);

            return model;
        }

        private async Task<SelectList> BuildSensorInstancesSelectListAsync(int? selectedId)
        {
            var sensorInstances = await _context.SensorInstances
                .Include(instance => instance.SensorModel)
                .OrderBy(instance => instance.SerialNumber)
                .ToListAsync();

            return new SelectList(sensorInstances.Select(instance => new
            {
                instance.Id,
                Name = instance.SensorModel == null
                    ? instance.SerialNumber
                    : $"{instance.SerialNumber} - {instance.SensorModel.ModelName}"
            }), "Id", "Name", selectedId);
        }
    }
}
