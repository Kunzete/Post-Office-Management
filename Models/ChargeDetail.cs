using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Post_Office_Management.Models
{
    public enum WeightRange
    {
        [Display(Name = "0-100g")]
        ZeroToOneHundredGrams,

        [Display(Name = "101-500g")]
        OneHundredOneToFiveHundredGrams,

        [Display(Name = "501-1000g")]
        FiveHundredOneToOneThousandGrams,

        [Display(Name = "1001-2000g")]
        OneThousandOneToTwoThousandGrams,

        [Display(Name = "2001g and above")]
        TwoThousandOneAndAboveGrams
    }

    public class ChargeDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ServiceTypeId { get; set; }

        [ForeignKey("ServiceTypeId")]
        public virtual ServiceType? ServiceType { get; set; }

        [Required]
        public WeightRange WeightRange { get; set; }

        [Range(0, 10000)] // adjust the range as needed
        public decimal Charge { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime EffectiveDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? EndDate { get; set; } // Optional

        [Required]
        public string CreatedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [Required]
        public string LastModifiedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime LastModifiedDate { get; set; }
    }
}