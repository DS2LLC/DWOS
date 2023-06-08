using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinMaskedEdit;
using NLog;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Smartbox creates the type of input control needed based on InputType.
    /// </summary>
    public partial class SmartBox : UserControl
    {
        #region Events

        public event EventHandler SelectionChanged;
        public event EventHandler ValueChanged;
        //public event EventHandler ButtonClicked;

        #endregion

        #region Properties
        
        public InputType InputType { get; private set; }

        private Control InputControl { get; set; }

        #endregion

        #region Methods
        
        public SmartBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Creates the input control required baesd on input type.
        /// </summary>
        /// <param name="inputType">Type of the input.</param>
        /// <param name="listID">The list identifier.</param>
        /// <param name="numericUnits">The numeric units.</param>
        /// <param name="defaultValue">The default value.</param>
        public Control CreateControl(InputType inputType, int listID, string numericUnits, string defaultValue = null)
        {
            RemoveControl();

            InputType = inputType;

            this.InputControl = ControlUtilities.CreateControl(inputType, listID, numericUnits, defaultValue);
            this.Controls.Add(this.InputControl);
            this.InputControl.Dock = DockStyle.Fill;

            this.InputControl.KeyPress += SmartBox_KeyPress;
            this.InputControl.GotFocus += SmartBox_GotFocus;
            this.InputControl.TextChanged += InputControl_TextChanged;
            //if (InputControl is Infragistics.Win.Misc.UltraButton)
            //    //((Infragistics.Win.Misc.UltraButton)InputControl).Click += SmartBox_ButtonClicked;
            //    ((Infragistics.Win.Misc.UltraButton)InputControl).Click += delegate (object sender, EventArgs e) { SmartBox_ButtonClicked(sender, e, "This is From Button1"); };
            if (InputControl is UltraComboEditor)
                ((UltraComboEditor)InputControl).SelectionChanged += SmartBox_SelectionChanged;

            return this.InputControl;
        }

        /// <summary>
        /// Removes the input control.
        /// </summary>
        private void RemoveControl()
        {
            this.Controls.Clear();

            if(this.InputControl != null)
            {
                this.InputControl.KeyPress -= SmartBox_KeyPress;
                this.InputControl.GotFocus -= SmartBox_GotFocus;

                if (InputControl is UltraComboEditor)
                    ((UltraComboEditor)InputControl).SelectionChanged -= SmartBox_SelectionChanged;

                this.InputControl.Dispose();
            }

            this.InputControl = null;
        }

        /// <summary>
        /// Sets the controls value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetValue(string value)
        {
            value = CleanAnswer(value);
            SetAnswer(value);
        }

        /// <summary>
        /// Sets the default value of the control based on its type.
        /// </summary>
        public void SetDefaultValue()
        {
            if (this.InputControl is UltraCheckEditor)
                ((UltraCheckEditor)this.InputControl).Checked = false;
            else if (this.InputControl is UltraCurrencyEditor)
                ((UltraCurrencyEditor)this.InputControl).ValueObject = 0;
            else if (this.InputControl is UltraDateTimeEditor)
                ((UltraDateTimeEditor)this.InputControl).Value = DateTime.Now;
            else if (this.InputControl is UltraTextEditor)
                ((UltraTextEditor)this.InputControl).Value = null;
            else if (this.InputControl is UltraNumericEditor)
                ((UltraNumericEditor)this.InputControl).Value = ((UltraNumericEditor)this.InputControl).MinValue;
            else if(this.InputControl is UltraComboEditor)
            {
                if(((UltraComboEditor) this.InputControl).Items.Count > 0)
                    ((UltraComboEditor) this.InputControl).SelectedIndex = 0;
            }
            else
                this.InputControl.Text = null;
        }

        public void SetFocus(bool selectText)
        {
            if (InputControl != null)
            {
                InputControl.Focus();

                if (selectText)
                    InputControl.SelectAllText();
            }
        }

        public object GetValue()
        {
            if (this.InputControl == null)
                return null;

            if (this.InputControl is UltraCheckEditor)
                return ((UltraCheckEditor)this.InputControl).Checked;

            if (this.InputControl is UltraCurrencyEditor)
                return ((UltraCurrencyEditor)this.InputControl).ValueObject;

            if (this.InputControl is UltraTextEditor)
                return ((UltraTextEditor)this.InputControl).Value;

            if (this.InputControl is UltraNumericEditor)
            {
                ((UltraNumericEditor)this.InputControl).MaskDisplayMode = MaskMode.IncludeLiterals;
                return ((UltraNumericEditor)this.InputControl).Value;
            }

            if (this.InputControl is UltraComboEditor)
                return ((UltraComboEditor)this.InputControl).Value;

            if (this.InputControl is UltraDateTimeEditor)
                return ((UltraDateTimeEditor)this.InputControl).Value;

            return this.InputControl.Text;
        }

        private string CleanAnswer(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                switch (this.InputType)
                {
                    case InputType.Decimal:
                    case InputType.DecimalBefore:
                    case InputType.DecimalAfter:
                    case InputType.DecimalDifference:
                    case InputType.PreProcessWeight:
                    case InputType.PostProcessWeight:
                        decimal d;
                        if (!decimal.TryParse(value, out d))
                            return Regex.Replace(value, "[^.0-9]", "");
                        break;
                    case InputType.PartQty:
                    case InputType.Integer:
                    case InputType.TimeDuration:
                    case InputType.RampTime:
                        Int32 intMinValue;
                        if (!Int32.TryParse(value, out intMinValue))
                            return Regex.Replace(value, "[^0-9]", "");
                        break;
                    case InputType.Date:
                    case InputType.Time:
                    case InputType.TimeIn:
                    case InputType.TimeOut:
                    case InputType.DateTimeIn:
                    case InputType.DateTimeOut:
                    case InputType.String:
                    case InputType.List:
                    case InputType.None:
                    default:
                        break;
                }
            }

            return value;
        }

        private void SetAnswer(string value)
        {
            try
            {
                if (this.InputControl == null)
                    return;

                if (String.IsNullOrWhiteSpace(value))
                    return;

                if (this.InputControl is UltraCheckEditor)
                    ((UltraCheckEditor)this.InputControl).Checked = Convert.ToBoolean(value);
                else if (this.InputControl is UltraCurrencyEditor)
                    ((UltraCurrencyEditor)this.InputControl).ValueObject = value;
                else if (this.InputControl is UltraTextEditor)
                    ((UltraTextEditor)this.InputControl).Value = value;
                else if (this.InputControl is UltraNumericEditor)
                {
                    if (((UltraNumericEditor)this.InputControl).ValidateWithinRange(value))
                        ((UltraNumericEditor)this.InputControl).Value = value;
                }
                else if (this.InputControl is UltraComboEditor)
                    ((UltraComboEditor)this.InputControl).Value = value;
                else
                    this.InputControl.Text = value;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error setting answer to {0} for type {1}.".FormatWith(value, this.InputControl.GetType().Name));
            }
        }

        #endregion

        #region Events
        
        private void SmartBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }

        private void SmartBox_GotFocus(object sender, EventArgs e)
        {
            if(InputControl != null)
                InputControl.SelectAllText();
        }
        
        private void SmartBox_SelectionChanged(object sender, EventArgs e)
        {
            if(SelectionChanged != null)
                SelectionChanged(this, e);
        }

        //private void SmartBox_ButtonClicked(object sender, EventArgs e, string message)
        //{
        //    MessageBox.Show(((Infragistics.Win.Misc.UltraButton)sender).Text);
        //}

        private void InputControl_TextChanged(object sender, EventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged(this, e);
        }

        #endregion
    }
}
