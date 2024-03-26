using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace DWOS.LicenseManager
{
    /// <summary>
    /// Implementation of <see cref="IUserStorageProvider"/> that saves changes to a binary file.
    /// </summary>
    internal class BinaryFileUserStorageProvider : IUserStorageProvider
    {
        #region Fields

        private readonly List<UserActivation> _users = new List<UserActivation>();
        private readonly object _userSyncObject = new object();

        #endregion

        #region Properties

        private string LicenseInfoFile
        {
            get
            {
                var storageDirectory = Environment.GetFolderPath(
                    Environment.SpecialFolder.CommonApplicationData);

                return Path.Combine(storageDirectory, "LicenseInfo.bin");
            }
        }

        #endregion

        #region Methods

        public BinaryFileUserStorageProvider()
        {
            // Load data
            if (File.Exists(LicenseInfoFile))
            {
                var formatter = new BinaryFormatter();
                using (var fileStream = File.OpenRead(LicenseInfoFile))
                {
                    var users = new List<UserActivation>(0);
                    try
                    {
                        users = formatter.Deserialize(fileStream) as List<UserActivation>;
                    }
                    catch (Exception)
                    {
                        //delete file
                        fileStream.Close();
                        File.Delete(LicenseInfoFile);
                    }
                   

                    lock (_userSyncObject)
                    {
                        foreach (var user in users ?? Enumerable.Empty<UserActivation>())
                        {
                            _users.Add(user);
                            user.PropertyChanged += User_PropertyChanged;
                        }
                    }
                }
            }
        }

        private void SaveData()
        {
            var formatter = new BinaryFormatter();
            using (var fileStream = File.Create(LicenseInfoFile))
            {
                lock (_userSyncObject)
                {
                    formatter.Serialize(fileStream, _users);
                }
            }
        }

        #endregion

        #region Events

        private void User_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                SaveData();
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger()
                    .Error(exc, "Error handling user property change.");
            }
        }

        #endregion

        #region IUserStorageProvider Members

        public int Count
        {
            get
            {
                lock (_userSyncObject)
                {
                    return _users.Count;
                }
            }
        }

        public List<UserActivation> CurrentUsers
        {
            get
            {
                lock (_userSyncObject)
                {
                    return new List<UserActivation>(_users);
                }
            }
        }

        public void Add(UserActivation user)
        {
            lock (_userSyncObject)
            {
                _users.Add(user);
            }

            SaveData();
            user.PropertyChanged += User_PropertyChanged;
        }

        public UserActivation Get(Guid uid)
        {
            lock (_userSyncObject)
            {
                return this._users.FirstOrDefault(u => u.UID == uid);
            }
        }

        public void Remove(UserActivation user)
        {
            lock (_userSyncObject)
            {
                _users.Remove(user);
            }

            SaveData();
            user.PropertyChanged -= User_PropertyChanged;

        }

        #endregion
    }
}
