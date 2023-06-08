using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters;
using DWOS.UI.Utilities;
using GalaSoft.MvvmLight.CommandWpf;
using NLog;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DWOS.UI.Admin.ViewModels
{
    internal class OrderApprovalTermsViewModel
        : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// Occurs when the user accepts the dialog.
        /// </summary>
        public event EventHandler Accepted;

        private readonly ApplicationSettingsDataSet.OrderApprovalTermDataTable _dtTerms =
            new ApplicationSettingsDataSet.OrderApprovalTermDataTable();

        private Term _selectedTerm;

        #endregion

        #region Properties

        public ObservableCollection<Term> Terms { get; } =
            new ObservableCollection<Term>();

        public Term SelectedTerm
        {
            get => _selectedTerm;
            set => Set(nameof(SelectedTerm), ref _selectedTerm, value);
        }

        public ICommand AddTerm { get; }

        public ICommand DeleteSelectedTerm { get; }

        public ICommand Accept { get; }

        #endregion

        #region Methods

        public OrderApprovalTermsViewModel()
        {
            AddTerm = new RelayCommand(
                () =>
                {
                    var newTermRow = _dtTerms.AddOrderApprovalTermRow(
                        "Terms",
                        string.Empty,
                        true);

                    Terms.Add(new Term(newTermRow));
                });

            DeleteSelectedTerm = new RelayCommand(DoDeleteSelected, CanDeleteSelected);
            Accept = new RelayCommand(DoAccept, CanAccept);
        }

        private void DoDeleteSelected()
        {
            if (!CanDeleteSelected())
            {
                return;
            }

            var selection = _selectedTerm;

            if (selection != null)
            {
                SelectedTerm = null;
                Terms.Remove(selection);
                selection.Row.Delete();
            }
        }

        private bool CanDeleteSelected()
        {
            try
            {
                var selection = _selectedTerm;

                if (selection == null)
                {
                    return false;
                }

                // Do not allow deletion of in-use terms
                int usageCount;
                using (var taTerms = new OrderApprovalTermTableAdapter())
                {
                    usageCount = taTerms.GetUsageCount(_selectedTerm.Row.OrderApprovalTermID)
                        ?? -1;
                }

                return usageCount == 0;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Cannot delete selected term.");

                return false;
            }
        }

        private void DoAccept()
        {
            if (!CanAccept())
            {
                return;
            }

            using (var taTerms = new OrderApprovalTermTableAdapter())
            {
                taTerms.Update(_dtTerms);
            }

            Accepted?.Invoke(this, EventArgs.Empty);
        }

        private bool CanAccept() => CheckDuplicateNames()
                && Terms.All(pc => string.IsNullOrEmpty(pc.ValidateAll()));

        private bool CheckDuplicateNames()
        {
            var duplicateNames = Terms
                .GroupBy(pc => pc.Name)
                .Where(pcGroup => pcGroup.Count() > 1)
                .Select(pcGroup => pcGroup.Key)
                .ToList();

            foreach (var term in Terms)
            {
                term.MarkAsDuplicate(duplicateNames.Contains(term.Name));
            }

            return duplicateNames.Count == 0;
        }

        public void LoadData()
        {
            using (var taTerms = new OrderApprovalTermTableAdapter())
            {
                taTerms.Fill(_dtTerms);
            }

            Terms.Clear();

            foreach (var termRow in _dtTerms)
            {
                Terms.Add(new Term(termRow));
            }
        }

        #endregion

        #region Term

        public class Term : ViewModelBase
        {
            #region Fields

            private bool _isDuplicate;

            #endregion

            public ApplicationSettingsDataSet.OrderApprovalTermRow Row { get; }

            public string Name
            {
                get => Row.Name;
                set
                {
                    if (Row.Name != value)
                    {
                        Row.Name = value ?? string.Empty;
                        RaisePropertyChanged(nameof(Name));
                    }
                }
            }

            public string Terms
            {
                get => Row.Terms;
                set
                {
                    if (Row.Terms != value)
                    {
                        Row.Terms = value;
                        RaisePropertyChanged(nameof(Terms));
                    }
                }
            }

            public bool Active
            {
                get => Row.Active;
                set
                {
                    if (Row.Active != value)
                    {
                        Row.Active = value;
                        RaisePropertyChanged(nameof(Active));
                    }
                }
            }

            public Term(ApplicationSettingsDataSet.OrderApprovalTermRow row)
            {
                Row = row ?? throw new ArgumentNullException(nameof(row));
            }

            public override string Validate(string propertyName)
            {
                if (propertyName == nameof(Name))
                {
                    if (string.IsNullOrEmpty(Name))
                    {
                        return "Name is required.";
                    }

                    if (_isDuplicate)
                    {
                        return "Name cannot be duplicate.";
                    }
                }

                // Validation of EmailAddress occurs in UI

                return null;
            }

            public override string ValidateAll() => Validate(nameof(Name));

            public void MarkAsDuplicate(bool isDuplicate)
            {
                if (_isDuplicate != isDuplicate)
                {
                    _isDuplicate = isDuplicate;
                    RaisePropertyChanged(nameof(Name));
                }
            }
        }

        #endregion
    }
}
