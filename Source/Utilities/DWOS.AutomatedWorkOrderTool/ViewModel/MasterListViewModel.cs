using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DWOS.AutomatedWorkOrderTool.Messages;
using DWOS.AutomatedWorkOrderTool.Model;
using DWOS.AutomatedWorkOrderTool.Services;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Reports.Reports;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Infragistics.Documents.Excel;
using NLog;

namespace DWOS.AutomatedWorkOrderTool.ViewModel
{
    public class MasterListViewModel : ViewModelBase
    {
        public IPartManager PartManager { get; }

        #region Fields

        public event EventHandler PartsLoaded;
        public event EventHandler DialogExit;

        private readonly AwotDataSet _dsAwot = new AwotDataSet();
        private DialogStatus _currentStatus = DialogStatus.Setup;
        private CustomerViewModel _customer;
        private decimal _eachPrice;
        private decimal _lotPrice;
        private string _masterListFileName;
        private string _currencyFormat = "C2";
        private string _errorText;
        private bool _hasError;
        private bool _isLoading;
        private decimal _importProgress;

        #endregion

        #region Properties

        public IFileService FilePickerService { get; }

        public ISettingsProvider SettingsProvider { get; }

        public IDataManager DataManager { get; }

        public IUserManager UserManager { get; }

        public IDocumentManager DocumentManager { get; }

        public DialogStatus CurrentStatus
        {
            get => _currentStatus;
            set
            {
                if (Set(nameof(CurrentStatus), ref _currentStatus, value))
                {
                    RaisePropertyChanged(nameof(ShowSetup));
                    RaisePropertyChanged(nameof(ShowConfirmation));
                    RaisePropertyChanged(nameof(ShowImport));
                    RaisePropertyChanged(nameof(ShowResults));
                }
            }
        }

        public bool ShowSetup => _currentStatus == DialogStatus.Setup;

        public bool ShowConfirmation => _currentStatus == DialogStatus.Confirmation;

        public bool ShowImport => _currentStatus == DialogStatus.Import;

        public bool ShowResults => _currentStatus == DialogStatus.Results;

        public bool IsLoading
        {
            get => _isLoading;
            set => Set(nameof(IsLoading), ref _isLoading,  value);
        }

        public CustomerViewModel Customer
        {
            get => _customer;
            set => Set(nameof(Customer), ref _customer, value);
        }

        public decimal EachPrice
        {
            get => _eachPrice;
            set => Set(nameof(EachPrice), ref _eachPrice, value);
        }

        public decimal LotPrice
        {
            get => _lotPrice;
            set => Set(nameof(LotPrice), ref _lotPrice, value);
        }

        public string MasterListFileName
        {
            get => _masterListFileName;
            set => Set(nameof(MasterListFileName), ref _masterListFileName, value);
        }

        public string CurrencyFormat
        {
            get => _currencyFormat;
            set => Set(nameof(CurrencyFormat), ref _currencyFormat, value);
        }

        public bool HasError
        {
            get => _hasError;
            set => Set(nameof(HasError), ref _hasError, value);
        }

        public string ErrorText
        {
            get => _errorText;
            set => Set(nameof(ErrorText), ref _errorText, value);
        }

        public ObservableCollection<MasterListPart> Parts { get; } =
            new ObservableCollection<MasterListPart>();

        public ObservableCollection<ImportSummaryItem> ImportDetails { get; } =
            new ObservableCollection<ImportSummaryItem>();

        public ICommand SelectMasterList { get; }

        public ICommand Continue { get; }

        public ICommand CloseDialog { get; }

        public ICommand GoBack { get; }

        public decimal ImportProgress
        {
            get => _importProgress;
            set => Set(nameof(ImportProgress), ref _importProgress, value);
        }

        public ICommand ImportParts { get; }

        #endregion

        #region Methods

        public MasterListViewModel(IMessenger messenger, IFileService filePickerService,
            ISettingsProvider settingsProvider, IDataManager dataManager, IDocumentManager documentManager,
            IUserManager userManager, IPartManager partManager)
            : base(messenger)
        {
            FilePickerService = filePickerService ?? throw new ArgumentNullException(nameof(filePickerService));
            SettingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
            DataManager = dataManager ?? throw new ArgumentNullException(nameof(dataManager));
            DocumentManager = documentManager ?? throw new ArgumentNullException(nameof(documentManager));
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            PartManager = partManager ?? throw new ArgumentNullException(nameof(partManager));

            SelectMasterList = new RelayCommand(() =>
            {
                var newMasterList = FilePickerService.GetSpreadsheet();

                if (!string.IsNullOrEmpty(newMasterList))
                {
                    MasterListFileName = newMasterList;
                }
            });

            Continue = new RelayCommand(DoContinue, CanContinue);
            CloseDialog = new RelayCommand(() => DialogExit?.Invoke(this, EventArgs.Empty));
            GoBack = new RelayCommand(DoGoBack, CanGoBack);
            ImportParts = new RelayCommand(DoImportParts, () => Parts.Count > 0 && ImportProgress == 0);
            CurrencyFormat = $"C{SettingsProvider.PriceDecimalPlaces}";
        }

        public override void Cleanup()
        {
            base.Cleanup();
            _dsAwot.Dispose();
        }

        public void Load(CustomerViewModel customer)
        {
            CurrentStatus = DialogStatus.Setup;
            IsLoading = false;
            ImportDetails.Clear();
            Parts.Clear();
            MasterListFileName = string.Empty;

            Customer = customer;
            DataManager.LoadOspFormatData(_dsAwot, customer.Id);
            DataManager.LoadCustomerDefaultPrices(_dsAwot.CustomerDefaultPrice, customer.Id);

            var eachPriceRow = _dsAwot.CustomerDefaultPrice
                .FirstOrDefault(p => p.PriceUnit == OrderPrice.enumPriceUnit.Each.ToString());

            var lotPriceRow = _dsAwot.CustomerDefaultPrice
                .FirstOrDefault(p => p.PriceUnit == OrderPrice.enumPriceUnit.Lot.ToString());

            EachPrice = eachPriceRow?.DefaultPrice ?? 0M;
            LotPrice = lotPriceRow?.DefaultPrice ?? 0M;
        }

        private async void DoContinue()
        {
            try
            {
                if (!CanContinue())
                {
                    return;
                }

                if (_currentStatus == DialogStatus.Setup)
                {
                    IsLoading = true;
                    CurrentStatus = DialogStatus.Confirmation;
                    var importer = new MasterListImporter(_masterListFileName, _customer, DocumentManager);
                    var result = importer.ValidateFile();
                    HasError = !result.IsSuccessful;
                    ErrorText = result.IsSuccessful ? string.Empty : result.Message;

                    Parts.Clear();

                    if (result.IsSuccessful)
                    {
                        var worksheetParts = await importer.GetPartsFromWorksheet();
                        await PartManager.ValidateAsync(_customer.Id, worksheetParts);
                        foreach (var part in worksheetParts)
                        {
                            Parts.Add(part);
                        }

                        if (worksheetParts.All(p => p.Status == MasterListPart.PartStatus.Invalid))
                        {
                            // Cannot import a master list with no parts to import
                            HasError = true;
                            ErrorText = "No valid parts to import.";
                        }
                    }

                    // Trigger event for parts load so the UI can sort items
                    PartsLoaded?.Invoke(this, EventArgs.Empty);

                    // Set IsLoading to false; this must be the last thing
                    // that continue does because unit tests depend on its
                    // value being set.
                    IsLoading = false;
                }
                else if (_currentStatus == DialogStatus.Confirmation)
                {
                    ImportProgress = 0;
                    CurrentStatus = DialogStatus.Import;
                }
                else if (_currentStatus == DialogStatus.Import)
                {
                    CurrentStatus = DialogStatus.Results;
                }
            }
            catch (Exception exc)
            {
                MessengerInstance.Send(new ErrorMessage(exc, "Error pressing continue in wizard for master list."));
            }
        }

        private void DoGoBack()
        {
            try
            {
                if (_currentStatus == DialogStatus.Confirmation)
                {
                    CurrentStatus = DialogStatus.Setup;
                }
                else if (_currentStatus == DialogStatus.Import)
                {
                    CurrentStatus = DialogStatus.Confirmation;
                }
            }
            catch (Exception exc)
            {
                MessengerInstance.Send(new ErrorMessage(exc, "Error going back."));
            }
        }

        private void DoImportParts()
        {
            const int overheadEnd = 5;

            try
            {
                var partsToImport = Parts
                    .Where(CanImport)
                    .ToList();

                if (partsToImport.Count == 0)
                {
                    ImportDetails.Add(new ImportSummaryItem(ImportSummaryItem.ItemType.Info,
                        "There were no parts to import."));
                    return;
                }

                ImportProgress = 0;

                var progressStep = (100 - overheadEnd) / Convert.ToDecimal(Parts.Count(p => p.Status != MasterListPart.PartStatus.Invalid));
                var newPartCount = 0;
                var existingPartCount = 0;

                var masterListDocument = SaveMasterList();

                var reportItems = new List<ImportReportItem>();

                using (var dsParts = new PartsDataset())
                {
                    DataManager.LoadPartData(dsParts, _customer.Id);

                    foreach (var part in Parts)
                    {
                        ImportProgress += progressStep;

                        var processingInfo = PartManager.Decode(_dsAwot, part.OspCode);

                        if (part.Status == MasterListPart.PartStatus.Existing || part.Status == MasterListPart.PartStatus.ExistingWithWarning)
                        {
                            // Update existing part
                            var matchingPartRow = dsParts.Part.FirstOrDefault(p => p.Name == part.Name);

                            if (matchingPartRow != null)
                            {
                                // Update price
                                UpdatePartPrice(matchingPartRow);

                                var partMarkRow = matchingPartRow.GetPart_PartMarkingRows().FirstOrDefault();

                                if (partMarkRow == null && processingInfo.Marking != null)
                                {
                                    // Update part with new part marking
                                    partMarkRow = dsParts.Part_PartMarking.NewPart_PartMarkingRow();
                                    partMarkRow.PartRow = matchingPartRow;
                                    partMarkRow.ProcessSpec = processingInfo.Marking.ProcessSpec;
                                    partMarkRow.Def1 = processingInfo.Marking.Def1;
                                    partMarkRow.Def2 = processingInfo.Marking.Def2;
                                    partMarkRow.Def3 = processingInfo.Marking.Def3;
                                    partMarkRow.Def4 = processingInfo.Marking.Def4;

                                    dsParts.Part_PartMarking.AddPart_PartMarkingRow(partMarkRow);
                                }

                                // Link document if needed
                                if (matchingPartRow.GetPart_DocumentLinkRows().All(l => l.DocumentInfoID != masterListDocument.DocumentInfoId))
                                {
                                    var docLinkRow = dsParts.Part_DocumentLink.NewPart_DocumentLinkRow();
                                    docLinkRow.DocumentInfoID = masterListDocument.DocumentInfoId;
                                    docLinkRow.LinkToType = "Part";
                                    docLinkRow.LinkToKey = matchingPartRow.PartID;
                                    dsParts.Part_DocumentLink.AddPart_DocumentLinkRow(docLinkRow);
                                }

                                existingPartCount++;
                                reportItems.Add(new ImportReportItem
                                                {
                                                    Part = part,
                                                    Status = part.Status == MasterListPart.PartStatus.ExistingWithWarning
                                                        ? ImportStatus.UpdatedWithIncorrectOspCode
                                                        : ImportStatus.Updated
                                                });
                            }
                            else
                            {
                                // Part not found - should not happen because validation checks for this,
                                // but handle this case to be sure.
                                ImportDetails.Add(new ImportSummaryItem(ImportSummaryItem.ItemType.Error,
                                    $"Could not find existing part '{part.Name}' in DWOS."));

                                reportItems.Add(new ImportReportItem
                                                {
                                                    Part = part,
                                                    Status = ImportStatus.Error
                                                });
                            }
                        }
                        else if (part.Status == MasterListPart.PartStatus.New)
                        {
                            // Create new part
                            var partRow = dsParts.Part.NewPartRow();
                            partRow.Name = part.Name;
                            partRow.CustomerID = _customer.Id;
                            partRow.Description = part.Description;
                            partRow.Active = true;
                            partRow.EachPrice = _eachPrice;
                            partRow.LotPrice = _lotPrice;
                            partRow.PartMarking = processingInfo.Marking != null;

                            if (!string.IsNullOrEmpty(part.ProductCode))
                            {
                                partRow.Airframe = part.ProductCode;
                            }

                            partRow.AssemblyNumber = part.PartMark; // May not be in system
                            partRow.ManufacturerID = processingInfo.Manufacturer; // Always in system; OSP code setup requires it
                            dsParts.Part.AddPartRow(partRow);

                            // If needed, create new airframe
                            // Assumption: AWOT does not import parts that
                            // specify an incorrect airframe-manufacturer combination.
                            if (!partRow.IsAirframeNull())
                            {
                                var hasAirframe = dsParts.d_Airframe.Any(a =>
                                    a.AirframeID == partRow.Airframe);

                                if (!hasAirframe)
                                {
                                    var airframeRow = dsParts.d_Airframe.Newd_AirframeRow();
                                    airframeRow.ManufacturerID = partRow.ManufacturerID;
                                    airframeRow.AirframeID = partRow.Airframe;
                                    dsParts.d_Airframe.Addd_AirframeRow(airframeRow);

                                    ImportDetails.Add(new ImportSummaryItem(ImportSummaryItem.ItemType.Info, $"Added new airframe - '{partRow.Airframe}'."));
                                }
                            }

                            // Create processes
                            var stepOrder = 1;
                            foreach (var section in processingInfo.Processes)
                            {
                                var processRow = dsParts.PartProcess.NewPartProcessRow();
                                processRow.PartRow = partRow;
                                processRow.ProcessID = section.ProcessId;
                                processRow.ProcessAliasID = section.ProcessAliasId;
                                processRow.StepOrder = stepOrder;

                                if (section.LoadCapacityQuantity.HasValue)
                                {
                                    processRow.LoadCapacityQuantity = section.LoadCapacityQuantity.Value;
                                }

                                if (section.LoadCapacityWeight.HasValue)
                                {
                                    processRow.LoadCapacityWeight = section.LoadCapacityWeight.Value;
                                }

                                dsParts.PartProcess.AddPartProcessRow(processRow);
                                stepOrder++;
                            }

                            // Create part marking
                            if (processingInfo.Marking != null)
                            {
                                var partMarkRow = dsParts.Part_PartMarking.NewPart_PartMarkingRow();
                                partMarkRow.PartRow = partRow;
                                partMarkRow.ProcessSpec = processingInfo.Marking.ProcessSpec;
                                partMarkRow.Def1 = processingInfo.Marking.Def1;
                                partMarkRow.Def2 = processingInfo.Marking.Def2;
                                partMarkRow.Def3 = processingInfo.Marking.Def3;
                                partMarkRow.Def4 = processingInfo.Marking.Def4;

                                dsParts.Part_PartMarking.AddPart_PartMarkingRow(partMarkRow);
                            }

                            var partProcessRows = partRow.GetPartProcessRows();

                            // Split price among processes, if needed.
                            if (SettingsProvider.PartPricingType == PricingType.Process && partProcessRows.Length > 0)
                            {
                                var eachPerProcess = _eachPrice / partProcessRows.Length;
                                var lotPerProcess = _lotPrice / partProcessRows.Length;

                                // For now, just add each/lot price prices.
                                // Users can adjust the volume discount in Part Manager.
                                foreach (var processRow in partProcessRows)
                                {
                                    var eachPriceRow = dsParts.PartProcessVolumePrice.NewPartProcessVolumePriceRow();
                                    eachPriceRow.PriceUnit = OrderPrice.enumPriceUnit.Each.ToString();
                                    eachPriceRow.PartProcessRow = processRow;
                                    eachPriceRow.Amount = eachPerProcess;
                                    dsParts.PartProcessVolumePrice.AddPartProcessVolumePriceRow(eachPriceRow);

                                    var lotPriceRow = dsParts.PartProcessVolumePrice.NewPartProcessVolumePriceRow();
                                    lotPriceRow.PriceUnit =  OrderPrice.enumPriceUnit.Lot.ToString();
                                    lotPriceRow.PartProcessRow = processRow;
                                    lotPriceRow.Amount = lotPerProcess;
                                    dsParts.PartProcessVolumePrice.AddPartProcessVolumePriceRow(lotPriceRow);
                                }
                            }

                            // Link master list
                            var docLinkRow = dsParts.Part_DocumentLink.NewPart_DocumentLinkRow();
                            docLinkRow.DocumentInfoID = masterListDocument.DocumentInfoId;
                            docLinkRow.LinkToType = "Part";
                            docLinkRow.LinkToKey = partRow.PartID;
                            dsParts.Part_DocumentLink.AddPart_DocumentLinkRow(docLinkRow);

                            newPartCount++;
                            reportItems.Add(new ImportReportItem
                                            {
                                                Part = part,
                                                Status = ImportStatus.Added
                                            });
                        }
                        else
                        {
                            reportItems.Add(new ImportReportItem
                                            {
                                                Part = part,
                                                Status = ImportStatus.Error
                                            });
                        }
                    }

                    // Save
                    DataManager.SavePartData(dsParts);

                    // Create report
                    using (var report = new ExcelReport("Master List Import", PartReportContents(reportItems)))
                    {
                        FilePickerService.Open(report);
                    }
                }

                ImportDetails.Add(new ImportSummaryItem(ImportSummaryItem.ItemType.Info,
                    $"Added {newPartCount} part{(newPartCount != 1 ? "s" : "")}."));

                ImportDetails.Add(new ImportSummaryItem(ImportSummaryItem.ItemType.Info,
                    $"Updated {existingPartCount} part{(existingPartCount != 1 ? "s" : "")}."));

                UpdateDefaultPrices();

                DataManager.SaveOspData(_dsAwot);
            }
            catch (Exception exc)
            {
                MessengerInstance.Send(new ErrorMessage(exc, "Error importing master list."));
                ImportDetails.Add(new ImportSummaryItem(ImportSummaryItem.ItemType.Error,
                    "Failed to import master list."));

            }

            ImportProgress = 100;
        }

        private static bool CanImport(MasterListPart p) =>
            p != null && p.Status != MasterListPart.PartStatus.Invalid;

        private static ICollection<ExcelReport.ReportTable> PartReportContents(IEnumerable<ImportReportItem> reportItems)
        {
            if (reportItems == null)
            {
                return null;
            }

            var reportTable = new ExcelReport.ReportTable
            {
                Name = "Imported Parts",
                FormattingOptions = new ExcelReport.TableFormattingOptions
                {
                    BorderAroundCells = true
                },
                Columns = new List<ExcelReport.Column>
                {
                    new ExcelReport.Column("Part")
                    {
                        Width = 40
                    },
                    new ExcelReport.Column("Status")
                    {
                        Width = 20
                    },
                    new ExcelReport.Column("Info")
                    {
                        Width = 75
                    },
                    new ExcelReport.Column("OSP Code")
                    {
                        Width = 30
                    },
                },
                IncludeCompanyHeader = true,
                Rows = new List<ExcelReport.Row>()
            };

            var groupedItems = reportItems
                .GroupBy(s => s.Status)
                .ToList();

            // Include invalid items first
            var invalidItems = groupedItems.FirstOrDefault(group => group.Key == ImportStatus.Error)
                ?? Enumerable.Empty<ImportReportItem>();

            foreach (var item in invalidItems.OrderBy(i => i.Part.Name))
            {
                reportTable.Rows.Add(new ExcelReport.Row
                                     {
                                         Cells = new object[]
                                                 {
                                                     item.Part.Name,
                                                     "Invalid",
                                                     item.Part.ImportNotes,
                                                     item.Part.OspCode
                                                 },

                                         BackgroundColor = Color.Red,
                                         IsBold = true
                                     });
            }

            // Then, show existing parts with incorrect OSP Codes
            var updatedWithIssueItems = groupedItems.FirstOrDefault(group => group.Key == ImportStatus.UpdatedWithIncorrectOspCode)
                ?? Enumerable.Empty<ImportReportItem>();

            foreach (var item in updatedWithIssueItems.OrderBy(i => i.Part.Name))
            {
                reportTable.Rows.Add(new ExcelReport.Row
                                     {
                                         Cells = new object[]
                                                 {
                                                     item.Part.Name,
                                                     "Updated",
                                                     "OSP format does not match existing part's processes",
                                                     item.Part.OspCode
                                                 },

                                         BackgroundColor = Color.Goldenrod
                                     });
            }

            // Next, show new parts
            var newItems = groupedItems.FirstOrDefault(group => group.Key == ImportStatus.Added)
                ?? Enumerable.Empty<ImportReportItem>();

            foreach (var item in newItems.OrderBy(i => i.Part.Name))
            {
                reportTable.Rows.Add(new ExcelReport.Row
                                     {
                                         Cells = new object[]
                                                 {
                                                     item.Part.Name,
                                                     "New",
                                                     string.Empty,
                                                     item.Part.OspCode
                                                 }
                                     });
            }

            // Finally, show existing parts without error
            var updatedItems = groupedItems.FirstOrDefault(group => group.Key == ImportStatus.Updated)
                ?? Enumerable.Empty<ImportReportItem>();

            foreach (var item in updatedItems.OrderBy(i => i.Part.Name))
            {
                reportTable.Rows.Add(new ExcelReport.Row
                                     {
                                         Cells = new object[]
                                                 {
                                                     item.Part.Name,
                                                     "Updated",
                                                     string.Empty,
                                                     item.Part.OspCode
                                                 }
                                     });
            }

            return new List<ExcelReport.ReportTable>
            {
                reportTable
            };
        }

        private void UpdatePartPrice(PartsDataset.PartRow matchingPartRow)
        {
            matchingPartRow.EachPrice = _eachPrice;
            matchingPartRow.LotPrice = _lotPrice;

            var volumePriceRows = matchingPartRow
                .GetPartProcessRows()
                .SelectMany(p => p.GetPartProcessVolumePriceRows())
                .ToList();

            if (volumePriceRows.Count == 0)
            {
                return;
            }

            var pricePoints = PricePoint.GetPricePoints(volumePriceRows);
            foreach (var pricePoint in pricePoints)
            {
                var newTotal = OrderPrice.GetPricingStrategy(pricePoint.PriceUnit) == PricingStrategy.Each
                    ? _eachPrice
                    : _lotPrice;

                pricePoint.RecalculatePrices(newTotal);
            }
        }

        private bool CanContinue()
        {
            try
            {
                switch (_currentStatus)
                {
                    case DialogStatus.Setup:
                        return _eachPrice > 0M &&
                            _lotPrice > 0M &&
                            !string.IsNullOrEmpty(_masterListFileName);
                    case DialogStatus.Confirmation:
                        return !_hasError && Parts.Count(CanImport) > 0;
                    case DialogStatus.Import:
                        return _importProgress == 100;
                }
            }
            catch (Exception exc)
            {
                MessengerInstance.Send(new ErrorMessage(exc, "Error checking valid state of wizard for master list."));
            }

            return false;
        }

        private bool CanGoBack() => ImportProgress == 0;

        #region Master List Import

        private void UpdateDefaultPrices()
        {
            var eachPriceRow = _dsAwot.CustomerDefaultPrice
                .FirstOrDefault(p => p.CustomerID == _customer.Id && p.PriceUnit == OrderPrice.enumPriceUnit.Each.ToString());

            var lotPriceRow = _dsAwot.CustomerDefaultPrice
                .FirstOrDefault(p => p.CustomerID == _customer.Id && p.PriceUnit == OrderPrice.enumPriceUnit.Lot.ToString());

            if (eachPriceRow == null)
            {
                eachPriceRow = _dsAwot.CustomerDefaultPrice.NewCustomerDefaultPriceRow();
                eachPriceRow.CustomerID = _customer.Id;
                eachPriceRow.PriceUnit = OrderPrice.enumPriceUnit.Each.ToString();
                eachPriceRow.DefaultPrice = _eachPrice;
                _dsAwot.CustomerDefaultPrice.AddCustomerDefaultPriceRow(eachPriceRow);
            }
            else
            {
                eachPriceRow.DefaultPrice = _eachPrice;
            }

            if (lotPriceRow == null)
            {
                lotPriceRow = _dsAwot.CustomerDefaultPrice.NewCustomerDefaultPriceRow();
                lotPriceRow.CustomerID = _customer.Id;
                lotPriceRow.PriceUnit = OrderPrice.enumPriceUnit.Lot.ToString();
                lotPriceRow.DefaultPrice = _lotPrice;
                _dsAwot.CustomerDefaultPrice.AddCustomerDefaultPriceRow(lotPriceRow);
            }
            else
            {
                lotPriceRow.DefaultPrice = _lotPrice;
            }
        }

        private DocumentFile SaveMasterList()
        {
            const string rootDirName = "AWOT";
            const string masterListDirName= "Master Lists";

            var mediaType = Path.GetExtension(_masterListFileName);

            var rootDirectory = DocumentManager.GetFolder(rootDirName) ??
                                DocumentManager.CreateFolder(rootDirName);

            var masterListDirectory = DocumentManager.GetFolder(rootDirectory, masterListDirName) ??
                                      DocumentManager.CreateFolder(rootDirectory, masterListDirName);

            var fileName = _customer.Name + mediaType;
            var masterListFile = DocumentManager.GetFile(masterListDirectory, fileName, mediaType) ??
                DocumentManager.CreateFile(masterListDirectory, fileName, mediaType);

            DocumentManager.Revise(masterListFile, _masterListFileName, UserManager.CurrentUser);

            return masterListFile;
        }

        #endregion

        #endregion

        #region DialogStatus

        public enum DialogStatus
        {
            Setup,
            Confirmation,
            Import,
            Results
        }

        #endregion

        #region MasterListImporter

        private class MasterListImporter
        {
            #region Fields

            private const string TEXT_PART = "part";
            private const string TEXT_DESCRIPTION = "partdescription";
            private const string TEXT_PGM = "pgm";
            private const string TEXT_PROD_CODE = "prodcode";
            private const string TEXT_IDENTITY = "ident";
            private const string TEXT_OSP_CODE = "osp code";
            private const string TEXT_PREFERRED = "preferred";
            private const string TEXT_ALT = "alt";
            private const string TEXT_MATERIAL_DESCRIPTION = "material desc";
            private const string TEXT_MASK = "mask";
            private const string TEXT_PART_MARK = "part mark";
            private const string TEXT_IDENT_CODES = "ident codes";

            #endregion

            #region Properties

            private string FileName { get; }

            private CustomerViewModel Customer { get; }

            private IDocumentManager DocumentManager { get; }

            #endregion

            #region Methods

            public MasterListImporter(string fileName, CustomerViewModel customer, IDocumentManager documentManager)
            {
                FileName = fileName;
                Customer = customer ?? throw new ArgumentNullException(nameof(customer));
                DocumentManager = documentManager ?? throw new ArgumentNullException(nameof(documentManager));
            }

            public ValidationResult ValidateFile() =>
                CheckFileExist()
                ?? CheckFileFormat()
                ?? CheckPreviousMasterLists()
                ?? ValidationResult.Success();

            public Task<List<MasterListPart>> GetPartsFromWorksheet()
            {
                return Task.Factory.StartNew(() =>
                {
                    // Assumption: User calls ValidateFile() first

                    try
                    {
                        var parts = new List<MasterListPart>();

                        var workbook = Workbook.Load(FileName);
                        var worksheet = workbook.Worksheets[0];

                        // Determine column -> value mapping
                        var nameToIndexMap = GetNameToIndexMap(worksheet.Rows.First().Cells);

                        // Import each row
                        foreach (var row in worksheet.Rows.Skip(1))
                        {
                            var part = new MasterListPart();

                            if (nameToIndexMap.TryGetValue(TEXT_PART, out var partIndex))
                            {
                                part.Name = row.Cells[partIndex].GetText()?.Trim().ToUpper();
                            }

                            if (nameToIndexMap.TryGetValue(TEXT_DESCRIPTION, out var descIndex))
                            {
                                part.Description = row.Cells[descIndex].GetText();
                            }

                            if (nameToIndexMap.TryGetValue(TEXT_PGM, out var pgmIndex))
                            {
                                part.Program = row.Cells[pgmIndex].GetText();
                            }

                            if (nameToIndexMap.TryGetValue(TEXT_PROD_CODE, out var prodCodeIndex))
                            {
                                part.ProductCode = row.Cells[prodCodeIndex].GetText();
                            }

                            if (nameToIndexMap.TryGetValue(TEXT_IDENTITY, out var identityIndex))
                            {
                                part.Identity = row.Cells[identityIndex].GetText();
                            }

                            if (nameToIndexMap.TryGetValue(TEXT_OSP_CODE, out var ospCodeIndex))
                            {
                                part.OspCode = row.Cells[ospCodeIndex].GetText();
                            }

                            if (nameToIndexMap.TryGetValue(TEXT_PREFERRED, out var preferredIndex))
                            {
                                part.Preferred = row.Cells[preferredIndex].GetText();
                            }

                            if (nameToIndexMap.TryGetValue(TEXT_ALT, out var altIndex))
                            {
                                part.Alt = row.Cells[altIndex].GetText();
                            }

                            if (nameToIndexMap.TryGetValue(TEXT_MATERIAL_DESCRIPTION, out var materialIndex))
                            {
                                part.MaterialDescription = row.Cells[materialIndex].GetText();
                            }

                            if (nameToIndexMap.TryGetValue(TEXT_MASK, out var maskIndex))
                            {
                                part.Mask = row.Cells[maskIndex].GetText();
                            }

                            if (nameToIndexMap.TryGetValue(TEXT_PART_MARK, out var partMaskIndex))
                            {
                                part.PartMark = row.Cells[partMaskIndex].GetText();
                            }

                            if (nameToIndexMap.TryGetValue(TEXT_IDENT_CODES, out var identCodesIndex))
                            {
                                part.IdentityCode = row.Cells[identCodesIndex].GetText();
                            }

                            parts.Add(part);
                        }

                        return parts;
                    }
                    catch (Exception exc)
                    {
                        LogManager.GetCurrentClassLogger().Error(exc, "Error getting data from master list file.");
                        return new List<MasterListPart>();
                    }
                });
            }

            private Dictionary<string, int> GetNameToIndexMap(WorksheetCellCollection cells)
            {
                var fields = new List<string>
                {
                    TEXT_PART,
                    TEXT_DESCRIPTION,
                    TEXT_PGM,
                    TEXT_PROD_CODE,
                    TEXT_IDENTITY,
                    TEXT_OSP_CODE,
                    TEXT_PREFERRED,
                    TEXT_ALT,
                    TEXT_MATERIAL_DESCRIPTION,
                    TEXT_MASK,
                    TEXT_PART_MARK,
                    TEXT_IDENT_CODES
                };

                var nameToIndexMap = new Dictionary<string, int>();

                foreach (var cell in cells)
                {
                    var headerText = cell.GetText()?.Trim().ToLowerInvariant();

                    if (string.IsNullOrEmpty(headerText))
                    {
                        continue;
                    }

                    var matchingField = fields.FirstOrDefault(f => f == headerText);

                    if (matchingField != null)
                    {
                        fields.Remove(matchingField);
                        nameToIndexMap.Add(matchingField, cell.ColumnIndex);
                    }
                }

                return nameToIndexMap;
            }

            private ValidationResult CheckFileExist()
            {
                if (File.Exists(FileName))
                {
                    return null;
                }

                return ValidationResult.Failure("File does not exist.");
            }

            private ValidationResult CheckFileFormat()
            {
                try
                {
                    var workbook = Workbook.Load(FileName);
                    var worksheet = workbook.Worksheets[0];
                    var header = worksheet.Rows[0];

                    var requiredFields = new List<string>
                    {
                        TEXT_PART,
                        TEXT_DESCRIPTION,
                        TEXT_PGM,
                        TEXT_PROD_CODE,
                        TEXT_IDENTITY,
                        TEXT_OSP_CODE,
                        TEXT_PREFERRED,
                        TEXT_ALT,
                        TEXT_MATERIAL_DESCRIPTION,
                        TEXT_MASK,
                        TEXT_PART_MARK,
                        TEXT_IDENT_CODES,
                    };

                    var spreadsheetFields = header.Cells.Select(c => c.GetText().ToLowerInvariant().Trim()).ToList();

                    var hasRequiredFields = !requiredFields.Except(spreadsheetFields).Any();

                    return hasRequiredFields ? null : ValidationResult.Failure("Invalid format");
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Warn(exc, "Unable to read master list.");
                    return ValidationResult.Failure("Could not read file.");
                }
            }

            private ValidationResult CheckPreviousMasterLists()
            {
                try
                {
                    var rootDir = DocumentManager.GetFolder("AWOT");

                    if (rootDir == null)
                    {
                        // No master lists have been added.
                        return null;
                    }

                    var masterListDir = DocumentManager.GetFolder(rootDir, "Master Lists");
                    if (masterListDir == null)
                    {
                        // No master lists have been added.
                        return null;
                    }

                    var mediaType = Path.GetExtension(FileName);
                    var dbFileName = Customer.Name + mediaType;

                    var masterList = DocumentManager.GetFile(masterListDir, dbFileName, mediaType);

                    if (masterList == null)
                    {
                        // No master list for this customer.
                        return null;
                    }

                    if (DocumentManager.MatchesAnyRevision(masterList, FileName))
                    {
                        return ValidationResult.Failure("This master list has already been imported");
                    }

                    return null;
                }
                catch (Exception exc)
                {
                    LogManager.GetCurrentClassLogger().Warn(exc, "Unable to read master list.");
                    return ValidationResult.Failure("Could not check previous revisions");
                }
            }

            #endregion
        }

        #endregion

        #region PricePoint

        private class PricePoint
        {
            #region Properties

            public int? MinQuantity { get; private set; }

            public decimal? MinWeight { get; private set; }

            public int? MaxQuantity { get; private set; }

            public decimal? MaxWeight { get; private set; }

            public OrderPrice.enumPriceUnit PriceUnit { get; private set; }

            public List<PartsDataset.PartProcessVolumePriceRow> Rows { get; private set; }

            #endregion

            #region Methods

            public void RecalculatePrices(decimal newTotal)
            {
                var oldTotal = Rows.Sum(p => p.Amount);

                var conversionFactor = newTotal / oldTotal;

                foreach (var row in Rows)
                {
                    row.Amount = row.Amount * conversionFactor;
                }
            }

            public static List<PricePoint> GetPricePoints(IEnumerable<PartsDataset.PartProcessVolumePriceRow> priceRows)
            {
                if (priceRows == null)
                {
                    throw new ArgumentNullException(nameof(priceRows));
                }

                var returnPricePoints = new List<PricePoint>();

                foreach (var priceRow in priceRows)
                {
                    var priceUnit = OrderPrice.ParsePriceUnit(priceRow.PriceUnit);

                    PricePoint existingPricePoint;

                    int? minQuantity = null;
                    int? maxQuantity = null;
                    decimal? minWeight = null;
                    decimal? maxWeight = null;

                    switch (OrderPrice.GetPriceByType(priceUnit))
                    {
                        case PriceByType.Quantity:
                            minQuantity = priceRow.IsMinValueNull() ? (int?)null : int.Parse(priceRow.MinValue);
                            maxQuantity = priceRow.IsMaxValueNull() ? (int?)null : int.Parse(priceRow.MaxValue);

                            existingPricePoint = returnPricePoints
                                .FirstOrDefault(p => p.PriceUnit == priceUnit && p.MinQuantity == minQuantity && p.MaxQuantity == maxQuantity);
                            break;
                        case PriceByType.Weight:
                            minWeight = priceRow.IsMinValueNull() ? (decimal?)null : decimal.Parse(priceRow.MinValue);
                            maxWeight = priceRow.IsMaxValueNull() ? (decimal?)null : decimal.Parse(priceRow.MaxValue);

                            existingPricePoint = returnPricePoints
                                .FirstOrDefault(p => p.PriceUnit == priceUnit && p.MinWeight == minWeight && p.MaxWeight == maxWeight);
                            break;
                        default:
                            existingPricePoint = null;
                            break;
                    }

                    if (existingPricePoint == null)
                    {
                        returnPricePoints.Add(new PricePoint
                        {
                            PriceUnit = priceUnit,
                            MinQuantity = minQuantity,
                            MaxQuantity = maxQuantity,
                            MinWeight = minWeight,
                            MaxWeight = maxWeight,
                            Rows = new List<PartsDataset.PartProcessVolumePriceRow> { priceRow }
                        });
                    }
                    else
                    {
                        existingPricePoint.Rows.Add(priceRow);
                    }
                }

                return returnPricePoints;
            }

            #endregion
        }

        #endregion

        #region ImportReportItem

        private class ImportReportItem
        {
            public MasterListPart Part { get; set; }

            public ImportStatus Status { get; set; }
        }

        #endregion

        #region ImportStatus

        private enum ImportStatus
        {
            Error,
            Added,
            Updated,
            UpdatedWithIncorrectOspCode
        }

        #endregion
    }
}
