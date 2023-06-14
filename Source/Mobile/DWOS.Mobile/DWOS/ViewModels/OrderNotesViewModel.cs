using DWOS.Services.Messages;
using DWOS.Utilities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS.ViewModels
{
    /// <summary>
    /// View Model for part detail functionality.
    /// </summary>
    public class OrderNotesViewModel : ViewModelBase
    {
        #region Fields

        private List<OrderNote> _notes =
            new List<OrderNote>();

        int _orderId;

        #endregion

        /// <summary>
        /// Gets or sets the current part.
        /// </summary>
        public List<OrderNote> Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                OnPropertyChanged(nameof(Notes));
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
                OnPropertyChanged(nameof(OrderId));
            }
        }

        #region Methods

        /// <summary>
        /// Populates this instance with an order's notes asynchronously.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ViewModelResult> GetNotesAsync(int orderId, CancellationToken cancellationToken = default(CancellationToken))
        {
            IsBusy = true;
            InvalidateViewModel();

            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();
            var orderNoteService = ServiceContainer.Resolve<IOrderNoteService>();
            if (!loginViewModel.IsLoggedIn)
                return new ViewModelResult(success: false, errorMessage: NotLoggedInMessage);

            orderNoteService.RootUrl = loginViewModel.ServerUrlWellFormed;
            var request = new OrderNotesRequest
            {
                OrderId = orderId,
                UserId = loginViewModel.UserProfile.UserId
            };

            var response = await orderNoteService.GetOrderNotesAsync(request, cancellationToken);
            if (string.IsNullOrEmpty(response.ErrorMessage))
            {
                OrderId = orderId;
                Notes = response.Notes ?? new List<OrderNote>(); // Set instead of re-use to trigger property change event
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
            Notes = new List<OrderNote>();

            base.InvalidateViewModel();
        }

        #endregion
    }
}
