using System;
using System.Collections.Generic;

namespace DWOS.LicenseManager
{
    /// <summary>
    /// Interface to provide common structure to CRUD user activations regardless of how they are stored.
    /// </summary>
    public interface IUserStorageProvider
    {
        /// <summary>
        /// Gets the number of current users.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the list of current users.
        /// </summary>
        /// <returns></returns>
        List<UserActivation> CurrentUsers { get; }

        /// <summary>
        /// Retrieves a specific user from the list of current users.
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        UserActivation Get(Guid uid);

        /// <summary>
        /// Adds a new user.
        /// </summary>
        /// <param name="user">
        /// The user to add.
        /// </param>
        void Add(UserActivation user);

        /// <summary>
        /// Removes a current user.
        /// </summary>
        /// <param name="user">
        /// The user to remove.
        /// </param>
        void Remove(UserActivation user);
    }
}
