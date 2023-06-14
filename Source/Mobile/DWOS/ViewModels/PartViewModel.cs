using DWOS.Services.Messages;
using DWOS.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS.ViewModels
{
    /// <summary>
    /// View Model for part detail functionality.
    /// </summary>
    public class PartViewModel : ViewModelBase
    {
        #region Fields

        PartDetailInfo _part;
        int _orderId; 

        #endregion

        /// <summary>
        /// Gets or sets the current part.
        /// </summary>
        public PartDetailInfo Part
        {
            get { return _part; }
            set 
            { 
                _part = value;
                OnPropertyChanged("Part"); 
            }
        }

        /// <summary>
        /// Gets or sets the current order ID.
        /// </summary>
        public int OrderId
        {
            get { return _orderId; }
            set
            {
                _orderId = value;
                OnPropertyChanged("OrderId");
            }
        }

        #region Methods

        /// <summary>
        /// Populates this instance with an order's part asynchronously.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ViewModelResult> GetPartAsync(int orderId, CancellationToken cancellationToken = default(CancellationToken))
        {
            IsBusy = true;
            InvalidateViewModel();
            
            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            var partService = ServiceContainer.Resolve<IPartService>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            partService.RootUrl = loginViewModel.ServerUrlWellFormed;
            var request = new PartDetailRequest
            {
                OrderId = orderId,
                UserId = loginViewModel.UserProfile.UserId
            };

            var response = await partService.GetOrderPartAsync(request, cancellationToken);
            if (string.IsNullOrEmpty(response.ErrorMessage))
            {
                OrderId = orderId;
                Part = response.PartDetail;
            }

            IsBusy = false;
            
            var result = new ViewModelResult(response.Success, response.ErrorMessage);
            return result;
        }

        /// <summary>
        /// Invalidates the properties that represent state.
        /// </summary>
        public override void InvalidateViewModel()
        {
            OrderId = -1;
            Part = null;

            base.InvalidateViewModel();
        } 

        #endregion
    }
}
