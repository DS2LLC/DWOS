using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Properties;
using DWOS.Portal.Filters;
using NLog;
using TimeZoneNames;

namespace DWOS.Portal.Models
{
    /// <summary>
    /// Primary data access class for the Portal back-end.
    /// </summary>
    public static class DataAccess
    {
        #region Fields

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        public static string ConnectionString => Settings.Default.DWOSDataConnectionString;

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously checks the user's email address and password.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="password"></param>
        /// <returns>
        /// <c>true</c> if the password is correct; otherwise, <c>false</c>
        /// </returns>
        public static async Task<bool> IsPasswordCorrect(string userEmail, string password)
        {
            bool isCorrect = false;
            var cmd = new SqlCommand("SELECT (CASE WHEN (SELECT Count(*) FROM d_Contact Where EmailAddress = @userEmail And Password = @password And PortalAuthorized = 1) > 0 THEN 1 ELSE 0 END) As Found");
            cmd.Parameters.AddWithValue("@userEmail", userEmail);
            cmd.Parameters.AddWithValue("@password", password);

            using (var da = await ExecuteReader(cmd))
            {
                if (da.Read())
                {
                    isCorrect = Convert.ToBoolean(da["Found"]);
                }
            }

            return isCorrect;
        }

        /// <summary>
        /// Asynchronously updates a user's password.
        /// </summary>
        /// <param name="userEmail">The user email.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>
        /// <c>true</c> if successful; otherwise, <c>false</c>.
        /// </returns>
        public static async Task<bool> UpdatePassword(string userEmail, string newPassword)
        {
            Logger.Info("Updating user password.");

            var cmd = new SqlCommand("UPDATE [d_Contact] SET [Password] = @password WHERE [EmailAddress] = @emailAddress");
            cmd.Parameters.AddWithValue("@password", newPassword);
            cmd.Parameters.AddWithValue("@emailAddress", userEmail);

            var rowsChanged = await ExecuteNonQuery(cmd);
            return rowsChanged > 0;
        }

        /// <summary>
        /// Asynchronously retrieves user information.
        /// </summary>
        /// <param name="identity"></param>
        public static async Task<User>GetUser(DwosAuthorizeAttribute.LoginIdentity identity)
        {
            if (identity == null)
            {
                return null;
            }

            // Get the contact info
            var cmd = new SqlCommand("SELECT ContactID, CustomerID, Name FROM [d_Contact] " +
                "WHERE EmailAddress = @userEmail " +
                "AND Password = @password " +
                "AND PortalAuthorized = 1");

            cmd.Parameters.AddWithValue("@userEmail", identity.Login.UserName);
            cmd.Parameters.AddWithValue("@password", identity.Login.Password);

            User user = null;

            using (var da = await ExecuteReader(cmd))
            {
                if (da.Read())
                {
                    var customerId = Convert.ToInt32(da["CustomerID"]);
                    var contactId = Convert.ToInt32(da["ContactID"]);

                    user = new User
                    {
                        ContactId = contactId,
                        CustomerId = customerId,
                        Name = da["Name"].ToString(),
                        Email = identity.Login.UserName,
                        CompanyName = await GetCompanyName(customerId),
                        SecondaryCustomerIds = await GetSecondaryCustomerIds(contactId)
                    };

                    user.LastLogin = await GetLastLogin(user);
                }
            }

            return user;
        }

        /// <summary>
        /// Asynchronously retrieves contact information.
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public static async Task<ContactInfo> GetContactInfo(int contactId)
        {
            // Get the contact info
            var cmd = new SqlCommand(
                @"SELECT CustomerID, EmailAddress, Name, PhoneNumber,
                         FaxNumber, ManufacturerID, InvoicePreference,
                         ShippingNotification, HoldNotification, ApprovalNotification,
                         LateOrderNotification
                  FROM [d_Contact] WHERE ContactId = @contactId");

            cmd.Parameters.AddWithValue("@contactId", contactId);

            ContactInfo user = null;

            using (var da = await ExecuteReader(cmd))
            {
                if (da.Read())
                {
                    var phoneNumber = da["PhoneNumber"].ToString();
                    string phoneNumberExt = null;

                    if (!string.IsNullOrEmpty(phoneNumber))
                    {
                        var phoneNumberParts = phoneNumber.Split(new[] { " Ext." }, StringSplitOptions.None);
                        phoneNumber = phoneNumberParts[0];

                        if (phoneNumberParts.Length > 1)
                        {
                            phoneNumberExt = phoneNumberParts[1];
                        }
                    }
                    user = new ContactInfo
                    {
                        ContactId = contactId,
                        Name = da["Name"].ToString(),
                        PhoneNumber = phoneNumber,
                        PhoneNumberExt = phoneNumberExt,
                        Email = da["EmailAddress"].ToString(),
                        FaxNumber = da["FaxNumber"].ToString(),
                        Manufacturer = da["ManufacturerID"].ToString(),
                        InvoicePreference = da["InvoicePreference"].ToString(),
                        ReceiveShippingNotifications = Convert.ToBoolean(da["ShippingNotification"]),
                        ReceiveHoldNotifications = Convert.ToBoolean(da["HoldNotification"]),
                        ReceiveApprovalNotifications = Convert.ToBoolean(da["ApprovalNotification"]),
                        ReceiveLateOrderNotifications = Convert.ToBoolean(da["LateOrderNotification"]),
                    };
                }
            }

            return user;
        }

        /// <summary>
        /// Asynchronously updates contact information.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task UpdateContactInfo(ContactInfo user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var originalUser = await GetContactInfo(user.ContactId);

            if (originalUser == null)
            {
                throw new ArgumentNullException(
                    nameof(user),
                    "Cannot add a new user through the Portal site.");
            }

            Logger.Info("Updating contact info.");

            var phoneNumber = user.PhoneNumber + " Ext." + user.PhoneNumberExt;

            var cmd = new SqlCommand(
                @"UPDATE [d_Contact]
                  SET [Name] = @name,
                      [PhoneNumber] = @phoneNumber,
                      [EmailAddress] = @emailAddress,
                      [FaxNumber] = @faxNumber,
                      [ManufacturerID] = @manufacturer,
                      [InvoicePreference] = @invoicePref,
                      [ShippingNotification] = @shippingNotification,
                      [HoldNotification] = @holdNotification,
                      [ApprovalNotification] = @approvalNotification,
                      [LateOrderNotification] = @lateOrderNotification
                  WHERE [ContactID] = @contactId");

            cmd.Parameters.AddWithValue("@name", user.Name ?? string.Empty);
            cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);
            cmd.Parameters.AddWithValue("@emailAddress", user.Email ?? string.Empty);
            cmd.Parameters.AddWithValue("@faxNumber", user.FaxNumber ?? string.Empty);
            cmd.Parameters.AddWithValue("@manufacturer", user.Manufacturer ?? string.Empty);
            cmd.Parameters.AddWithValue("@invoicePref", user.InvoicePreference ?? string.Empty);
            cmd.Parameters.AddWithValue("@shippingNotification", user.ReceiveShippingNotifications);
            cmd.Parameters.AddWithValue("@holdNotification", user.ReceiveHoldNotifications);
            cmd.Parameters.AddWithValue("@approvalNotification", user.ReceiveApprovalNotifications);
            cmd.Parameters.AddWithValue("@lateOrderNotification", user.ReceiveLateOrderNotifications);

            cmd.Parameters.AddWithValue("@contactId", user.ContactId);

            await ExecuteNonQuery(cmd);

            if (user.ReceiveShippingNotifications != originalUser.ReceiveShippingNotifications)
            {
                // Update shipping notification preferences for additional customers
                var shippingCmd = new SqlCommand(
                    @"UPDATE ContactAdditionalCustomer
                      SET IncludeInShippingNotifications = @include
                      WHERE ContactID = @contactId;");

                shippingCmd.Parameters.AddWithValue("@include",
                    user.ReceiveShippingNotifications);

                shippingCmd.Parameters.AddWithValue("@contactId",
                    user.ContactId);

                await ExecuteNonQuery(shippingCmd);
            }

            if (user.ReceiveHoldNotifications != originalUser.ReceiveHoldNotifications)
            {
                // Update hold notification preferences for additional customers
                var holdCmd = new SqlCommand(
                    @"UPDATE ContactAdditionalCustomer
                      SET IncludeInHoldNotifications = @include
                      WHERE ContactID = @contactId;");

                holdCmd.Parameters.AddWithValue("@include",
                    user.ReceiveHoldNotifications);

                holdCmd.Parameters.AddWithValue("@contactId",
                    user.ContactId);

                await ExecuteNonQuery(holdCmd);
            }

            if (user.ReceiveApprovalNotifications != originalUser.ReceiveApprovalNotifications)
            {
                // Update approval notification preferences for additional customers
                var approvalCmd = new SqlCommand(
                    @"UPDATE ContactAdditionalCustomer
                      SET IncludeInApprovalNotifications = @include
                      WHERE ContactID = @contactId;");

                approvalCmd.Parameters.AddWithValue("@include",
                    user.ReceiveApprovalNotifications);

                approvalCmd.Parameters.AddWithValue("@contactId",
                    user.ContactId);

                await ExecuteNonQuery(approvalCmd);
            }

            if (user.ReceiveLateOrderNotifications != originalUser.ReceiveLateOrderNotifications)
            {
                // Update late order notification preferences for additional customers
                var approvalCmd = new SqlCommand(
                    @"UPDATE ContactAdditionalCustomer
                      SET IncludeInLateOrderNotifications = @include
                      WHERE ContactID = @contactId;");

                approvalCmd.Parameters.AddWithValue("@include",
                    user.ReceiveLateOrderNotifications);

                approvalCmd.Parameters.AddWithValue("@contactId",
                    user.ContactId);

                await ExecuteNonQuery(approvalCmd);
            }
        }

        public static async Task UpdateOrderApproval(OrderApproval approval, User user)
        {
            if (approval == null)
            {
                return;
            }

            // Update approval
            var cmd = new SqlCommand(@"
                UPDATE OrderApproval
                SET Status = @status,
                    ContactID = @contactId,
                    ContactNotes = @contactNotes,
                    ModifiedByContact = @modifiedDate
                WHERE OrderApprovalID = @orderApprovalId");

            cmd.Parameters.AddWithValue("@status", approval.Status);
            cmd.Parameters.AddWithValue("@contactId", user.ContactId);
            cmd.Parameters.AddWithValue("@contactNotes", approval.ContactNotes);
            cmd.Parameters.AddWithValue("@modifiedDate", DateTime.Now);
            cmd.Parameters.AddWithValue("@orderApprovalId", approval.OrderApprovalId);

            await ExecuteNonQuery(cmd);

            // Update Order History
            OrderHistoryDataSet.UpdateOrderHistory(
                approval.OrderId,
                "Portal",
                $"Contact {approval.Status.ToLower()} Approval {approval.OrderApprovalId}.",
                "N/A");
        }

        private static async Task<DateTime> GetLastLogin(User contact)
        {
            // Login occurs before retrieving information; the last login
            // is the second most recent.
            var date = DateTime.Now;
            var cmd = new SqlCommand("SELECT TOP 2 [LastLogin] FROM [PortalLoginHistory] WHERE [UserID] = @contactId ORDER BY LastLogin DESC");
            cmd.Parameters.AddWithValue("@contactId", contact.ContactId);

            using (var da = await ExecuteReader(cmd))
            {
                if (da.Read() && da.Read())
                {
                    date = TimeZoneInfo.ConvertTime(da.GetDateTime(0), TimeZoneInfo.Local);
                }
            }

            return date;
        }

        /// <summary>
        /// Gets the name of the company.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <returns>
        /// <c>true</c> if company name successfully retrieved and set; otherwise, <c>false</c>
        /// </returns>
        private static async Task<string> GetCompanyName(int customerId)
        {
            var cmd = new SqlCommand("SELECT Name FROM [Customer] WHERE CustomerID = @customerID");
            cmd.Parameters.AddWithValue("@customerID", customerId);

            using (var da = await ExecuteReader(cmd))
            {
                return da.Read() ? da["Name"].ToString() : null;
            }
        }

        private static async Task<List<int>> GetSecondaryCustomerIds(int contactId)
        {
            if (!NewApplicationSettings().AllowAdditionalCustomersForContacts)
            {
                return null;
            }

            var cmd = new SqlCommand(@"SELECT CustomerID
FROM ContactAdditionalCustomer
WHERE ContactID = @contactId AND IncludeInPortal = 1");

            cmd.Parameters.AddWithValue("@contactId", contactId);

            var customerIds = new HashSet<int>();

            using (var da = await ExecuteReader(cmd))
            {
                while (da.Read())
                {
                    customerIds.Add(da.GetInt32(0));
                }
            }

            return customerIds.ToList();
        }

        /// <summary>
        /// Asynchronously adds an entry to login history.
        /// </summary>
        /// <param name="user">The user.</param>
        public static async Task AddLoginHistory(User user)
        {
            if (user != null)
            {
                Logger.Info("Adding login history.");

                var cmd = new SqlCommand("INSERT INTO [PortalLoginHistory]([UserID],[LastLogin],[IPAddress]) VALUES (@contactId, @date, @ipAddress)");
                cmd.Parameters.AddWithValue("@contactId", user.ContactId);
                cmd.Parameters.AddWithValue("@date", DateTime.Now);
                cmd.Parameters.AddWithValue("@ipAddress", GetLocalIpAddress());

                // Add an entry to the portal login history table
                await ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Gets the host's IP Address.
        /// </summary>
        /// <returns>
        /// IP Address if found; otherwise, <c>"?"</c>
        /// </returns>
        public static string GetLocalIpAddress()
        {
            var localIp = "?";

            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily.ToString() == "InterNetwork")
                    {
                        localIp = ip.ToString();
                    }
                }
            }
            catch
            {
                localIp = "?";
            }

            return localIp;
        }

        /// <summary>
        /// Asynchronously retrieves order summary data for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<OrderSummary>> GetSummaries(User user)
        {
            var orderDataAccess = new OrderDataAccess(ConnectionString, NewApplicationSettings());
            return await orderDataAccess.GetSummaries(user);
        }

        /// <summary>
        /// Asynchronously determines if a COC with the given ID exists.
        /// </summary>
        /// <param name="cocId"></param>
        /// <returns>
        /// <c>true</c> if the COC exists; otherwise, <c>false</c>.
        /// </returns>
        public static async Task<bool> DoesCocExist(int cocId)
        {

            var cmd = new SqlCommand("SELECT (CASE WHEN (SELECT Count(*) FROM COC Where COCID = @cocId) > 0 THEN 1 ELSE 0 END) As Found");
            cmd.Parameters.AddWithValue("@cocId", cocId);

            using (var da = await ExecuteReader(cmd))
            {
                if (da.Read())
                {
                    return Convert.ToBoolean(da["Found"]);
                }
            }

            return false;
        }

        /// <summary>
        /// Asynchronously retrieves full order information for a given WO and user.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task<Order> GetOrder(int orderId, User user)
        {
            if (user == null)
            {
                return null;
            }

            var orderDataAccess = new OrderDataAccess(ConnectionString, NewApplicationSettings());
            return await orderDataAccess.GetOrder(orderId, user);
        }

        private static async Task<FileData> GetMedia(int mediaId)
        {
            var cmd = new SqlCommand($@"
                SELECT Media, FileName, FileExtension
                FROM Media
                WHERE MediaID = @mediaId");

            cmd.Parameters.AddWithValue("@mediaId", mediaId);
            using (var da = await ExecuteReader(cmd))
            {
                if (da.Read())
                {
                    var imgBytes = da["Media"] == DBNull.Value ? null : (byte[])da["Media"];
                    var fileName = da["FileName"].ToString();
                    var fileExtension = da["FileExtension"].ToString();

                    if (imgBytes != null && imgBytes.Length != 0)
                    {
                        return new FileData
                        {
                            Content = Convert.ToBase64String(imgBytes),
                            Name = fileName,
                            Type = GetFileType(fileExtension)
                        };
                    }
                }
            }

            return null;
        }

        private static string GetFileType(string fileExtension)
        {
            if (string.IsNullOrEmpty(fileExtension))
            {
                return "application/octet-stream";
            }

            return MimeMapping.GetMimeMapping($"file.{fileExtension}");
        }

        /// <summary>
        /// Asynchronously retrieves Portal-specific application settings.
        /// </summary>
        /// <returns></returns>
        public static async Task<PortalSettings> GetAppSettings()
        {
            var fieldDict = new Dictionary<string, bool>();
            var cmd = new SqlCommand("SELECT Name, IsRequired | IsVisible AS Include FROM Fields WHERE IsSystem = 1");
            using (var da = await ExecuteReader(cmd))
            {
                while (da.Read())
                {
                    fieldDict.Add(da.GetString(0), da.GetBoolean(1));
                }
            }

            var settings = NewApplicationSettings();

            return new PortalSettings
            {
                ShowManufacturer = !fieldDict.TryGetValue("Manufacturer", out bool mfg) || mfg,
                ShowRequiredDate = !fieldDict.TryGetValue("Required Date", out bool reqDate) || reqDate,
                ShowSerialNumbers = !fieldDict.TryGetValue("Serial Number", out bool serialNumber) || serialNumber,
                ShowTrackingNumber = !fieldDict.TryGetValue("Tracking Number", out bool trackingNumber) || trackingNumber,
                ShowOrderApprovals = settings.OrderApprovalEnabled,
                ShowLateOrderNotificationOption = settings.ShowLateOrderNotificationSetting,
                PriceDecimalPlaces = settings.PriceDecimalPlaces,
            };
        }

        /// <summary>
        /// Asynchronously retrieves information to use when showing the
        /// application's header.
        /// </summary>
        /// <returns></returns>
        public static HeaderData HeaderData()
        {
            var appSettings = NewApplicationSettings();
            var timeZone = TimeZoneInfo.Local;
            var now = DateTime.Now;

            var timeZoneNames = TZNames.GetAbbreviationsForTimeZone(timeZone.Id, "en-US");

            var headerData = new HeaderData
            {
                Logo = "data:image/png; base64," + appSettings.CompanyLogoImageEncoded64,
                CompanyName = appSettings.CompanyName,
                Tagline = appSettings.CompanyTagline,
                Timezone = timeZone.IsDaylightSavingTime(now)
                    ? timeZoneNames.Daylight
                    : timeZoneNames.Standard,
                TimezoneOffsetMinutes = -timeZone.GetUtcOffset(now).TotalMinutes
            };

            return headerData;
        }

        /// <summary>
        /// Creates a new <see cref="ApplicationSettings"/> instance.
        /// </summary>
        /// <remarks>
        /// Using this instead of <see cref="ApplicationSettings.Current"/>
        /// throughout the Portal site bypasses
        /// <see cref="ApplicationSettings"/> caching and ensures that all
        /// values are current.
        /// </remarks>
        /// <returns></returns>
        public static ApplicationSettings NewApplicationSettings()
        {
            var settings = ApplicationSettings.NewApplicationSettings();
            settings.UpdateDatabaseConnection(ConnectionString);
            return settings;
        }

        public static async Task<List<OrderApprovalSummary>> GetApprovalSummaries(User user)
        {
            var customerIdsString = $"({string.Join(",", user.AllCustomerIds)})";

            var cmd = new SqlCommand($@"
                SELECT OrderApproval.OrderApprovalID, 
                       OrderApproval.OrderID,
                       OrderApproval.Status
                FROM OrderApproval
                INNER JOIN [Order] ON OrderApproval.OrderID = [Order].OrderID
                WHERE [Order].CustomerID IN {customerIdsString}");

            var orderApprovals = new List<OrderApprovalSummary>();
            using (var da = await ExecuteReader(cmd))
            {
                while (da.Read())
                {
                    var approval = ReadOrderApprovalSummary(da);
                    orderApprovals.Add(approval);
                }
            }

            return orderApprovals;
        }

        private static OrderApprovalSummary ReadOrderApprovalSummary(SqlDataReader da) => new OrderApprovalSummary
        {
            OrderApprovalId = Convert.ToInt32(da["OrderApprovalID"]),
            OrderId = Convert.ToInt32(da["OrderID"]),
            Status = da["Status"].ToString()
        };

        public static async Task<OrderApproval> GetApproval(int orderApprovalId, User user)
        {
            if (user == null)
            {
                return null;
            }

            var customerIdsString = $"({string.Join(",", user.AllCustomerIds)})";

            var cmd = new SqlCommand($@"
                SELECT OrderApproval.OrderApprovalID, 
                       OrderApproval.OrderID,
                       Part.Name AS PartName,
                       OrderApproval.Status,
                       OrderApprovalTerm.Terms,
                       OrderApproval.Notes,
                       OrderApproval.DateCreated,
                       OrderApproval.ContactID,
                       OrderApproval.ContactNotes
                FROM OrderApproval
                INNER JOIN [Order] ON OrderApproval.OrderID = [Order].OrderID
                LEFT OUTER JOIN Part ON [Order].PartID = Part.PartID
                LEFT OUTER JOIN OrderApprovalTerm ON OrderApproval.OrderApprovalTermID = OrderApprovalTerm.OrderApprovalTermID
                WHERE OrderApproval.OrderApprovalID = @orderApprovalId AND [Order].CustomerID IN {customerIdsString}");

            cmd.Parameters.AddWithValue("@orderApprovalId", orderApprovalId);

            using (var da = await ExecuteReader(cmd))
            {
                if (da.Read())
                {
                    return await ReadOrderApproval(da);
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves a list of email addresses associated with an order's
        /// current address and product class.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>
        /// List of email addresses.
        /// </returns>
        public async static Task<List<string>> GetInternalEmailAddresses(int orderId)
        {
            var emailAddresses = new List<string>();

            // Retrieve email for current department
            var departmentCmd = new SqlCommand($@"
                SELECT EmailAddress
                FROM d_Department
                WHERE DepartmentID =
                (
                    SELECT TOP 1 CurrentLocation
                    FROM [Order]
                    WHERE OrderID = @orderId
                );");

            departmentCmd.Parameters.AddWithValue("@orderId", orderId);

            using (var deptReader = await ExecuteReader(departmentCmd))
            {
                if (deptReader.Read())
                {
                    var departmentEmail = deptReader[0].ToString();

                    if (!string.IsNullOrEmpty(departmentEmail))
                    {
                        emailAddresses.Add(departmentEmail);
                    }
                }
            }

            // Retrieve email for product classes
            if (await ShowProductClass())
            {
                var productClassCmd = new SqlCommand($@"
                    SELECT ProductClass.EmailAddress
                    FROM [Order]
                    INNER JOIN OrderProductClass ON [Order].OrderID = OrderProductClass.OrderID
                    INNER JOIN ProductClass ON OrderProductClass.ProductClass = ProductClass.Name
                    WHERE [Order].OrderID = @orderId;");

                productClassCmd.Parameters.AddWithValue("@orderId", orderId);

                using (var productClassReader = await ExecuteReader(productClassCmd))
                {
                    while (productClassReader.Read())
                    {
                        var productClassEmail = productClassReader[0].ToString();

                        if (!string.IsNullOrEmpty(productClassEmail))
                        {
                            emailAddresses.Add(productClassEmail);
                        }
                    }
                }
            }

            return emailAddresses;
        }

        private static async Task<bool> ShowProductClass()
        {
            var cmd = new SqlCommand("SELECT TOP 1 IsRequired | IsVisible FROM Fields WHERE Name = 'Product Class'");
            var showSerialNumbers = await ExecuteScalar(cmd) as bool? ?? true;
            return showSerialNumbers;
        }

        private async static Task<OrderApproval> ReadOrderApproval(SqlDataReader da)
        {
            var orderApprovalId = Convert.ToInt32(da["OrderApprovalID"]);

            return new OrderApproval
            {
                OrderApprovalId = orderApprovalId,
                OrderId = Convert.ToInt32(da["OrderID"]),
                Status = da["Status"].ToString(),
                Terms = da["Terms"].ToString(),
                Notes = da["Notes"].ToString(),
                DateCreated = da["DateCreated"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(da["DateCreated"]),
                ContactId = da["ContactID"] == DBNull.Value ? (int?)null : Convert.ToInt32(da["ContactID"]),
                ContactNotes = da["ContactNotes"].ToString(),
                PrimaryMedia = await GetPrimaryMedia(orderApprovalId),
                MediaUrls = await GetOrderApprovalMediaUrls(orderApprovalId)
            };
        }

        private async static Task<FileData> GetPrimaryMedia(int orderApprovalId)
        {
            var cmd = new SqlCommand($@"
                SELECT TOP 1 MediaID
                FROM OrderApprovalMedia
                WHERE OrderApprovalID = @orderApprovalId AND IsPrimary = 1;");

            cmd.Parameters.AddWithValue("@orderApprovalId", orderApprovalId);

            using (var da = await ExecuteReader(cmd))
            {
                if (da.Read())
                {
                    var mediaId = Convert.ToInt32(da["MediaID"]);

                    return await GetMedia(mediaId);
                }
            }

            return null;
        }

        private async static Task<List<string>> GetOrderApprovalMediaUrls(int orderApprovalId)
        {
            var mediaUrls = new List<string>();
            var cmd = new SqlCommand($@"
                SELECT OrderApprovalMediaId
                FROM OrderApprovalMedia
                WHERE OrderApprovalID = @orderApprovalId AND IsPrimary = 0;");

            cmd.Parameters.AddWithValue("@orderApprovalId", orderApprovalId);

            using (var da = await ExecuteReader(cmd))
            {
                while (da.Read())
                {
                    mediaUrls.Add($"/api/orderapprovalmedia/getmedia/{Convert.ToInt32(da["OrderApprovalMediaId"])}");
                }
            }

            return mediaUrls;
        }

        public static async Task<FileData> GetApprovalMedia(int orderApprovalMediaId, User user)
        {
            if (user == null)
            {
                return null;
            }

            var customerIdsString = $"({string.Join(",", user.AllCustomerIds)})";

            var cmd = new SqlCommand($@"
                SELECT OrderApprovalMedia.MediaID
                FROM OrderApprovalMedia
                INNER JOIN OrderApproval ON OrderApprovalMedia.OrderApprovalID = OrderApproval.OrderApprovalID
                INNER JOIN [Order] ON OrderApproval.OrderID = [Order].OrderID
                WHERE OrderApprovalMedia.OrderApprovalMediaID = @orderApprovalMediaID AND [Order].CustomerID IN {customerIdsString}");

            cmd.Parameters.AddWithValue("@orderApprovalMediaId", orderApprovalMediaId);

            var mediaId = 0;
            using (var da = await ExecuteReader(cmd))
            {
                if (da.Read())
                {
                    mediaId = Convert.ToInt32(da[0]);
                }
            }

            return await GetMedia(mediaId);
        }

        /// <summary>
        /// Executes the command and returns the first column of the first
        /// row of the result.
        /// </summary>
        /// <param name="cmd">The command to execute.</param>
        /// <returns>query result</returns>
        private static async Task<object> ExecuteScalar(SqlCommand cmd)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                cmd.Connection = conn;
                conn.Open();
                return await cmd.ExecuteScalarAsync();
            }
        }

        /// <summary>
        /// Executes the command and returns a <see cref="SqlDataReader"/>
        /// instance.
        /// </summary>
        /// <param name="cmd">The command to execute.</param>
        /// <returns>SQL Data Reader</returns>
        private static async Task<SqlDataReader> ExecuteReader(SqlCommand cmd)
        {
            var conn = new SqlConnection(ConnectionString);
            cmd.Connection = conn;
            conn.Open();
            return await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// Executes a stored procedure with a single parameter and returns a
        /// <see cref="SqlDataReader"/> instance.
        /// </summary>
        /// <param name="storedProcedure">The stored procedure.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <returns>Sql Data Reader</returns>
        private static async Task<SqlDataReader> ExecuteReader(string storedProcedure, string parameterName, object parameterValue)
        {
            var conn = new SqlConnection(ConnectionString);
            var cmd = new SqlCommand(storedProcedure, conn) { CommandType = CommandType.StoredProcedure };

            if (parameterName != null)
                cmd.Parameters.Add(new SqlParameter(parameterName, parameterValue));

            conn.Open();
            return await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// Executes the command as a non-query.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <returns>number of rows that were affected</returns>
        private static async Task<int> ExecuteNonQuery(SqlCommand cmd)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                cmd.Connection = conn;
                conn.Open();
                return await cmd.ExecuteNonQueryAsync();
            }
        }

        #endregion

    }
}