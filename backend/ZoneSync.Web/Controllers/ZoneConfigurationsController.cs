using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZoneSync.Core.Data;
using ZoneSync.Service.Contracts;
using ZoneSync.Web.ViewModels.Sensors;

namespace ZoneSync.Web.Controllers
{
    public class ZoneConfigurationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISensorService _sensorService;

        public ZoneConfigurationsController(ApplicationDbContext context, ISensorService sensorService)
        {
            _context = context;
            _sensorService = sensorService;
        }

        public async Task<IActionResult> Index()
        {
            var configurations = await _context.ZoneConfigurations
                .Include(configuration => configuration.Zone)
                .Include(configuration => configuration.SensorInstance)
                    .ThenInclude(instance => instance!.SensorModel)
                .Include(configuration => configuration.ConfiguredByUser)
                .OrderByDescending(configuration => configuration.ConfiguredAt)
                .ToListAsync();

            return View(configurations);
        }

        public async Task<IActionResult> Create()
        {
            return View(await BuildFormViewModelAsync(new ZoneConfigurationFormViewModel()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ZoneConfigurationFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(await BuildFormViewModelAsync(model));
            }

            var configuration = await _sensorService.ConfigureSensorInZoneAsync(
                model.ZoneId,
                model.SensorInstanceId,
                model.ConfiguredByUserId,
                model.ConfiguredAt);

            return RedirectToAction(nameof(Index), new { id = configuration.Id });
        }

        private async Task<ZoneConfigurationFormViewModel> BuildFormViewModelAsync(ZoneConfigurationFormViewModel model)
        {
            var zones = await _context.Zones.OrderBy(zone => zone.ZoneName).ToListAsync();
            var sensorInstances = await _context.SensorInstances
                .Include(instance => instance.SensorModel)
                .OrderBy(instance => instance.SerialNumber)
                .ToListAsync();
            var users = await _context.UserProfiles.OrderBy(user => user.UserFirstName).ToListAsync();

            model.ZonesList = new SelectList(zones, "ZoneId", "ZoneName", model.ZoneId);
            model.SensorInstancesList = new SelectList(sensorInstances.Select(instance => new
            {
                instance.Id,
                Name = instance.SensorModel == null
                    ? instance.SerialNumber
                    : $"{instance.SerialNumber} - {instance.SensorModel.ModelName}"
            }), "Id", "Name", model.SensorInstanceId);
            model.UsersList = new SelectList(users.Select(user => new
            {
                user.UserId,
                Name = $"{user.UserFirstName} {user.UserLastName}"
            }), "UserId", "Name", model.ConfiguredByUserId);

            return model;
        }
    }
}
