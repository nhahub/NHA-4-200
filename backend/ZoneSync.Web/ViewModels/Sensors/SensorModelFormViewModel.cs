using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ZoneSync.Web.ViewModels.Sensors
{
    public class SensorModelFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Type")]
        public string Type { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Model Name")]
        public string ModelName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Output Type")]
        public string OutputType { get; set; } = string.Empty;

        [Display(Name = "Measurement Types")]
        public List<int> SelectedMeasurementTypeIds { get; set; } = new();

        public MultiSelectList? MeasurementTypesList { get; set; }
    }
}
