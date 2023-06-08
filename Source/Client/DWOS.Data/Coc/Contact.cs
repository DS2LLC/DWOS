namespace DWOS.Data.Coc
{
    public class Contact
    {
        #region Properties

        public int ContactId { get; }

        public string EmailAddress { get; }

        public bool CocNotification { get; }

        public bool Active { get; }

        #endregion

        #region Methods

        public Contact(int contactId, string emailAddress, bool cocNotification, bool active)
        {
            ContactId = contactId;
            EmailAddress = emailAddress;
            CocNotification = cocNotification;
            Active = active;
        }

        #endregion
    }
}
