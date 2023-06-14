using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using DWOS.ViewModels;
using DWOS.Utilities;
using Android.Graphics;
using System.Threading.Tasks;
using Android.Renderscripts;

namespace DWOS.Android
{
    /// <summary>
    /// Shows information about the current user in a dialog window.
    /// </summary>
    public class AboutFragment : DialogFragment
    {
        #region Fields

        public const string ABOUT_FRAGMENT_TAG = "AboutFragment";

        LogInViewModel _loginViewModel;
        ImageView _blurredUserImageView;
        ImageView _userImageView;
        TextView _userNameTextView;
        TextView _userTitleTextView;
        TextView _companyTextView;
        TextView _departmentTextView;
        TextView _serverVersionTextView;
        TextView _appVersionTextView;
        TextView _serverAddressTextView;
        CircleDrawable _circleDrawable;

        #endregion

        #region Methods

        public AboutFragment()
        {
            _loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
        }

        /// <summary>
        /// Called when the fragment is visible to the user and actively running.
        /// </summary>
        public override async void OnResume()
        {
            base.OnResume();

            var userProfile = _loginViewModel.UserProfile;

            if (userProfile != null)
            {
                var settings = ApplicationSettings.Settings;

                _userNameTextView.Text = userProfile.Name;
                _userTitleTextView.Text = userProfile.Title;
                _departmentTextView.Text = !string.IsNullOrEmpty(userProfile.Department) ? " - " + userProfile.Department : string.Empty;
                _serverAddressTextView.Text = _loginViewModel.ServerUrlWellFormed;
                _serverVersionTextView.Text = $"{settings.ServerVersion} - API {settings.ServerApiVersion}";
                _companyTextView.Text = settings.CompanyName;
                _appVersionTextView.Text = GetAppVersion();

                if (userProfile.Image != null)
                {
                    var userImage = await BitmapFactory.DecodeByteArrayAsync(userProfile.Image, 0, userProfile.Image.Length);
                    _circleDrawable = new CircleDrawable(userImage);
                    _userImageView.SetImageDrawable(_circleDrawable);
                    DisplayBlurredImage(userProfile.Image);
                }
            }
        }

        /// <summary>
        /// Called when the fragment is no longer in use.
        /// </summary>
        public override void OnDestroyView()
        {
            _userNameTextView.Dispose();
            _userTitleTextView.Dispose();
            _departmentTextView.Dispose();
            _serverAddressTextView.Dispose();
            _serverVersionTextView.Dispose();
            _companyTextView.Dispose();
            _userImageView.SetImageBitmap(null);
            _userImageView.Dispose();
            _blurredUserImageView.SetImageBitmap(null);
            _blurredUserImageView.Dispose();
            _appVersionTextView.Dispose();

            _circleDrawable?.Dispose();

            base.OnDestroyView();
        }

        /// <summary>
        /// Override to build your own custom Dialog container.
        /// </summary>
        /// <param name="savedInstanceState">The last saved instance state of the Fragment,
        /// or null if this is a freshly created Fragment.</param>
        /// <returns>
        /// To be added.
        /// </returns>
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var builder = new AlertDialog.Builder(Activity);
            builder.SetPositiveButton("OK", (sender, args) => { });
            var factory = LayoutInflater.From(Activity);
            var view = factory.Inflate(Resource.Layout.AboutFragmentLayout, null);
            _userImageView = view.FindViewById<ImageView>(Resource.Id.imageViewUser);
            _blurredUserImageView = view.FindViewById<ImageView>(Resource.Id.imageViewBlurredUser);
            _userNameTextView = view.FindViewById<TextView>(Resource.Id.textViewName);
            _userTitleTextView = view.FindViewById<TextView>(Resource.Id.textViewTitle);
            _companyTextView = view.FindViewById<TextView>(Resource.Id.textViewCompany);
            _departmentTextView = view.FindViewById<TextView>(Resource.Id.textViewDepartment);
            _serverVersionTextView = view.FindViewById<TextView>(Resource.Id.textViewServerVersion);
            _serverAddressTextView = view.FindViewById<TextView>(Resource.Id.textViewServerAddress);
            _appVersionTextView = view.FindViewById<TextView>(Resource.Id.textViewAppVersion);
            builder.SetView(view);

            return builder.Create();
        }

        private void DisplayBlurredImage(byte[] copyImage)
        {
            int radius = 5;

            Task.Factory.StartNew(() =>
            {
                Bitmap bmp = CreateBlurredImage(radius, copyImage);
                return bmp;
            })
            .ContinueWith(task =>
            {
                using (var bmp = task.Result)
                {
                    _blurredUserImageView.SetImageBitmap(bmp);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private Bitmap CreateBlurredImage(int radius, byte[] image)
        {
            // Create another bitmap that will hold the results of the filter.
            int fudgeFactor = (int)(300 * Resources.DisplayMetrics.Density + 0.5f);
            var blurredBitmap = ImageConverter.DecodeSampledBitmapFromBytes(image, fudgeFactor, fudgeFactor);
            
            // Create the Renderscript instance that will do the work.
            RenderScript rs = RenderScript.Create(Activity);

            // Allocate memory for Renderscript to work with
            Allocation input = Allocation.CreateFromBitmap(rs, blurredBitmap, Allocation.MipmapControl.MipmapFull, AllocationUsage.Script);
            Allocation output = Allocation.CreateTyped(rs, input.Type);

            // Load up an instance of the specific script that we want to use.
            ScriptIntrinsicBlur script = ScriptIntrinsicBlur.Create(rs, Element.U8_4(rs));
            script.SetInput(input);

            // Set the blur radius
            script.SetRadius(radius);

            // Start the ScriptIntrinisicBlur
            script.ForEach(output);

            // Copy the output to the blurred bitmap
            output.CopyTo(blurredBitmap);

            return blurredBitmap;
        }

        private string GetAppVersion()
        {
            var versionService = ServiceContainer.Resolve<IVersionService>();
            return versionService.GetAppVersion();
        }

        #endregion
    }
}