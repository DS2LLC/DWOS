//
//  Copyright 2012  Xamarin Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using DWOS.Utilities;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DWOS.ViewModels
{
    /// <summary>
    /// Base class for all view models
    /// - Implements INotifyPropertyChanged for WinRT
    /// - Implements some basic validation logic
    /// - Implements some IsBusy logic
    /// </summary>
    public class ViewModelBase : PropertyChangedBase
    {
        protected const string NotLoggedInMessage = "Not logged in.";

        /// <summary>
        /// Event for when IsBusy changes
        /// </summary>
        public event EventHandler IsBusyChanged;

        /// <summary>
        /// Event for when IsValid changes
        /// </summary>
        public event EventHandler IsValidChanged;

        readonly List<string> errors = new List<string>();
        bool isBusy = false;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ViewModelBase()
        {
            //Make sure validation is performed on startup
            Validate();
            Messenger.Default.Register<NotificationMessage>(this, (message) =>
                {
                    if (message.Notification == MessageNames.InvalidateViewModelMessage)
                        InvalidateViewModel();
                });
        }

        /// <summary>
        /// Returns true if the current state of the ViewModel is valid
        /// </summary>
        public bool IsValid
        {
            get { return errors.Count == 0; }
        }

        /// <summary>
        /// A list of errors if IsValid is false
        /// </summary>
        protected List<string> Errors
        {
            get { return errors; }
        }

        /// <summary>
        /// An aggregated error message
        /// </summary>
        public virtual string Error
        {
            get
            {
                return errors.Aggregate(new StringBuilder(), (b, s) => b.AppendLine(s)).ToString().Trim();
            }
        }

        /// <summary>
        /// Protected method for validating the ViewModel
        /// - Fires PropertyChanged for IsValid and Errors
        /// </summary>
        protected virtual void Validate()
        {
            OnPropertyChanged("IsValid");
            OnPropertyChanged("Errors");

            var method = IsValidChanged;
            if (method != null)
                method(this, EventArgs.Empty);
        }

        /// <summary>
        /// Other viewmodels should call this when overriding Validate, to validate each property
        /// </summary>
        /// <param name="validate">Func to determine if a value is valid</param>
        /// <param name="error">The error message to use if not valid</param>
        protected virtual void ValidateProperty(Func<bool> validate, string error)
        {
            if (validate())
            {
                if (!Errors.Contains(error))
                    Errors.Add(error);
            }
            else
            {
                Errors.Remove(error);
            }
        }

        protected virtual void ValidateProperty(bool isInvalid, string error)
        {
            if (isInvalid)
            {
                if (!Errors.Contains(error))
                    Errors.Add(error);
            }
            else
            {
                Errors.Remove(error);
            }
        }

        /// <summary>
        /// Value inidicating if a spinner should be shown
        /// </summary>
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;

                    OnPropertyChanged("IsBusy");
                    OnIsBusyChanged();
                }
            }
        }

        /// <summary>
        /// Other viewmodels can override this if something should be done when busy
        /// </summary>
        protected virtual void OnIsBusyChanged()
        {
            var method = IsBusyChanged;
            if (method != null)
                IsBusyChanged(this, EventArgs.Empty);
        }

        protected void LogError(string message, Exception exception = null, bool displayToast = false)
        {
            var errorService = ServiceContainer.Resolve<ILogService>();
            errorService.LogError(message, exception, context: null, toast: displayToast);
        }

        /// <summary>
        /// Invalidates/Resets the view model.
        /// </summary>
        public virtual void InvalidateViewModel()
        {
            
        }

        /// <summary>
        /// Requests that all view models be invalidated.
        /// </summary>
        public static void RequestInvalidateViewModel()
        {
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage(MessageNames.InvalidateViewModelMessage));
        }
    }
}