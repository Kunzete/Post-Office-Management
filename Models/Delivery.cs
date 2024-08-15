namespace Post_Office_Management.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public string DeliveryNumber { get; set; }
        public string Status { get; set; }
        public DateTime DateOfPosting { get; set; }
        public DateTime? DateOfReceipt { get; set; }
        public DateTime? DateOfDelivery { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverContactNumber { get; set; }
        public decimal Weight { get; set; }
        public int ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; }
        public int FromOfficeId { get; set; }
        public Office FromOffice { get; set; }
        public int ToOfficeId { get; set; }
        public Office ToOffice { get; set; }
    }
}