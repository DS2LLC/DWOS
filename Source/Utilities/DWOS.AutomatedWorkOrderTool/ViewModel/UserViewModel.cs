using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DWOS.AutomatedWorkOrderTool.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace DWOS.AutomatedWorkOrderTool.ViewModel
{
    public class UserViewModel : ViewModelBase
    {
        #region Fields

        private ImageSource _image;

        #endregion

        #region Properties

        public IUserManager UserManager { get; }

        public string Name => UserManager.CurrentUser?.Name ?? "No User";

        public string Title => UserManager.CurrentUser?.Title;

        public string Department => UserManager.CurrentUser?.Department;

        public ImageSource Image
        {
            get => _image;
            set => Set(nameof(Image), ref _image, value);
        }

        #endregion

        #region Methods

        public UserViewModel(IMessenger messenger, IUserManager userManager)
            : base(messenger)
        {
            UserManager = userManager;
            UserManager.UserChanged += OnUserChanged;
            UpdateUserImage();
        }

        public override void Cleanup()
        {
            base.Cleanup();
            UserManager.UserChanged -= OnUserChanged;
        }

        private void UpdateUserImage()
        {
            Image = UserManager.CurrentUser?.MediaId == null
                ? new BitmapImage(new Uri("/Images/nopicture_thumb.jpg", UriKind.Relative))
                : UserManager.GetImage(UserManager.CurrentUser);
        }

        #endregion

        #region Events

        private void OnUserChanged(object sender, UserChangedEventArgs args)
        {
            RaisePropertyChanged(nameof(Name));
            RaisePropertyChanged(nameof(Title));
            RaisePropertyChanged(nameof(Department));

            UpdateUserImage();
        }

        #endregion
    }
}
