using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Windows.Threading;
using DWOS.AutomatedWorkOrderTool.Licensing;
using DWOS.AutomatedWorkOrderTool.Model;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.SecurityDataSetTableAdapters;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DWOS.Data;
using NLog;

namespace DWOS.AutomatedWorkOrderTool.Services
{
    internal class UserManager : IUserManager
    {
        #region Fields

        private readonly Guid _instanceGuid = Guid.NewGuid();
        private readonly DispatcherTimer _keepAliveTimer;

        #endregion

        #region Properties

        public IServerManager ServerManager { get; }

        #endregion

        #region Methods

        public UserManager(IServerManager serverManager)
        {
            ServerManager = serverManager ?? throw new ArgumentNullException(nameof(serverManager));

            _keepAliveTimer =
                new DispatcherTimer(TimeSpan.FromMinutes(5), DispatcherPriority.Normal, OnKeepAliveTick, Application.Current.Dispatcher)
                {
                    IsEnabled = false
                };
        }

        private DwosUser GetUser(int? userId)
        {
            if (!userId.HasValue)
            {
                return null;
            }

            var row = GetUserRow(userId.Value);
            return new DwosUser
            {
                Id = row.UserID,
                Name = row.Name,
                Department = row.IsDepartmentNull() ? null : row.Department,
                Title = row.IsTitleNull() ? null : row.Title,
                MediaId = row.IsMediaIDNull() ? (int?)null : row.MediaID,
                Roles = GetUserRoles(userId.Value)
            };
        }

        private static int? GetUserId(string pin)
        {
            using (var ta = new UsersTableAdapter())
            {
                return ta.GetUserIdByUserLoginPin(pin);
            }
        }

        private bool CheckOutLicense(DwosUser user)
        {
            if (user == null)
            {
                return false;
            }

            var client = NewClient();
            var response = client.CheckOutLicense(new CheckOutLicenseRequest(Environment.MachineName, user.Id, user.Name, _instanceGuid));
            client.Close();
            return response.CheckOutLicenseResult;
        }

        private LicenseServiceClient NewClient()
        {
            var serverInfo = ServerManager.ServerInfo;
            var client = new LicenseServiceClient(new NetTcpBinding(SecurityMode.None, false), new EndpointAddress(serverInfo.ServerAddressUri));
            return client;
        }

        private void CheckInLicense()
        {
            var client = NewClient();
            client.CheckInLicense(new CheckInLicenseRequest(_instanceGuid));
            client.Close();
        }

        private SecurityDataSet.UsersRow GetUserRow(int userId)
        {
            using (var dtUser = new SecurityDataSet.UsersDataTable())
            {
                using (var ta = new UsersTableAdapter())
                {
                    ta.FillByUserID(dtUser, userId);
                }

                return dtUser.FirstOrDefault();
            }
        }

        private static List<string> GetUserRoles(int userId)
        {
            using (var dtUserSecurityRoles = new SecurityDataSet.User_SecurityRolesDataTable())
            {
                using (var taURoles = new User_SecurityRolesTableAdapter())
                {
                    taURoles.FillAllByUser(dtUserSecurityRoles, userId);
                }

                return dtUserSecurityRoles.Select(s => s.SecurityRoleID).ToList();
            }
        }

        private void HandleKeepAliveFailure()
        {
            CurrentUser = null;
            UserChanged?.Invoke(this, new UserChangedEventArgs(UserChangedEventArgs.ChangeType.Unexpected));
        }

        #endregion

        #region Events

        private void OnKeepAliveTick(object sender, EventArgs e)
        {
            if (CurrentUser == null)
            {
                return;
            }

            try
            {
                var client = NewClient();
                var response = client.KeepLicenseAlive(new KeepLicenseAliveRequest(_instanceGuid));

                if (!response.KeepLicenseAliveResult)
                {
                    HandleKeepAliveFailure();
                }

                client.Close();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Error while contacting server w/ keep alive.");
                HandleKeepAliveFailure();
            }
        }

        #endregion

        #region IUserManager Members

        public event EventHandler<UserChangedEventArgs> UserChanged;

        public DwosUser CurrentUser { get; private set; }

        public bool LogIn(string pin)
        {
            if (CurrentUser != null)
            {
                LogOut();
            }

            var userId = GetUserId(pin);
            if (userId.HasValue)
            {
                var user = GetUser(userId);

                if (user != null && CheckOutLicense(user))
                {
                    _keepAliveTimer.Start();
                    CurrentUser = user;
                    UserChanged?.Invoke(this, new UserChangedEventArgs(UserChangedEventArgs.ChangeType.Expected));
                    return true;
                }
            }

            return false;
        }

        public void LogOut()
        {
            _keepAliveTimer.Stop();

            if (CurrentUser != null)
            {
                CurrentUser = null;
                UserChanged?.Invoke(this, new UserChangedEventArgs(UserChangedEventArgs.ChangeType.Expected));
                CheckInLicense();
            }
        }

        public ImageSource GetImage(DwosUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter())
            {
                var imageBytes = ta.GetMediaStream(user.MediaId ?? -1);

                using (var memoryStream = new MemoryStream(imageBytes))
                {
                    var img = new BitmapImage();
                    img.BeginInit();
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.StreamSource = memoryStream;
                    img.EndInit();
                    img.Freeze();
                    return img;
                }
            }
        }

        #endregion
    }
}
