using DWOS.Services.Messages;
using DWOS.Utilities;
using System;
using System.Threading.Tasks;

namespace DWOS.ViewModels
{
    public class EditOrderNoteViewModel : ViewModelBase
    {
        #region Fields

        private bool _isDirty;
        private int _orderId;
        private int _orderNoteId;
        private string _noteType;
        private string _noteText;

        #endregion

        #region Properties

        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    OnPropertyChanged(nameof(IsDirty));
                }
            }
        }

        public int OrderId
        {
            get => _orderId;
            set
            {
                if (_orderId != value)
                {
                    _orderId = value;
                    OnPropertyChanged(nameof(OrderId));
                    IsDirty = true;
                }
            }
        }

        public int OrderNoteId
        {
            get => _orderNoteId;
            set
            {
                if (_orderNoteId != value)
                {
                    _orderNoteId = value;
                    OnPropertyChanged(nameof(OrderNoteId));
                    IsDirty = true;
                }
            }
        }

        public string NoteType
        {
            get => _noteType;
            set
            {
                if (!string.Equals(_noteType, value, StringComparison.Ordinal))
                {
                    _noteType = value;
                    OnPropertyChanged(nameof(NoteType));
                    IsDirty = true;
                }
            }
        }

        public string NoteText
        {
            get => _noteText;
            set
            {
                if (!string.Equals(_noteText, value, StringComparison.Ordinal))
                {
                    _noteText = value;
                    OnPropertyChanged(nameof(NoteText));
                    IsDirty = true;
                }
            }
        }

        #endregion

        #region Methods

        public void Load(OrderNote note, bool isDirty)
        {
            InvalidateViewModel();

            if (note == null)
            {
                return;
            }

            OrderId = note.OrderId;
            OrderNoteId = note.OrderNoteId;
            NoteType = note.NoteType;
            NoteText = note.Note;
            IsDirty = isDirty;

            Validate();
        }

        public async Task<ViewModelResult> SaveAsync()
        {
            IsBusy = true;

            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            var noteService = ServiceContainer.Resolve<IOrderNoteService>();
            noteService.RootUrl = loginViewModel.ServerUrlWellFormed;

            var result = await noteService.SaveOrderNotesAsync(new SaveOrderNotesRequest
            {
                Note = new OrderNote
                {
                    Note = _noteText,
                    NoteType = _noteType,
                    OrderId = _orderId,
                    OrderNoteId = _orderNoteId
                },
                UserId = loginViewModel.UserProfile.UserId
            });

            IsBusy = false;
            return new ViewModelResult(result.Success, result.ErrorMessage);

        }

        public override void InvalidateViewModel()
        {
            OrderId = -1;
            OrderNoteId = -1;
            NoteType = null;
            NoteText = null;

            Errors.Clear();
            IsDirty = false;
        }

        protected override void Validate()
        {
            ValidateProperty(() => string.IsNullOrEmpty(_noteType), "Note Type is required");
            base.Validate();
        }

        #endregion
    }
}
