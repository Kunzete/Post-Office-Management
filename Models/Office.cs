namespace Post_Office_Management.Models
{
    public class Office
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public ICollection<Delivery>? FromDeliveries { get; set; }
        public ICollection<Delivery>? ToDeliveries { get; set; }

    }
}
