using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace DWOS.Utilities.Validation
{
    /// <summary>
    /// Manages multiple <see cref="DisplayValidator"/> instances for a Form.
    /// </summary>
    public class ValidatorManager : IDisposable
    {
        #region Fields

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the validators.
        /// </summary>
        /// <value> The validators. </value>
        protected List <DisplayValidator> Validators { get; set; } = new List <DisplayValidator>();

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="ValidatorManager" /> is enabled.
        /// </summary>
        /// <value> <c>true</c> if enabled; otherwise, <c>false</c> . </value>
        public bool Enabled { get; set; } = true;

        public IValidationSummary ValidationSummary { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Force validation of all validators.
        /// </summary>
        /// <returns> Returns true if all controls are vaild, else if any control is invalid returns false. </returns>
        public bool ValidateControls()
        {
            var isValid = true;

            ValidationSummary?.Reset();

            if(Enabled) //only validate controls if validation is enabled
            {
                var cancelEvent = new CancelEventArgs();

                foreach(var item in Validators)
                {
                    if (!item.IsEnabled)
                    {
                        continue;
                    }

                    item.Validator.ValidateControl(this, cancelEvent);

                    if(cancelEvent.Cancel)
                    {
                        isValid = false;

                        ValidationSummary?.StatusUpdate(item, !cancelEvent.Cancel);
                    }
                }
            }

            ValidationSummary?.Complete();

            return isValid;
        }

        /// <summary>
        ///     Adds the validator to collection of validators.
        /// </summary>
        /// <param name="validator"> The validator. </param>
        public void Add(DisplayValidator validator)
        {
            Validators.Add(validator);
        }

        /// <summary>
        ///     Remove the validator from the list
        /// </summary>
        /// <param name="validator"> </param>
        public void Remove(DisplayValidator validator)
        {
            Validators.Remove(validator);
        }

        public DisplayValidator Find(Control validatingControl)
        {
            return Validators.FirstOrDefault(v => v.Validator != null && v.Validator.ValidatingControl == validatingControl);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (Validators == null)
            {
                return;
            }

            foreach (var v in Validators)
            {
                v.Dispose();
            }

            Validators.Clear();
            Validators = null;
        }

        #endregion
    }
}