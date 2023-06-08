using DWOS.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DWOS.UI.Sales
{
    /// <summary>
    /// Data context for <see cref="PricePointDialog"/>.
    /// </summary>
    public interface IPricePointDialogContext
    {
        /// <summary>
        /// Gets the window title.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the identifying label for the item.
        /// </summary>
        string ItemLabel { get; }

        /// <summary>
        /// Gets the current item's value.
        /// </summary>
        string ItemValue { get; }

        /// <summary>
        /// Gets the 'accept Changes' command.
        /// </summary>
        ICommand AcceptCommand { get; }

        /// <summary>
        /// Gets the 'add price point' command
        /// </summary>
        ICommand AddCommand { get; }

        /// <summary>
        /// Gets the 'delete selected price point' command.
        /// </summary>
        ICommand DeleteCommand { get; }

        /// <summary>
        /// Gets a value indicating if data in this context is valid.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Gets a collection of available pricing options.
        /// </summary>
        IEnumerable<PriceByType> PriceByOptions { get; }

        /// <summary>
        /// Gets a collection of currently selectable price point options.
        /// </summary>
        /// <remarks>
        /// This can change as <see cref="SelectedPriceByOption"/> changes.
        /// </remarks>
        ObservableCollection<IPricePointDialogItem> PricePoints { get; }

        /// <summary>
        /// Gets an enumerable of all price point options.
        /// </summary>
        /// <remarks>
        /// Unlike <see cref="PricePoints"/>, this value should not change as <see cref="SelectedPriceByOption"/> changes.
        /// </remarks>
        IEnumerable<IPricePointDialogItem> AllPricePoints { get; }

        /// <summary>
        /// Gets or sets the currently selected pricing option.
        /// </summary>
        PriceByType SelectedPriceByOption { get; set; }

        /// <summary>
        /// Gets or sets the currently selected price point.
        /// </summary>
        IPricePointDialogItem SelectedPricePoint { get; set; }

        /// <summary>
        /// Gets a value indicating if this context is using volume discounts.
        /// </summary>
        bool UsingVolumeDiscounts { get; }

        /// <summary>
        /// Occurs when a user accepts changes made.
        /// </summary>
        event EventHandler Accept;

        /// <summary>
        /// Occurs when there is an error while saving changes made.
        /// </summary>
        event EventHandler SaveFailed;
    }
}