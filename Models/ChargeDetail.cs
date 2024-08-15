using System.ComponentModel.DataAnnotations.Schema;

namespace Post_Office_Management.Models
{
    public class ChargeDetail
    {
        public int Id { get; set; }
        public int ServiceTypeId { get; set; }
        public virtual ServiceType? ServiceType { get; set; }

        public decimal Charge { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; } // Optional
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}