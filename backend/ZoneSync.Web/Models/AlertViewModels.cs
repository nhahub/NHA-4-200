using System.ComponentModel.DataAnnotations;
using ZoneSync.Core.Enums;

namespace ZoneSync.Web.Models
{
    public class AlertCreateViewModel
    {
        [Required]
        public int ZoneId { get; set; }

        public int? CropPlanId { get; set; }

        [Required]
        public AlertSeverity AlertSeverity { get; set; }

        // ⚠️ مؤقت: اليوزر بيدخله يدوي دلوقتي لحد ما نربط بالـ Identity الحقيقي
        // (Member 1) ونجيب اليوزر المسجل دخوله تلقائيًا بدل الفورم ده.
        [Required]
        public int CreatedByUserId { get; set; }
    }
}