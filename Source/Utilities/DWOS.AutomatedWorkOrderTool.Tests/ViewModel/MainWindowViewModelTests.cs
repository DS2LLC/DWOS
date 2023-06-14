using System;
using System.Threading;
using DWOS.AutomatedWorkOrderTool.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DWOS.AutomatedWorkOrderTool.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using DWOS.AutomatedWorkOrderTool.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace DWOS.AutomatedWorkOrderTool.Tests.ViewModel
{
    [TestClass]
    public class MainWindowViewModelTests
    {
        private MockUserManager _userManagerMock;
        private MockCustomerManager _customerManagerMock;
        private MockDataManager _dataManagerMock;
        private UnsavedDataService _unsavedData;
        private MainWindowViewModel _target;

        [TestInitialize]
        public void Initialize()
        {
            _userManagerMock = new MockUserManager();
            _customerManagerMock = new MockCustomerManager();
            _dataManagerMock = new MockDataManager();
            _unsavedData = new UnsavedDataService();
            _target = new MainWindowViewModel(_userManagerMock, _customerManagerMock, _dataManagerMock, _unsavedData, new Messenger());
        }

        [TestMethod]
        public void TitleTest()
        {
            Assert.IsNotNull(_target.TitleText);
        }

        [TestMethod]
        public void UserLogInTest()
        {
            Assert.IsFalse(_target.LoggedIn);
            Assert.IsTrue(_target.ShowLoggedInPrompt);
            var triggeredChangeEvent = new AutoResetEvent(false);
            _target.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(MainWindowViewModel.LoggedIn))
                {
                    triggeredChangeEvent.Set();
                }
            };

            _userManagerMock.LogIn("58392");
            Assert.IsTrue(triggeredChangeEvent.WaitOne(TimeSpan.FromMilliseconds(500)));
            Assert.IsTrue(_target.LoggedIn);
            Assert.IsFalse(_target.ShowLoggedInPrompt);
        }

        [TestMethod]
        public void UserLogOutTest()
        {
            // Setup
            _userManagerMock.LogIn("58392");

            // Test
            Assert.IsTrue(_target.LoggedIn);
            Assert.IsFalse(_target.ShowLoggedInPrompt);
            var triggeredChangeEvent = new AutoResetEvent(false);
            _target.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(MainWindowViewModel.LoggedIn))
                {
                    triggeredChangeEvent.Set();
                }
            };

            // Test
            _userManagerMock.LogOut();
            Assert.IsTrue(triggeredChangeEvent.WaitOne(TimeSpan.FromMilliseconds(500)));
            Assert.IsFalse(_target.LoggedIn);
            Assert.IsTrue(_target.ShowLoggedInPrompt);
        }

        private class MockUserManager : IUserManager
        {
            public event EventHandler<UserChangedEventArgs> UserChanged;

            public DwosUser CurrentUser { get; private set; }

            public bool LogIn(string pin)
            {
                CurrentUser = new DwosUser
                {
                    Id = 1,
                    Name = "Test User"
                };
                UserChanged?.Invoke(this, new UserChangedEventArgs(UserChangedEventArgs.ChangeType.Expected));

                return true;
            }

            public void LogOut()
            {
                CurrentUser = null;
                UserChanged?.Invoke(this, new UserChangedEventArgs(UserChangedEventArgs.ChangeType.Expected));
            }

            public ImageSource GetImage(DwosUser user)
            {
                return null;
            }
        }

        private class MockCustomerManager : ICustomerManager
        {
            public IEnumerable<CustomerViewModel> CurrentCustomers => Enumerable.Empty<CustomerViewModel>();

            public void Load()
            {
                // Do nothing
            }

            public void Add(CustomerViewModel newCustomer)
            {
                // Do nothing
            }

            public IEnumerable<CustomField> GetCustomFields(CustomerViewModel customer)
            {
                // Do nothing
                return null;
            }

            public bool UsesOrderReview(CustomerViewModel customer)
            {
                throw new NotImplementedException();
            }

            public string GetDefaultValue(CustomerViewModel customer, string field)
            {
                throw new NotImplementedException();
            }
        }

        private class MockDataManager : IDataManager
        {
            public bool HasDefaultPrices { get; set; }

            public void LoadCustomerDefaultPrices(AwotDataSet.CustomerDefaultPriceDataTable dtCustomerDefaultPrice, int customerId)
            {
                throw new NotImplementedException();
            }

            public void LoadInitialData(AwotDataSet dataSet)
            {
                // Should be unused
                throw new NotImplementedException();
            }

            public void LoadInitialData(AwotDataSet dataSet, int customerId, int ospCodeId)
            {
                // Should be unused
                throw new NotImplementedException();
            }

            public void LoadOspFormatData(AwotDataSet dataSet, int customerId)
            {
                // Do nothing for now
            }

            public void LoadManufacturers(AwotDataSet.d_ManufacturerDataTable dtManufacturer)
            {
                // Should be unused
                throw new NotImplementedException();
            }

            public void LoadProcesses(AwotDataSet dataSet, string department)
            {
                // Should be unused
                throw new NotImplementedException();
            }

            public void SaveOspData(AwotDataSet dataSet)
            {
                // Should be unused for now
                throw new NotImplementedException();
            }

            public void LoadPartData(Data.Datasets.PartsDataset dsParts, int customerId)
            {
                // Do nothing for now
            }

            public void SavePartData(Data.Datasets.PartsDataset dataSet)
            {
                // Should be unused
                throw new NotImplementedException();
            }

            public int? GetLoadCapacityQuantity(int processId)
            {
                // Should be unused
                throw new NotImplementedException();
            }

            public decimal? GetLoadCapacityWeight(int processId)
            {
                // Should be unused
                throw new NotImplementedException();
            }

            public void LoadCustomFields(AwotDataSet.CustomFieldDataTable dtCustomField, int customerId)
            {
                // Should be unused
                throw new NotImplementedException();
            }

            public void LoadOrderData(Data.Datasets.OrdersDataSet dataSet, int customerId)
            {
                // Unused
                throw new NotImplementedException();
            }

            public void SaveOrderData(Data.Datasets.OrdersDataSet dataSet)
            {
                // Unused
                throw new NotImplementedException();
            }

            public IEnumerable<ProcessRequisite> GetProcessRequisites(int processId)
            {
                throw new NotImplementedException();
            }

            public PartMark FromPart(int partId)
            {
                throw new NotImplementedException();
            }

            public int GetOrderCount(int partId)
            {
                throw new NotImplementedException();
            }

            public Data.Datasets.OrdersDataSet.OrderRow GetMatchingWorkOrder(string customerWorkOrder, int customerId)
            {
                return null;
            }

            public AwotDataSet.OSPFormatRow NewOspFormat(int matchingCustomerId, string mfgName)
            {
                throw new NotImplementedException();
            }
        }
    }
}