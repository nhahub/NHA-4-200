using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // عشان الـ SelectList

namespace ZoneSync.Web.ViewModels
{
    public class CreateCropPlanViewModel
    {
        [Required]
        [Display(Name = "Select Zone")]
        public int ZoneId { get; set; }

        [Required]
        [Display(Name = "Select Crop")]
        public int CropId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime PlantingDate { get; set; }

        // Dropdown lists
        public SelectList ZonesList { get; set; }
        public SelectList CropsList { get; set; }
    }
}
