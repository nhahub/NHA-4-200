using Microsoft.AspNetCore.Mvc;
using ZoneSync.Service.Contracts;
using ZoneSync.Web.Models;

namespace ZoneSync.Web.Controllers
{
    public class TasksController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly IAlertService _alertService;
        private readonly IFarmZoneService _farmZoneService;

        public TasksController(ITaskService taskService, IAlertService alertService, IFarmZoneService farmZoneService)
        {
            _taskService = taskService;
            _alertService = alertService;
            _farmZoneService = farmZoneService;
        }

        // GET: /Tasks?zoneId=5
        public async Task<IActionResult> Index(int zoneId)
        {
            var zone = await _farmZoneService.GetZoneAsync(zoneId);
            if (zone == null) return NotFound();

            ViewBag.Zone = zone;
            return View(await _taskService.GetByZoneAsync(zoneId));
        }

        // GET: /Tasks/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var task = await _taskService.GetByIdWithDetailsAsync(id);
            if (task == null) return NotFound();

            if (task.Zone?.FarmId != null)
                ViewBag.FarmMembers = await _farmZoneService.GetFarmMembersAsync(task.Zone.FarmId);

            return View(task);
        }

        // GET: /Tasks/CreateFromAlert?alertId=5
        public async Task<IActionResult> CreateFromAlert(int alertId)
        {
            var alert = await _alertService.GetByIdAsync(alertId);
            if (alert == null) return NotFound();

            return View(new TaskFromAlertViewModel { AlertId = alertId, ZoneId = alert.ZoneId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromAlert(TaskFromAlertViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var task = await _taskService.CreateFromAlertAsync(
                vm.AlertId, vm.TaskName, vm.TaskDesc, vm.DueDate,
                vm.CreatedByUserId, vm.ActualVerificationAfterHours);

            return RedirectToAction(nameof(Details), new { id = task.TaskId });
        }

        // GET: /Tasks/CreateManual?zoneId=5
        public IActionResult CreateManual(int zoneId)
        {
            return View(new TaskManualViewModel { ZoneId = zoneId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateManual(TaskManualViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var task = await _taskService.CreateManualAsync(
                vm.ZoneId, vm.CropPlanId, vm.TaskName, vm.TaskDesc,
                vm.DueDate, vm.TaskPriority, vm.CreatedByUserId);

            return RedirectToAction(nameof(Details), new { id = task.TaskId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignUser(int taskId, int userId)
        {
            await _taskService.AssignUserToTaskAsync(taskId, userId);
            return RedirectToAction(nameof(Details), new { id = taskId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(int id)
        {
            await _taskService.StartTaskAsync(id);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddActionLog(int taskId, int executedByUserId,
            string? quantityType, string? quantityDesc, string? result, string? notes)
        {
            await _taskService.AddActionLogAsync(taskId, executedByUserId, quantityType, quantityDesc, result, notes);
            return RedirectToAction(nameof(Details), new { id = taskId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            await _taskService.CompleteTaskAsync(id);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Fail(int id)
        {
            await _taskService.FailTaskAsync(id);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Skip(int id)
        {
            await _taskService.SkipTaskAsync(id);
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}