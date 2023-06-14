namespace DWOS.Portal.Models
{
    /// <summary>
    /// Represents a contact.
    /// </summary>
    public class ContactInfo
    {
        public int ContactId { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string PhoneNumberExt { get; set; }

        public string Email { get; set; }

        public string FaxNumber { get; set; }

        public string Manufacturer { get; set; }

        public string InvoicePreference { get; set; }

        public bool ReceiveShippingNotifications { get; set; }

        public bool ReceiveHoldNotifications { get; set; }

        public  bool ReceiveApprovalNotifications { get; set; }

        public  bool ReceiveLateOrderNotifications { get; set; }

    }
}