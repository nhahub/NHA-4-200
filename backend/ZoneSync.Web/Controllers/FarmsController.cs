using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZoneSync.Core.Entities.Identity;
using ZoneSync.Service.Contracts;
using ZoneSync.Web.ViewModels.FarmZone;

namespace ZoneSync.Web.Controllers
{
    [Authorize]
    public class FarmsController : Controller
    {
        private readonly IFarmZoneService farmZoneService;
        private readonly UserManager<ApplicationUser> userManager;

        public FarmsController(IFarmZoneService farmZoneService, UserManager<ApplicationUser> userManager)
        {
            this.farmZoneService = farmZoneService;
            this.userManager = userManager;
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

            var aspNetUserId = userManager.GetUserId(User);
            var ownerUserId = await farmZoneService.GetUserProfileIdAsync(aspNetUserId!);

            var farm = await farmZoneService.CreateFarmAsync(
                model.FarmName,
                model.FarmLocation,
                model.SoilType,
                ownerUserId);

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

            try
            {
                await farmZoneService.UpdateFarmAsync(
                    model.FarmId,
                    model.FarmName,
                    model.FarmLocation,
                    model.SoilType);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }

            return RedirectToAction(nameof(Details), new { id = model.FarmId });
        }

        public async Task<IActionResult> Details(int id)
        {
            var farm = await farmZoneService.GetFarmAsync(id);

            if (farm is null)
            {
                return NotFound();
            }

            var zones = await farmZoneService.GetActiveZonesForFarmAsync(id);

            ViewBag.Zones = zones;

            return View(farm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                await farmZoneService.SoftDeleteFarmAsync(id);
                TempData["SuccessMessage"] = "Farm deleted successfully";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index", "Home");
           
        }
    }
}