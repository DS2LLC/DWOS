using System;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.LicenseManager
{
    /// <summary>
    /// Memory-based implementation of <see cref="IUserStorageProvider"/>.
    /// </summary>
    /// 
    internal class MemoryUserStorageProvider : IUserStorageProvider
    {
        #region Fields

        private readonly List<UserActivation> _users = new List<UserActivation>();
        private readonly object _userSyncObject = new object();

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

        public UserActivation Get(Guid uid)
        {
            lock (_userSyncObject)
            {
                return this._users.FirstOrDefault(u => u.UID == uid);
            }
        }

        public void Add(UserActivation user)
        {
            lock (_userSyncObject)
            {
                _users.Add(user);
            }
        }

        public void Remove(UserActivation user)
        {
            lock (_userSyncObject)
            {
                _users.Remove(user);
            }
        }

        #endregion
    }


}
