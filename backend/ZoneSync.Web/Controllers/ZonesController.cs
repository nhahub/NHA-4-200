using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ZoneSync.Core.Entities.Identity;
using ZoneSync.Core.Entities.FarmZone;
using ZoneSync.Service.Contracts;
using ZoneSync.Web.ViewModels.FarmZone;

namespace ZoneSync.Web.Controllers
{
    [Authorize]
    public class ZonesController : Controller
    {
        private readonly IFarmZoneService farmZoneService;
        private readonly UserManager<ApplicationUser> userManager;

        public ZonesController(IFarmZoneService farmZoneService, UserManager<ApplicationUser> userManager)
        {
            this.farmZoneService = farmZoneService;
            this.userManager = userManager;
        }

        // Checks whether the currently logged-in user is a member of the given farm.
        // Used to stop a logged-in user from viewing or modifying a zone belonging
        // to a farm they don't belong to (IDOR protection).
        private async Task<bool> IsCurrentUserMemberOfFarmAsync(int farmId)
        {
            var aspNetUserId = userManager.GetUserId(User);
            if (aspNetUserId is null) return false;

            var currentUserId = await farmZoneService.GetUserProfileIdAsync(aspNetUserId);
            var members = await farmZoneService.GetFarmMembersAsync(farmId);

            return members.Any(m => m.UserId == currentUserId);
        }

        // Same check, but starting from a zoneId — resolves the zone's parent farm first.
        // Returns null if the zone itself doesn't exist, so callers can tell "not found"
        // apart from "found but not a member".
        private async Task<bool?> IsCurrentUserMemberOfZoneAsync(int zoneId)
        {
            var zone = await farmZoneService.GetZoneAsync(zoneId);
            if (zone is null) return null;

            return await IsCurrentUserMemberOfFarmAsync(zone.FarmId);
        }

        public async Task<IActionResult> Create(int farmId)
        {
            if (!await IsCurrentUserMemberOfFarmAsync(farmId))
            {
                return Forbid();
            }

            var model = new CreateZoneViewModel { FarmId = farmId };

            ViewBag.FarmMembers = await farmZoneService.GetFarmMembersAsync(farmId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateZoneViewModel model)
        {
            if (!await IsCurrentUserMemberOfFarmAsync(model.FarmId))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.FarmMembers = await farmZoneService.GetFarmMembersAsync(model.FarmId);
                return View(model);
            }

            var aspNetUserId = userManager.GetUserId(User);

            int createdByUserId;
            Zone zone;

            try
            {
                createdByUserId = await farmZoneService.GetUserProfileIdAsync(aspNetUserId!);

                zone = await farmZoneService.CreateZoneAsync(
                    model.FarmId,
                    model.ZoneName,
                    model.ZoneArea,
                    createdByUserId);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ViewBag.FarmMembers = await farmZoneService.GetFarmMembersAsync(model.FarmId);
                return View(model);
            }

            // Zone is created at this point — if assignment/supervisor steps fail,
            // redirect to the zone instead of losing it.
            try
            {
                foreach (var userId in model.AssignedUserIds)
                {
                    await farmZoneService.AssignUserToZoneAsync(zone.ZoneId, userId);
                }

                if (model.SupervisorUserId.HasValue)
                {
                    await farmZoneService.SetZoneSupervisorAsync(zone.ZoneId, model.SupervisorUserId.Value);
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = $"Zone created, but: {ex.Message}";
            }

            return RedirectToAction(nameof(Details), new { id = zone.ZoneId });
        }
        public async Task<IActionResult> Edit(int id)
        {
            var zone = await farmZoneService.GetZoneAsync(id);

            if (zone is null)
            {
                return NotFound();
            }

            if (!await IsCurrentUserMemberOfFarmAsync(zone.FarmId))
            {
                return Forbid();
            }

            var model = new EditZoneViewModel
            {
                ZoneId = zone.ZoneId,
                ZoneName = zone.ZoneName,
                ZoneArea = zone.ZoneArea,
                ZoneStatus = zone.ZoneStatus
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditZoneViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var isMember = await IsCurrentUserMemberOfZoneAsync(model.ZoneId);
            if (isMember is null)
            {
                return NotFound();
            }
            if (isMember == false)
            {
                return Forbid();
            }

            try
            {
                await farmZoneService.UpdateZoneAsync(
                    model.ZoneId,
                    model.ZoneName,
                    model.ZoneArea,
                    model.ZoneStatus);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }

            return RedirectToAction(nameof(Details), new { id = model.ZoneId });
        }
        public async Task<IActionResult> Details(int id)
        {
            var zone = await farmZoneService.GetZoneAsync(id);

            if (zone is null)
            {
                return NotFound();
            }

            if (!await IsCurrentUserMemberOfFarmAsync(zone.FarmId))
            {
                return Forbid();
            }

            var model = new ZoneDetailsViewModel
            {
                ZoneId = zone.ZoneId,
                ZoneName = zone.ZoneName,
                ZoneArea = zone.ZoneArea,
                ZoneStatus = zone.ZoneStatus,
                FarmName = zone.Farm?.FarmName ?? string.Empty,
                SupervisorName = zone.Supervisor is null
                    ? null
                    : $"{zone.Supervisor.UserFirstName} {zone.Supervisor.UserLastName}",
                AssignedUserNames = zone.ZoneUsers
                    .Select(zu => $"{zu.User.UserFirstName} {zu.User.UserLastName}")
                    .ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignUser(int zoneId, int userId)
        {
            var isMember = await IsCurrentUserMemberOfZoneAsync(zoneId);
            if (isMember is null)
            {
                return NotFound();
            }
            if (isMember == false)
            {
                return Forbid();
            }

            try
            {
                await farmZoneService.AssignUserToZoneAsync(zoneId, userId);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id = zoneId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetSupervisor(int zoneId, int supervisorUserId)
        {
            var isMember = await IsCurrentUserMemberOfZoneAsync(zoneId);
            if (isMember is null)
            {
                return NotFound();
            }
            if (isMember == false)
            {
                return Forbid();
            }

            try
            {
                await farmZoneService.SetZoneSupervisorAsync(zoneId, supervisorUserId);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id = zoneId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var zone = await farmZoneService.GetZoneAsync(id);

            if (zone is null)
            {
                return NotFound();
            }

            if (!await IsCurrentUserMemberOfFarmAsync(zone.FarmId))
            {
                return Forbid();
            }

            try
            {
                await farmZoneService.SoftDeleteZoneAsync(id);
                TempData["SuccessMessage"] = "Zone deleted successfully";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Details", "Farms", new { id = zone?.FarmId });
        }
    }
}