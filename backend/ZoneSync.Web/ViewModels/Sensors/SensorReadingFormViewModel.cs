using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;

namespace ZoneSync.Web.ViewModels.Sensors
{
    public class SensorReadingFormViewModel
    {
        [Required]
        [Display(Name = "Sensor Instance")]
        public int SensorInstanceId { get; set; }

        [Required]
        [Display(Name = "Measurement Type")]
        public int MeasurementTypeId { get; set; }

        [Required]
        [Display(Name = "Reading Value")]
        public decimal ReadingValue { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Reading Date")]
        public DateTime ReadingDate { get; set; } = DateTime.Today;

        [DataType(DataType.Time)]
        [Display(Name = "Reading Time")]
        public TimeSpan ReadingTime { get; set; } = DateTime.Now.TimeOfDay;

        public SelectList? SensorInstancesList { get; set; }
        public SelectList? MeasurementTypesList { get; set; }
    }
}
