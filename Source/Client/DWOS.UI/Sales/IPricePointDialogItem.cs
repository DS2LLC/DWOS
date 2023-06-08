using DWOS.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DWOS.UI.Sales
{
    /// <summary>
    /// A price point item on <see cref="PricePointDialog"/>.
    /// </summary>
    public interface IPricePointDialogItem : INotifyPropertyChanged, IDataErrorInfo, IComparable
    {
        /// <summary>
        /// Gets the currently available price calculation options.
        /// </summary>
        IEnumerable<OrderPrice.enumPriceUnit> CalculateByOptions { get; }

        /// <summary>
        /// Gets or sets the currently selected price calculation option.
        /// </summary>
        OrderPrice.enumPriceUnit CalculateBy { get; set; }

        /// <summary>
        /// Gets the display string of the price point.
        /// </summary>
        string DisplayString { get; }

        /// <summary>
        /// Gets the string representation of the current minimum value (inclusive).
        /// </summary>
        string MinValueString { get; }

        /// <summary>
        /// Gets the string representation of the current maximum value (inclusive).
        /// </summary>
        string MaxValueString { get; }

        /// <summary>
        /// Gets or sets a value indicating if the existence of a maximum value.
        /// </summary>
        bool HasMaxValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the opposite of <see cref="HasMaxValue"/>
        /// </summary>
        /// <remarks>
        /// This is intended as a convenience property for use with radio buttons.
        /// </remarks>
        bool EraseMaxValue { get; set; }

        /// <summary>
        /// Gets the Mask value to use for numeric editors.
        /// </summary>
        string EditorMask { get; }

        /// <summary>
        /// Gets the Format value to use for numeric editors.
        /// </summary>
        string EditorFormat { get; }

        /// <summary>
        /// Gets the object type of the minimum value editor.
        /// </summary>
        Type MinEditorType { get; }

        /// <summary>
        /// Gets the object type of the maximum value editor.
        /// </summary>
        Type MaxEditorType { get; }

        /// <summary>
        /// Gets the original row identifier associated with this instance
        /// </summary>
        /// <remarks>
        /// The original row identifier if this instance is associated with an existing row. Otherwise, <c>null</c>.
        /// </remarks>
        int OriginalRowId { get; }

        /// <summary>
        /// Validates this item using the previous and next values on the list.
        /// </summary>
        /// <remarks>
        /// This method should identify any problems with this item. These
        /// include, but are not limited to, basic validation issues (such
        /// as having a larger minimum than maximum) and gaps between items.
        /// </remarks>
        /// <param name="previousValue"></param>
        /// <param name="nextValue"></param>
        void Validate(IPricePointDialogItem previousValue, IPricePointDialogItem nextValue);
    }
}
