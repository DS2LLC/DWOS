namespace DWOS
{
    /// <summary>
    /// Defines navigation functionality.
    /// </summary>
    public interface IViewNavigationService
    {
        /// <summary>
        /// Navigates to view.
        /// </summary>
        /// <param name="viewName">View to navigate to.</param>
        void NavigateToView(ViewName viewName);
    }
}
