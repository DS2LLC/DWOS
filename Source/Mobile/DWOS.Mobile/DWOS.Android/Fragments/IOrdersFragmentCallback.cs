namespace DWOS.Android
{
    /// <summary>
    /// Callback interface for <see cref="OrdersListFragment"/>.
    /// </summary>
    public interface IOrdersFragmentCallback
    {
        /// <summary>
        /// Called when the user selects an order.
        /// </summary>
        /// <param name="orderId"></param>
        void OnOrderSelected(int orderId);
    }
}
