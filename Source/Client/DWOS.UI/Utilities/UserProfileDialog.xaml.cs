using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.SecurityDataSetTableAdapters;
using DWOS.Utilities;
using NLog;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Interaction logic for UserProfileDialog.xaml
    /// </summary>
    public partial class UserProfileDialog : UserControl
    {
        #region Fields
        
        private Data.Datasets.SecurityDataSet _securityDataSet = null;
        private ObservableCollection<UserCustomerItem> Customers { get; set; }
        private Data.Datasets.SecurityDataSet.UsersRow User { get; set; }
        private UserInfo UserData { get; set; }

        #endregion

        #region Methods

        public UserProfileDialog()
        {
            InitializeComponent();
        }

        public void Load(int userId)
        {
            try
            {
                _securityDataSet = new Data.Datasets.SecurityDataSet() { EnforceConstraints = false };

                using (var ta = new Data.Datasets.SecurityDataSetTableAdapters.UsersTableAdapter())
                    ta.FillByUserID(_securityDataSet.Users, userId);
                using (var ta = new Data.Datasets.SecurityDataSetTableAdapters.CustomerSummaryTableAdapter())
                    ta.FillByActive(_securityDataSet.CustomerSummary);
                using (var ta = new Data.Datasets.SecurityDataSetTableAdapters.User_CustomersTableAdapter())
                    ta.FillBy(_securityDataSet.User_Customers, userId);

                User = _securityDataSet.Users.FindByUserID(userId);
                               

                if (User == null)
                    return;

                UserData = new UserInfo() { UserId = User.UserID, Department = User.IsDepartmentNull() ? null : User.Department, Name = User.Name, Title = User.IsTitleNull() ? null : User.Title, Email = User.IsEmailAddressNull() ? null : User.EmailAddress };
                this.DataContext = UserData;

                //set user pin
                userPin.Password = User.IsLoginPinNull() ? "******" : User.LoginPin.ToString();
                userPin.IsEnabled = false;

                //load customers
                Customers = new ObservableCollection<UserCustomerItem>();

                foreach (var customerSummary in _securityDataSet.CustomerSummary)
                    Customers.Add(new UserCustomerItem(User, customerSummary));

                lstCustomers.ItemsSource = Customers;

                LoadUserImage(User);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error loading the user profile dialog.");
            }
        }

        private void LoadUserImage(Data.Datasets.SecurityDataSet.UsersRow user)
        {
            if (!user.IsMediaIDNull())
            {
                using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter())
                {
                    byte[] imageBytes = ta.GetMediaStream(user.MediaID);
                    
                    if (imageBytes != null && imageBytes.Length > 0)
                        this.picUserImage.Source = WPFUtilities.ToWpfImage(imageBytes);
                }
            }
        }

        public void SaveData()
        {
            try
            {
                var currentUserCustomers = this.User.GetUser_CustomersRows().ToList();

                foreach (var customerItem in Customers)
                {
                    var customerSummary = customerItem.CustomerSummary;

                    var userCustomer = currentUserCustomers.FirstOrDefault(uc => uc.IsValidState() && uc.CustomerId == customerSummary.CustomerID);

                    //if should be added
                    if (customerItem.IsChecked)
                    {
                        //if not added yet then add one
                        if (userCustomer == null)
                            _securityDataSet.User_Customers.AddUser_CustomersRow(this.User, customerSummary);
                    }
                    else
                    {
                        //if should be removed and one exists then delete it
                        if (userCustomer != null)
                            userCustomer.Delete();
                    }
                }

                using (var ta = new Data.Datasets.SecurityDataSetTableAdapters.User_CustomersTableAdapter())
                    ta.Update(_securityDataSet.User_Customers);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error saving user customer data.");
            }
        }

        private void SetNewImage()
        {
            try
            {
                var user = this.User;
                var imgPath = MediaUtilities.SelectImageDialog();
                
                if (!String.IsNullOrWhiteSpace(imgPath) && File.Exists(imgPath))
                {
                    ClearCurrentImage();

                    //resize image
                    var imageBytes = MediaUtilities.CreateMediaStream(imgPath, 100);
                    this.picUserImage.Source = WPFUtilities.ToWpfImage(imageBytes);

                    if (this.picUserImage.Source is BitmapImage && user != null)
                    {
                        //add media to the database
                        using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter())
                        {
                            var mediaRow = ta.AddMedia(user.UserID.ToString(), "UserPicture.jpg", "jpg", imageBytes);
                            
                            if (mediaRow != null)
                                user.MediaID = mediaRow.MediaID;
                        }

                        //update users media id
                        using (var ta = new UsersTableAdapter())
                            ta.Update(user);
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error setting new image.");
            }
        }

        private void ClearCurrentImage()
        {
            try
            {
                if(this.picUserImage.Source is BitmapImage)
                    ((BitmapImage) this.picUserImage.Source).StreamSource.Dispose();

                this.picUserImage.Source = null;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error clearing the currently selected image.");
            }
        }

        private void DisposeMe()
        {
            if(Customers != null)
                Customers.ForEach(c => c.Dispose());

            Customers = null;
            this.User = null;

            if(_securityDataSet != null)
                _securityDataSet.Dispose();

            _securityDataSet = null;
        }

        #endregion

        #region Events

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var frm = new ResetPin { User = User })
                    frm.ShowDialog(DWOSApp.MainForm);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error  ");
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e) { DisposeMe(); }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetNewImage();
            e.Handled = true;
        }

        #endregion
        
        #region UserCustomerItem

        private class UserCustomerItem: INotifyPropertyChanged, IDisposable
        {
            private bool _isChecked = false;
            public event PropertyChangedEventHandler PropertyChanged;
            private Data.Datasets.SecurityDataSet.CustomerSummaryRow _customerSummary;

            public Data.Datasets.SecurityDataSet.CustomerSummaryRow CustomerSummary 
            {
                get { return _customerSummary; }
            }

            public bool IsChecked
            {
                get { return _isChecked; }
                set
                {
                    _isChecked = value;
                    OnPropertyChanged("IsChecked");
                }
            }

            public string Name
            {
                get
                {
                    return _customerSummary.Name;
                }
            }

            public UserCustomerItem( Data.Datasets.SecurityDataSet.UsersRow user, Data.Datasets.SecurityDataSet.CustomerSummaryRow customerSummary)
            {
                _customerSummary = customerSummary;
                IsChecked = user.GetUser_CustomersRows().Any(ucr => ucr.IsValidState() && ucr.CustomerId == customerSummary.CustomerID);
            }

            private void OnPropertyChanged(string name)
            {
                if(PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
            }

            public void Dispose()
            {
                _customerSummary = null;
                PropertyChanged = null;
            }
        }

        #endregion

        public class UserInfo
        {
            public int UserId { get; set; }
            public string Title { get; set; }
            public string Name { get; set; }
            public string Department { get; set; }
            public string Email { get; set; }
        }

    }
}
