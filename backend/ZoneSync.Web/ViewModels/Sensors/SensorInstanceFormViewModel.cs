using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using ZoneSync.Core.Enums;

namespace ZoneSync.Web.ViewModels.Sensors
{
    public class SensorInstanceFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Sensor Model")]
        public int SensorModelId { get; set; }

        [Required]
        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; } = string.Empty;

        [Display(Name = "Status")]
        public SensorStatus Status { get; set; } = SensorStatus.Available;

        public SelectList? SensorModelsList { get; set; }
    }
}
