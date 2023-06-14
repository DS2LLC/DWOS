using DWOS.ViewModels;

namespace DWOS.Services
{
    public class AuthenticationInfoProvider
    {
        private LogInViewModel loginViewModel;

        public AuthenticationInfoProvider(LogInViewModel loginViewModel)
        {
            this.loginViewModel = loginViewModel;
        }

        public AuthenticationInfo Info
        {
            get
            {
                var pin = loginViewModel.UserPin;
                var userId = loginViewModel.UserProfile?.UserId;

                if (userId.HasValue && !string.IsNullOrEmpty(pin))
                {
                    return new AuthenticationInfo
                    {
                        UserId = userId.Value,
                        Pin = pin
                    };
                }

                return null;
            }
        }
    }
}
