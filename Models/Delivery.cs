namespace Post_Office_Management.Models
{
    public enum DeliveryStatus
    {
        Posted = 1,
        InTransit = 2,
        Delivered = 3,
        // Add other status values as needed
    }

    public class Delivery
    {
        public int Id { get; set; }
        public string DeliveryNumber { get; set; } = "";
        public DeliveryStatus Status { get; set; } = DeliveryStatus.Posted; // Default status
        public DateTime DateOfPosting { get; set; } = DateTime.Now; // Set the date of posting to the current date and time
        public DateTime? DateOfReceipt { get; set; }
        public DateTime? DateOfDelivery { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverContactNumber { get; set; }
        public decimal Weight { get; set; }
        public int ServiceTypeId { get; set; }
        public ServiceType? ServiceType { get; set; }
        public int FromOfficeId { get; set; }
        public Office? FromOffice { get; set; }
        public int ToOfficeId { get; set; }
        public Office? ToOffice { get; set; }
        public decimal Charge { get; set; }
    }
}