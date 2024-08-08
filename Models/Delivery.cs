namespace Post_Office_Management.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public string DeliveryNumber { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverContactNumber { get; set; }
        public int Weight { get; set; }
        public int ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; }
        public int FromOfficeId { get; set; }
        public Office FromOffice { get; set; }
        public int ToOfficeId { get; set; }
        public Office ToOffice { get; set; }
    }
}
