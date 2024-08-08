namespace Post_Office_Management.Models
{
    public class ChargeDetail
    {
        public int Id { get; set; }
        public int ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; }
        public decimal Charge { get; set; }
        public DateTime EffectiveDate { get; set; }
        // Additional properties for tracking history of charges
    }
}
