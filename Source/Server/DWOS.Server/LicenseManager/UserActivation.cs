using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace DWOS.LicenseManager
{
    /// <summary>
    ///   Defines information about active clients.
    /// </summary>
    [Serializable]
    [DataContract]
    public class UserActivation : INotifyPropertyChanged
    {
        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private Guid _uid;
        private string _computerName;
        private int _userId;
        private string _userName;
        private DateTime _activated;
        private DateTime _lastCommunication;

        [DataMember]
        public Guid UID
        {
            get => _uid;
            set
            {
                if (_uid != value)
                {
                    _uid = value;
                    OnPropertyChanged(nameof(UID));
                }
            }
        }

        [DataMember]
        public string ComputerName
        {
            get => _computerName;
            set
            {
                if (!string.Equals(_computerName, value, StringComparison.Ordinal))
                {
                    _computerName = value;
                    OnPropertyChanged(nameof(ComputerName));
                }
            }
        }

        [DataMember]
        public int UserId
        {
            get => _userId;
            set
            {
                if (_userId != value)
                {
                    _userId = value;
                    OnPropertyChanged(nameof(UserId));
                }
            }
        }

        [DataMember]
        public string UserName
        {
            get => _userName;
            set
            {
                if (!string.Equals(_userName, value, StringComparison.Ordinal))
                {
                    _userName = value;
                    OnPropertyChanged(nameof(UserName));
                }
            }
        }

        [DataMember]
        public DateTime Activated
        {
            get => _activated;
            set
            {
                if (_activated != value)
                {
                    _activated = value;
                    OnPropertyChanged(nameof(Activated));
                }
            }
        }

        [DataMember]
        public DateTime LastCommunication
        {
            get => _lastCommunication;
            set
            {
                if (_lastCommunication != value)
                {
                    _lastCommunication = value;
                    OnPropertyChanged(nameof(LastCommunication));
                }
            }
        }

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public override string ToString()
        {
            return "Computer: " + this.ComputerName + " User: " + this.UserName + " UID: " + this.UID.ToString();
        }
    }
}