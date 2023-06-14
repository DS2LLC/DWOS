using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using DWOS.AutomatedWorkOrderTool.Model;
using DWOS.AutomatedWorkOrderTool.Services;
using DWOS.AutomatedWorkOrderTool.Tests.Helpers;
using DWOS.AutomatedWorkOrderTool.ViewModel;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Order;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.AutomatedWorkOrderTool.Tests.ViewModel
{
    [TestClass]
    public class ShippingManifestViewModelTests
    {
        private const int CUSTOMER_ID = 1;
        private IMessenger _messenger;
        private MockFilePicker _filePicker;
        private MockCustomerManager _customerManager;
        private MockDocumentManager _documentManager;
        private MockDataManager _dataManager;
        private MockUserManager _userManager;
        private MockSettingsProvider _settingsProvider;
        private MockPriceUnitPersistence _priceUnitPersistence;
        private MockLeadTimePersistence _leadTimePersistence;
        private ShippingManifestViewModel _target;

        [TestInitialize]
        public void Initialize()
        {
            _messenger = new Messenger();
            _filePicker = new MockFilePicker();
            _customerManager = new MockCustomerManager();
            _documentManager = new MockDocumentManager();
            _dataManager = new MockDataManager();
            _userManager = new MockUserManager();
            _settingsProvider = new MockSettingsProvider();
            _priceUnitPersistence = new MockPriceUnitPersistence();
            _leadTimePersistence = new MockLeadTimePersistence();
            _target = new ShippingManifestViewModel(_messenger, _filePicker, _customerManager, _documentManager, _dataManager, _userManager, _settingsProvider, _priceUnitPersistence, _leadTimePersistence);
        }

        [TestMethod]
        public void ConfirmValidTest()
        {
            var shippingManifestFile = GetTestFile("Shipping Manifest.xlsx");
            Assert.IsTrue(File.Exists(shippingManifestFile));

            // Initial state
            Assert.AreEqual(ShippingManifestViewModel.DialogStatus.Setup, _target.CurrentStatus);
            Assert.IsNull(_target.Customer);
            Assert.IsTrue(_target.Fields.Count == 0);

            // After load
            _target.Load(_customerManager.CurrentCustomers.FirstOrDefault(c => c.Id == CUSTOMER_ID));
            Assert.IsNotNull(_target);
            Assert.IsTrue(_target.Fields.Count == 1);

            // Settings
            Assert.IsFalse(_target.Continue.CanExecute(null));
            _target.ShippingFileName = shippingManifestFile;
            Assert.IsTrue(_target.Continue.CanExecute(null));

            // Confirmation
            var triggeredChangeEvent = new AutoResetEvent(false);
            _target.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(ShippingManifestViewModel.IsLoading) && !_target.IsLoading)
                {
                    triggeredChangeEvent.Set();
                }
            };

            _target.Continue.Execute(null);

            triggeredChangeEvent.WaitOne(TimeSpan.FromSeconds(1)); // Wait for async code
            Assert.AreEqual(ShippingManifestViewModel.DialogStatus.Confirmation, _target.CurrentStatus);

            Assert.IsFalse(_target.HasError);
            Assert.IsTrue(string.IsNullOrEmpty(_target.ErrorText));

            Assert.IsTrue(_target.Orders.Count == 1, $"Actual order count: {_target.Orders.Count}");

            // Check order data
            var expectedOrder = new ShippingManifestOrder
            {
                Priority = "0",
                KacShipper = "123456",
                PurchaseOrder = "PO 123",
                PurchaseOrderItem = "100",
                WorkOrder = "WO 123",
                Part = "PART NAME",
                Project = "P00",
                DueDate = new DateTime(2017, 12, 14),
                Quantity = 5,
                LotCost = "80",
                InvoiceNumber = "1",
                VendorPackslip = "0001",
                PurchasingInvoiceApproval = "0002",
                VendorNumber = "0000-100",
                SourceCode = "Code 5"
            };

            var actualOrder = _target.Orders.FirstOrDefault();
            AssertHelpers.HaveSameData(expectedOrder, actualOrder);
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

            public void Print(Reports.Report report)
            {
                // Do nothing
            }
        }

        private class MockCustomerManager : ICustomerManager
        {
            public IEnumerable<CustomerViewModel> CurrentCustomers => new List<CustomerViewModel>
            {
                CustomerViewModel.Test(CUSTOMER_ID, "Test Customer", new List<OspFormatViewModel>
                {
                    OspFormatViewModel.Test(CUSTOMER_ID, "Test Manufacturer", 0)
                })
            };

            public void Add(CustomerViewModel newCustomer)
            {
                // Unused
                throw new NotImplementedException();
            }

            public IEnumerable<CustomField> GetCustomFields(CustomerViewModel customer)
            {
                if (customer?.Id == CUSTOMER_ID)
                {
                    return new List<CustomField>
                    {
                        new CustomField
                        {
                            Name = "Field 1",
                            IsVisible = true
                        },
                        new CustomField
                        {
                            Name = "Invisible",
                            IsVisible = false
                        }
                    };
                }

                return new List<CustomField>();
            }

            public bool UsesOrderReview(CustomerViewModel customer)
            {
                // Unused
                throw new NotImplementedException();
            }

            public string GetDefaultValue(CustomerViewModel customer, string field)
            {
                throw new NotImplementedException();
            }

            public void Load()
            {
                // Unused
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
        private class MockDataManager : IDataManager
        {
            public bool HasDefaultPrices { get; set; }

            public void LoadCustomerDefaultPrices(AwotDataSet.CustomerDefaultPriceDataTable dtCustomerDefaultPrice, int customerId)
            {
                // Should be unused
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

            public void LoadPartData(PartsDataset dsParts, int customerId)
            {
                if (customerId != CUSTOMER_ID)
                {
                    return;
                }

                var partRow = dsParts.Part.NewPartRow();
                partRow.Name = "PART NAME";
                partRow.CustomerID = CUSTOMER_ID;
                partRow.PartMarking = false;
                partRow.RequireCocByDefault = true;
                dsParts.Part.AddPartRow(partRow);
                dsParts.Part.AcceptChanges();
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
                // Should be unused for now
                throw new NotImplementedException();
            }

            public void SaveOrderData(OrdersDataSet dsOrders)
            {
                // Should be unused for now
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
                return 0;
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

        private class MockPriceUnitPersistence : IPriceUnitPersistence
        {
            public IEnumerable<OrderPrice.enumPriceUnit> ActivePriceUnits => new[]
            {
                OrderPrice.enumPriceUnit.Lot,
                OrderPrice.enumPriceUnit.Each
            };

            public PriceUnitData FindByPriceUnitId(int customerId, string priceUnitId)
            {
                throw new NotImplementedException();
            }

            public bool IsActive(string priceUnitId)
            {
                throw new NotImplementedException();
            }
        }

        private class MockLeadTimePersistence : ILeadTimePersistence
        {
            public TimeSpan ReceivingRolloverTime { get; }
            public double ShippingLeadTime { get; }
            public double CocLeadTime { get; }
            public double PartMarkingLeadTime { get; }
            public bool CocEnabled { get; }
            public bool PartMarkingEnabled { get; }
            public decimal GetLeadTimeDays(int processId)
            {
                throw new NotImplementedException();
            }
        }
    }
}
