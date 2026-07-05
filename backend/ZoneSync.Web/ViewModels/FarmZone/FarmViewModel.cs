using System.ComponentModel.DataAnnotations;

namespace ZoneSync.Web.ViewModels.FarmZone
{
    public class CreateFarmViewModel
    {
         [Required]
        [Display(Name = "Farm Name")]
        public string FarmName { get; set; } = null!;

        [Required]
        [Display(Name = "Farm Location")]
        public string FarmLocation { get; set; } = null!;

        [Required]
        [Display(Name = "Soil Type")]
        public string SoilType { get; set; } = null!;
    }

    public class EditFarmViewModel
    {
        public int FarmId { get; set; }

        [Required]
        [Display(Name = "Farm Name")]
        public string FarmName { get; set; } = null!;

        [Required]
        [Display(Name = "Farm Location")]
        public string FarmLocation { get; set; } = null!;

        [Required]
        [Display(Name = "Soil Type")]
        public string SoilType { get; set; } = null!;
    }
}