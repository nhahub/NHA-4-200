using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZoneSync.Service.Contracts;
using ZoneSync.Web.Models;

namespace ZoneSync.Web.Controllers
{
    [Authorize]
    public class AlertsController : Controller
    {
        private readonly IAlertService _alertService;
        private readonly IFarmZoneService _farmZoneService;

        public AlertsController(IAlertService alertService, IFarmZoneService farmZoneService)
        {
            _alertService = alertService;
            _farmZoneService = farmZoneService;
        }

        // GET: /Alerts?zoneId=5
        public async Task<IActionResult> Index(int zoneId)
        {
            var zone = await _farmZoneService.GetZoneAsync(zoneId);
            if (zone == null) return NotFound();

            ViewBag.Zone = zone;
            return View(await _alertService.GetByZoneAsync(zoneId));
        }

        // GET: /Alerts/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var alert = await _alertService.GetByIdAsync(id);
            if (alert == null) return NotFound();
            return View(alert);
        }

        // GET: /Alerts/Create?zoneId=5
        public async Task<IActionResult> Create(int zoneId)
        {
            var zone = await _farmZoneService.GetZoneAsync(zoneId);
            if (zone == null) return NotFound();
            return View(new AlertCreateViewModel { ZoneId = zoneId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AlertCreateViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var alert = await _alertService.CreateManualAsync(
                vm.ZoneId, vm.CropPlanId, vm.AlertSeverity, vm.CreatedByUserId);

            return RedirectToAction(nameof(Details), new { id = alert.AlertId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id, int confirmedByUserId)
        {
            try
            {
                await _alertService.ConfirmAsync(id, confirmedByUserId);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Discard(int id, int confirmedByUserId)
        {
            try
            {
                await _alertService.DiscardAsync(id, confirmedByUserId);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}