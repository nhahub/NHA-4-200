using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;

namespace ZoneSync.Web.ViewModels
{
    public class CreateCropPlanViewModel
    {
        [Required(ErrorMessage = "يجب اختيار المنطقة (Zone)")]
        [Display(Name = "Zone")]
        public int ZoneId { get; set; }

        [Required(ErrorMessage = "يجب اختيار المحصول (Crop)")]
        [Display(Name = "Crop Type")]
        public int CropId { get; set; }

        [Required(ErrorMessage = "تاريخ الزراعة مطلوب")]
        [DataType(DataType.Date)]
        [Display(Name = "Planting Date")]
        public DateTime PlantingDate { get; set; } = DateTime.Today;

        // دول اللي أنت عرفتهم في الـ Controller بتاعك
        public SelectList ZonesList { get; set; }
        public SelectList CropsList { get; set; }
    }
}