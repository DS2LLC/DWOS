using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using DWOS.Services;
using DWOS.Services.Messages;
using DWOS.Utilities;
using DWOS.ViewModels;
using Newtonsoft.Json;

namespace DWOS.Android
{
    [Activity(Label = "Order Note", Name = "dwos.android.EditOrderNoteActivity", WindowSoftInputMode = SoftInput.AdjustPan)]
    public class EditOrderNoteActivity : BaseActivity
    {
        #region Fields

        public const string INTENT_INFO = "OrderNote";
        public const string INTENT_OUT_ORDER_ID = "OrderId";
        public const string INTENT_OUT_SUCCESS = "Success";
        public const string INTENT_OUT_ERROR_MESSAGE = "ErrorMessage";

        private const string BUNDLE_INFO = "OrderNote";
        private const string BUNDLE_DIRTY = "IsDirty";

        private readonly List<string> _noteTypes = new List<string>
        {
            OrderNote.Internal,
            OrderNote.External
        };

        private EditOrderNoteViewModel _noteViewModel;
        private LogInViewModel _loginViewModel;
        private TextView _textViewWorkOrder;
        private Spinner _spinnerNoteType;
        private EditText _editTextOrderNotes;
        private bool _hudVisible;

        #endregion

        #region Methods

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                _loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
                _noteViewModel = ServiceContainer.Resolve<EditOrderNoteViewModel>();

                SetContentView(Resource.Layout.EditOrderNoteLayout);

                SupportActionBar.SetDisplayHomeAsUpEnabled(showHomeAsUp: true);
                SupportActionBar.Subtitle = GetString(Resource.String.LoggedInFormat,
                    _loginViewModel.UserProfile.Name);

                _textViewWorkOrder = FindViewById<TextView>(Resource.Id.textViewWorkOrder);
                _spinnerNoteType = FindViewById<Spinner>(Resource.Id.spinnerNoteType);
                _editTextOrderNotes = FindViewById<EditText>(Resource.Id.editTextOrderNotes);

                // Setup note type spinner
                var adapter = new ArrayAdapter<string>(BaseContext, Resource.Layout.SpinnerItem, _noteTypes);
                adapter.SetDropDownViewResource(Resource.Layout.SpinnerDropDownItem);
                _spinnerNoteType.Adapter = adapter;

                // Load & show note info
                if (savedInstanceState == null)
                {
                    LoadIntent();
                }
                else
                {
                    LoadBundle(savedInstanceState);
                }

                ShowCurrentNote();

                // Register view listeners
                RegisterViewEvents();
            }
            catch (Exception exc)
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                logService.LogError("Error in OnCreate", exc);
                DWOSApplication.Current.RestoreMainActivityFromCrash = true;
                base.RestartApp();
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            UnregisterViewEvents();
        }

        protected override void OnDestroy()
        {
            if (_hudVisible)
            {
                _hudVisible = false;
                AndHUD.Shared.Dismiss();
            }

            UnregisterViewEvents();
            _textViewWorkOrder?.Dispose();
            _spinnerNoteType?.Dispose();
            _editTextOrderNotes?.Dispose();
            base.OnDestroy();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutString(BUNDLE_INFO, JsonConvert.SerializeObject(new OrderNote
            {
                OrderId = _noteViewModel.OrderId,
                OrderNoteId = _noteViewModel.OrderNoteId,
                Note = _noteViewModel.NoteText,
                NoteType = _noteViewModel.NoteType
            }));

            outState.PutBoolean(BUNDLE_DIRTY, _noteViewModel.IsDirty);
        }

        private void LoadIntent()
        {
            var orderNoteJson = Intent.GetStringExtra(INTENT_INFO);

            if (string.IsNullOrEmpty(orderNoteJson))
            {
                const string message = "Unable to show order note.";
                Toast.MakeText(this, message, ToastLength.Short)
                    .Show();
            }
            else
            {
                var orderNote = JsonConvert.DeserializeObject<OrderNote>(orderNoteJson,
                    JsonSerializationSettings.Settings);
                _noteViewModel.Load(orderNote, false);
            }
        }

        private void LoadBundle(Bundle bundle)
        {
            var orderNoteJson = bundle.GetString(BUNDLE_INFO);
            var isDirty = bundle.GetBoolean(BUNDLE_DIRTY);

            if (string.IsNullOrEmpty(orderNoteJson))
            {
                const string message = "Unable to show order note.";
                Toast.MakeText(this, message, ToastLength.Short)
                    .Show();
            }
            else
            {
                var orderNote = JsonConvert.DeserializeObject<OrderNote>(orderNoteJson,
                    JsonSerializationSettings.Settings);
                _noteViewModel.Load(orderNote, false);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.order_note, menu);
            var isEnabled = _noteViewModel.IsValid && !_noteViewModel.IsBusy;

            menu.FindItem(Resource.Id.action_noteSave).SetEnabled(isEnabled, this, Resource.Drawable.ic_action_accept);

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_noteSave:
                    if (_loginViewModel.IsLicenseActivated)
                    {
                        var _ = SaveAsync();
                        return true;
                    }

                    LogOutUserWithExpiredMessage();
                    return false;
                case global::Android.Resource.Id.Home:
                    if (_noteViewModel.IsDirty && _noteViewModel.IsValid)
                    {
                        ConfirmExit();
                    }
                    else
                    {
                        Finish();
                    }
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed()
        {
            if (_noteViewModel.IsDirty && _noteViewModel.IsValid)
            {
                ConfirmExit();
            }
            else
            {
                Finish();
            }
        }
        protected override void RegisterViewModelEvents()
        {
            _noteViewModel.PropertyChanged += NoteViewModel_PropertyChanged;
            _noteViewModel.IsValidChanged += NoteViewModel_IsValidChanged;
            _noteViewModel.IsBusyChanged += NoteViewModel_IsValidChanged;
        }

        protected override void UnregisterViewModelEvents()
        {
            _noteViewModel.PropertyChanged -= NoteViewModel_PropertyChanged;
            _noteViewModel.IsValidChanged -= NoteViewModel_IsValidChanged;
            _noteViewModel.IsBusyChanged -= NoteViewModel_IsValidChanged;
        }

        private void RegisterViewEvents()
        {
            _editTextOrderNotes.TextChanged += EditTextOrderNotes_TextChanged;
            _spinnerNoteType.ItemSelected += SpinnerNoteType_ItemSelected;
        }

        private void UnregisterViewEvents()
        {
            _editTextOrderNotes.TextChanged -= EditTextOrderNotes_TextChanged;
            _spinnerNoteType.ItemSelected -= SpinnerNoteType_ItemSelected;
        }

        private void ShowCurrentNote()
        {
            _textViewWorkOrder.Text = $"WORK ORDER {_noteViewModel.OrderId}";
            _editTextOrderNotes.Text = _noteViewModel.NoteText;
            _spinnerNoteType.SetSelection(Math.Max(_noteTypes.IndexOf(_noteViewModel.NoteType), 0));
        }

        private async Task SaveAsync()
        {
            AndHUD.Shared.Show(this, "Saving Order Notes");
            _hudVisible = true;
            var result = await _noteViewModel.SaveAsync();
            var returnIntent = new Intent();
            returnIntent.PutExtra(INTENT_OUT_ORDER_ID, _noteViewModel.OrderId);
            returnIntent.PutExtra(INTENT_OUT_SUCCESS, result.Success);
            returnIntent.PutExtra(INTENT_OUT_ERROR_MESSAGE, result.ErrorMessage);
            SetResult(Result.Ok, returnIntent);
            Finish();
        }

        private void ConfirmExit()
        {
            var saveConfirmDialog = SaveConfirmFragment.New(message: "Would you like to save changes to the note?", title: "Save",
                    positiveCallback: async (dialog) =>
                    {
                        dialog.Dismiss();
                        await SaveAsync();
                    },
                    negativeCallback: (dialog) =>
                    {
                        dialog.Dismiss();
                        _noteViewModel.InvalidateViewModel();
                        Finish();
                    },
                    neutralCallback: (dialog) =>
                    {
                        dialog.Dismiss();
                    });

            var transaction = FragmentManager.BeginTransaction();
            saveConfirmDialog.Show(transaction, SaveConfirmFragment.LOGOUT_CONFIRM_FRAGMENT_TAG);
        }

        #endregion

        #region Events

        private void EditTextOrderNotes_TextChanged(object sender, global::Android.Text.TextChangedEventArgs e)
        {
            try
            {
                _noteViewModel.NoteText = _editTextOrderNotes.Text;
            }
            catch (Exception exc)
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                logService.LogError("Error changing note text", exc);
            }
        }

        private void SpinnerNoteType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                _noteViewModel.NoteType = _spinnerNoteType.GetItemAtPosition(e.Position).ToString();
            }
            catch (Exception exc)
            {
                var logService = ServiceContainer.Resolve<ILogService>();
                logService.LogError("Error changing note type", exc);
            }

        }

        private void NoteViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EditOrderNoteViewModel.OrderNoteId))
            {
                ShowCurrentNote();
            }
        }

        private void NoteViewModel_IsValidChanged(object sender, EventArgs e)
        {
            // Enable/disable accept menu item
            InvalidateOptionsMenu();
        }

        #endregion
    }
}