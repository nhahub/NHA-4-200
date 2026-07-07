using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ZoneSync.Core.Data;
using ZoneSync.Core.Entities.CropPlanModule;
using ZoneSync.Core.Entities.FarmZone;
using ZoneSync.Core.Entities.FarmZone; 
using ZoneSync.Web.ViewModels;

namespace ZoneSync.Web.Controllers
{
    public class CropPlansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CropPlansController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateCropPlanViewModel
            {
                ZonesList = new SelectList(await _context.Zones.ToListAsync(), "ZoneId", "ZoneName"),
                CropsList = new SelectList(await _context.Crops.ToListAsync(), "CropId", "CropName")
            };

            return View(viewModel);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCropPlanViewModel model)
        {
            if (ModelState.IsValid)
            {
               
                var cropPlan = new CropPlan
                {
                    CropId = model.CropId,
                    ZoneId = model.ZoneId,
                    PlantingDate = model.PlantingDate
                };

                _context.CropPlans.Add(cropPlan);

                
                var zone = await _context.Zones.FindAsync(model.ZoneId);
                if (zone != null)
                {
                    zone.ZoneStatus = ZoneSync.Core.Enums.ZoneStatus.Planted;
                }

                await _context.SaveChangesAsync();

               
                return Content("تم حفظ الخطة الزراعية بنجاح وتحديث حالة المنطقة!");
            }

            model.ZonesList = new SelectList(await _context.Zones.ToListAsync(), "ZoneId", "ZoneName");
            model.CropsList = new SelectList(await _context.Crops.ToListAsync(), "CropId", "CropName");

            return View(model);
        }
    }
}