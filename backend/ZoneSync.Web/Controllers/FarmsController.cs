using Microsoft.AspNetCore.Mvc;
using ZoneSync.Service.Contracts;
using ZoneSync.Web.ViewModels.FarmZone;

namespace ZoneSync.Web.Controllers
{
    public class FarmsController : Controller
    {
        private readonly IFarmZoneService farmZoneService;

        public FarmsController(IFarmZoneService farmZoneService)
        {
            this.farmZoneService = farmZoneService;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateFarmViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var farm = await farmZoneService.CreateFarmAsync(
                model.FarmName,
                model.FarmLocation,
                model.SoilType,
                model.OwnerUserId);

            return RedirectToAction(nameof(Details), new { id = farm.FarmId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var farm = await farmZoneService.GetFarmAsync(id);

            if (farm is null)
            {
                return NotFound();
            }

            var model = new EditFarmViewModel
            {
                FarmId = farm.FarmId,
                FarmName = farm.FarmName,
                FarmLocation = farm.FarmLocation,
                SoilType = farm.SoilType
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditFarmViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await farmZoneService.UpdateFarmAsync(
                model.FarmId,
                model.FarmName,
                model.FarmLocation,
                model.SoilType);

            return RedirectToAction(nameof(Details), new { id = model.FarmId });
        }

        public async Task<IActionResult> Details(int id)
        {
            var farm = await farmZoneService.GetFarmAsync(id);

            if (farm is null)
            {
                return NotFound();
            }

            // Task doc requirement: Farm Details must only list non-deleted zones.
            var zones = await farmZoneService.GetActiveZonesForFarmAsync(id);

            ViewBag.Zones = zones;

            return View(farm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(int id)
        {
            await farmZoneService.SoftDeleteFarmAsync(id);

            TempData["SuccessMessage"] = "Farm deleted successfully";

            return RedirectToAction("Index", "Home");
        }
    }
}