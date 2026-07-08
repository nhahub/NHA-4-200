using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;

namespace ZoneSync.Web.ViewModels.Sensors
{
    public class ZoneConfigurationFormViewModel
    {
        [Required]
        [Display(Name = "Zone")]
        public int ZoneId { get; set; }

        [Required]
        [Display(Name = "Sensor Instance")]
        public int SensorInstanceId { get; set; }

        [Required]
        [Display(Name = "Configured By")]
        public int ConfiguredByUserId { get; set; }

        [Display(Name = "Configured At")]
        public DateTime ConfiguredAt { get; set; } = DateTime.Now;

        public SelectList? ZonesList { get; set; }
        public SelectList? SensorInstancesList { get; set; }
        public SelectList? UsersList { get; set; }
    }
}
