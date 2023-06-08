using System;
using System.Net.Mail;
using System.Windows.Forms;
using DWOS.UI.Support;

namespace DWOS.UI.Utilities
{
    public partial class CustomerTicket : Form
    {
        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerTicket"/> class.
        /// </summary>
        public CustomerTicket()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads the page.
        /// </summary>
        private void LoadPage()
        {
            var userEmail = string.Empty;
            
            if (SecurityManager.Current.CurrentUser != null)
                userEmail = SecurityManager.Current.CurrentUser.IsEmailAddressNull() ? "" : SecurityManager.Current.CurrentUser.EmailAddress;
            
            this.teFrom.Text = userEmail;
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        private bool SendTicket()
        {
            NLog.LogManager.GetCurrentClassLogger((typeof(CustomerTicket))).Info("Sending message.");

            try
            {
                if (string.IsNullOrEmpty(teFrom.Text) || !this.IsEmailValid(teFrom.Text))
                {
                    MessageBox.Show("A valid email address is required when submitting a ticket as we may need to contact you.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (string.IsNullOrEmpty(teSubject.Text) || string.IsNullOrEmpty(teBody.Text))
                {
                    MessageBox.Show("A ticket subject and message are required.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                var client = new EmailSupportClient();
                client.AddTicket(new DWOS.UI.Support.Ticket() { FromAddress = teFrom.Text, Message = teBody.Text, Subject = teSubject.Text, UserName = SecurityManager.Current == null ? null : SecurityManager.Current.UserName });

                return true;
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger(typeof(CustomerTicket)).Error(exc, "Error sending message.");
                return false;
            }
        }

        /// <summary>
        /// Determines whether [is email valid] [the specified emailaddress].
        /// </summary>
        /// <param name="emailaddress">The emailaddress.</param>
        /// <returns>
        ///   <c>true</c> if [is email valid] [the specified emailaddress]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsEmailValid(string emailaddress)
        {
            try
            {
                var m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        #endregion Methods

        #region Events

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the btnSubmit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (this.SendTicket())
                this.Close();
        }

        /// <summary>
        /// Handles the Load event of the CustomerTicket control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CustomerTicket_Load(object sender, EventArgs e)
        {
            LoadPage();
        }

        #endregion Events
    }
}
