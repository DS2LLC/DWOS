using System;
using DWOS.Shared.Utilities;

namespace DWOS.UI.Utilities
{
    public interface ISecurityManager
    {
        /// <summary>
        /// Occurs when the user changes.
        /// </summary>
        event EventHandler<UserUpdatedEventArgs> UserUpdated;

        ISecurityUserInfo UserInfo { get; }

        bool RequiresOrderReview { get; }

        /// <summary>
        ///   Determines whether the user is in the specified role.
        /// </summary>
        /// <param name="role"> The role. </param>
        /// <returns> <c>true</c> if [is in role] [the specified role]; otherwise, <c>false</c> . </returns>
        bool IsInRole(string role);

        /// <summary>
        /// Determines whether the user is in the specified security group.
        /// </summary>
        /// <param name="securityGroupId">The security group id.</param>
        /// <returns><c>true</c> if [is in group] [the specified security group id]; otherwise, <c>false</c>.</returns>
        bool IsInGroup(int securityGroupId);
    }
}