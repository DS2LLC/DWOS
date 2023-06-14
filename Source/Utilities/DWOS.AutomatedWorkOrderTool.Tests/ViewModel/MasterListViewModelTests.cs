using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using DWOS.AutomatedWorkOrderTool.Model;
using DWOS.AutomatedWorkOrderTool.Services;
using DWOS.AutomatedWorkOrderTool.Tests.Helpers;
using DWOS.AutomatedWorkOrderTool.ViewModel;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Reports;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.AutomatedWorkOrderTool.Tests.ViewModel
{
    [TestClass]
    public class MasterListViewModelTests
    {
        private const int CUSTOMER_ID = 1;
        private IMessenger _messenger;
        private MockFilePicker _filePicker;
        private MockSettingsProvider _settingsProvider;
        private MockDataManager _dataManager;
        private MockDocumentManager _documentManager;
        private MockUserManager _userManager;
        private MockPartManager _partManager;
        private MasterListViewModel _target;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            _messenger = new Messenger();
            _filePicker = new MockFilePicker();
            _settingsProvider = new MockSettingsProvider();
            _dataManager = new MockDataManager();
            _documentManager = new MockDocumentManager();
            _userManager = new MockUserManager();
            _partManager = new MockPartManager();
            _target = new MasterListViewModel(_messenger, _filePicker, _settingsProvider, _dataManager,
                _documentManager, _userManager, _partManager);
        }

        [TestMethod]
        public void ConfirmValidTest()
        {
            var masterListFile = GetTestFile("Master List.xlsx");
            Assert.IsTrue(File.Exists(masterListFile));

            // Initial state
            Assert.AreEqual(MasterListViewModel.DialogStatus.Setup, _target.CurrentStatus);
            Assert.IsNull(_target.Customer);
            Assert.AreEqual(0M, _target.EachPrice);
            Assert.AreEqual(0M, _target.LotPrice);

            // After load
            var customer = CustomerViewModel.Test(CUSTOMER_ID, "Test Customer", new List<OspFormatViewModel>
            {
                OspFormatViewModel.Test(CUSTOMER_ID, "Test Manufacturer", 0)
            });

            _target.Load(customer);

            Assert.IsNotNull(_target.Customer);

            // Settings
            Assert.IsFalse(_target.Continue.CanExecute(null));
            _target.MasterListFileName = masterListFile;
            _target.EachPrice = 2M;
            _target.LotPrice = 5M;
            Assert.IsTrue(_target.Continue.CanExecute(null));

            // Confirmation
            var triggeredChangeEvent = new AutoResetEvent(false);
            _target.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(MasterListViewModel.IsLoading) && !_target.IsLoading)
                {
                    triggeredChangeEvent.Set();
                }
            };

            _target.Continue.Execute(null);

            triggeredChangeEvent.WaitOne(TimeSpan.FromSeconds(1)); // Wait for async code
            Assert.AreEqual(MasterListViewModel.DialogStatus.Confirmation, _target.CurrentStatus);

            Assert.IsFalse(_target.HasError);
            Assert.IsTrue(string.IsNullOrEmpty(_target.ErrorText));

            Assert.IsTrue(_target.Parts.Count == 1);

            // Check part data
            var expectedPart = new MasterListPart
            {
                Name = "PART NAME", // Import capitalizes part name
                Description = "Part Description",
                Program = "0L",
                ProductCode = "767",
                Identity = "BA",
                OspCode = "BA-P-AE2-CA-PE1-00000-I2",
                Preferred = "Company 1",
                Alt = "Company 2",
                MaterialDescription = "AL-UT BAR 7075-T73511 AMS-QQ-A-200/11",
                Mask = "PLUG",
                PartMark = "113T1003-3",
                IdentityCode = "118"
            };

            var actualPart = _target.Parts.FirstOrDefault();
            AssertHelpers.HaveSameData(expectedPart, actualPart);
        }

        [TestMethod]
        public void ConfirmFileNotFoundTest()
        {
            var masterListFile = GetTestFile("File Not Found.xlsx");
            Assert.IsFalse(File.Exists(masterListFile));

            // Load
            var customer = CustomerViewModel.Test(CUSTOMER_ID, "Test Customer", new List<OspFormatViewModel>
            {
                OspFormatViewModel.Test(CUSTOMER_ID, "Test Manufacturer", 0)
            });

            _target.Load(customer);

            // Settings
            Assert.IsFalse(_target.Continue.CanExecute(null));
            _target.MasterListFileName = masterListFile;
            _target.EachPrice = 2M;
            _target.LotPrice = 5M;
            Assert.IsTrue(_target.Continue.CanExecute(null));

            // Confirmation
            _target.Continue.Execute(null);
            Assert.AreEqual(MasterListViewModel.DialogStatus.Confirmation, _target.CurrentStatus);

            Assert.IsTrue(_target.HasError);
            Assert.IsFalse(string.IsNullOrEmpty(_target.ErrorText));
        }

        [TestMethod]
        public void ConfirmEmptyFileTest()
        {
            var masterListFile = GetTestFile("Empty.xlsx");
            Assert.IsTrue(File.Exists(masterListFile));

            // Load
            var customer = CustomerViewModel.Test(CUSTOMER_ID, "Test Customer", new List<OspFormatViewModel>
            {
                OspFormatViewModel.Test(CUSTOMER_ID, "Test Manufacturer", 0)
            });

            _target.Load(customer);

            // Settings
            Assert.IsFalse(_target.Continue.CanExecute(null));
            _target.MasterListFileName = masterListFile;
            _target.EachPrice = 2M;
            _target.LotPrice = 5M;
            Assert.IsTrue(_target.Continue.CanExecute(null));

            // Confirmation
            _target.Continue.Execute(null);
            Assert.AreEqual(MasterListViewModel.DialogStatus.Confirmation, _target.CurrentStatus);

            Assert.IsTrue(_target.HasError);
            Assert.IsFalse(string.IsNullOrEmpty(_target.ErrorText));
        }

        [TestMethod]
        public void ConfirmInvalidFormatTest()
        {
            var masterListFile = GetTestFile("Master List - Invalid Format.xlsx");
            Assert.IsTrue(File.Exists(masterListFile));

            // Load
            var customer = CustomerViewModel.Test(CUSTOMER_ID, "Test Customer", new List<OspFormatViewModel>
            {
                OspFormatViewModel.Test(CUSTOMER_ID, "Test Manufacturer", 0)
            });

            _target.Load(customer);

            // Settings
            Assert.IsFalse(_target.Continue.CanExecute(null));
            _target.MasterListFileName = masterListFile;
            _target.EachPrice = 2M;
            _target.LotPrice = 5M;
            Assert.IsTrue(_target.Continue.CanExecute(null));

            // Confirmation
            _target.Continue.Execute(null);
            Assert.AreEqual(MasterListViewModel.DialogStatus.Confirmation, _target.CurrentStatus);

            Assert.IsTrue(_target.HasError);
            Assert.IsFalse(string.IsNullOrEmpty(_target.ErrorText));
        }

        [TestMethod]
        public void LoadDefaultPricesTest()
        {
            _dataManager.HasDefaultPrices = true;
            var customer = CustomerViewModel.Test(CUSTOMER_ID, "Test Customer", new List<OspFormatViewModel>
            {
                OspFormatViewModel.Test(CUSTOMER_ID, "Test Manufacturer", 0)
            });

            _target.Load(customer);

            // Assert values
            Assert.AreEqual(8M, _target.EachPrice);
            Assert.AreEqual(80M, _target.LotPrice);
        }

        private string GetTestFile(string name) =>
            Path.Combine("Test Files", name);

        private class MockFilePicker : IFileService
        {
            public string GetSpreadsheet() => null;

            public void Open(Reports.Report report)
            {
                // Do nothing
            }

            public void Print(Report report)
            {
                // Do nothing
            }
        }

        private class MockSettingsProvider : ISettingsProvider
        {
            public int PriceDecimalPlaces => 2;

            public PricingType PartPricingType => PricingType.Part;

            public bool DisplayProcessCocByDefault => false;

            public bool UseLeadTimeDayScheduling => false;

            public bool UseLeadTimeHourScheduling => false;

            public string WorkStatusChangingDepartment => "Changing Department";

            public string WorkStatusPendingOrderReview => "Pending Order Review";

            public bool OrderReviewEnabled => true;

            public string DepartmentSales => "Sales";
        }

        private class MockDataManager : IDataManager
        {
            public bool HasDefaultPrices { get; set; }

            public void LoadCustomerDefaultPrices(AwotDataSet.CustomerDefaultPriceDataTable dtCustomerDefaultPrice, int customerId)
            {
                if (HasDefaultPrices && customerId == CUSTOMER_ID)
                {
                    var eachPriceRow = dtCustomerDefaultPrice.NewCustomerDefaultPriceRow();
                    eachPriceRow.CustomerID = customerId;
                    eachPriceRow.DefaultPrice = 8M;
                    eachPriceRow.PriceUnit = OrderPrice.enumPriceUnit.Each.ToString();
                    dtCustomerDefaultPrice.AddCustomerDefaultPriceRow(eachPriceRow);
                    eachPriceRow.AcceptChanges();

                    var lotPriceRow = dtCustomerDefaultPrice.NewCustomerDefaultPriceRow();
                    lotPriceRow.CustomerID = customerId;
                    lotPriceRow.DefaultPrice = 80M;
                    lotPriceRow.PriceUnit = OrderPrice.enumPriceUnit.Lot.ToString();
                    dtCustomerDefaultPrice.AddCustomerDefaultPriceRow(lotPriceRow);
                    lotPriceRow.AcceptChanges();
                }
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

            public void LoadPartData(PartsDataset dsParts, int customerId)
            {
                // Do nothing for now
            }

            public void SavePartData(PartsDataset dataSet)
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

            public void LoadOrderData(OrdersDataSet dsOrders, int customerId)
            {
                // Unused
                throw new NotImplementedException();
            }

            public void SaveOrderData(OrdersDataSet dsOrders)
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

            public OrdersDataSet.OrderRow GetMatchingWorkOrder(string customerWorkOrder, int customerId)
            {
                return null;
            }

            public AwotDataSet.OSPFormatRow NewOspFormat(int matchingCustomerId, string mfgName)
            {
                throw new NotImplementedException();
            }
        }

        private class MockDocumentManager : IDocumentManager
        {
            public DocumentFolder GetFolder(string name)
            {
                return null;
            }

            public DocumentFolder GetFolder(DocumentFolder parentFolder, string name)
            {
                return null;
            }

            public DocumentFolder CreateFolder(string name)
            {
                return new DocumentFolder
                {
                    Name = name
                };
            }

            public DocumentFolder CreateFolder(DocumentFolder parentFolder, string name)
            {
                return new DocumentFolder
                {
                    Name = name,
                    Parent = parentFolder
                };
            }

            public DocumentFile GetFile(DocumentFolder folder, string name, string mediaType)
            {
                return null;
            }

            public DocumentFile CreateFile(DocumentFolder folder, string name, string mediaType)
            {
                return new DocumentFile
                {
                    Name = name,
                    Folder = folder,
                    MediaType = mediaType
                };
            }

            public void Revise(DocumentFile file, string sourceFile, DwosUser currentUser)
            {
                // Do nothing
            }

            public bool MatchesAnyRevision(DocumentFile file, string sourceFile) => false;

            public IEnumerable<DocumentFile> GetFiles(DocumentFolder folder)
            {
                throw new NotImplementedException();
            }
        }

        private class MockUserManager : IUserManager
        {
            public event EventHandler<UserChangedEventArgs> UserChanged
            {
                add { }
                remove { }
            }

            public DwosUser CurrentUser { get; } = new DwosUser();

            public bool LogIn(string pin)
            {
                throw new NotImplementedException();
            }

            public void LogOut()
            {
                throw new NotImplementedException();
            }

            public ImageSource GetImage(DwosUser user)
            {
                throw new NotImplementedException();
            }
        }

        private class MockPartManager : IPartManager
        {
            public PartProcessingInfo Decode(AwotDataSet dsAwot, string ospCode)
            {
                throw new NotImplementedException();
            }

            public Task ValidateAsync(int customerId, IEnumerable<MasterListPart> parts)
            {
                return Task.Factory.StartNew(() => {/* Do nothing */});
            }
        }
    }
}
