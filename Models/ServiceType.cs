namespace Post_Office_Management.Models
{
    public class ServiceType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal BaseCharge { get; set; }
        public bool IsVPP { get; set; } // added to distinguish VPP service
        public virtual ICollection<ChargeDetail>? ChargeDetails { get; set; }
        public virtual ICollection<Delivery>? Deliveries { get; set; }
    }
}
