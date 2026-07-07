using ZoneSync.Core.Entities.FarmZone;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ZoneSync.Web.ViewModels;
using ZoneSync.Core.Data;

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
    }
}