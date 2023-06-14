using System.Collections.Generic;

namespace DWOS.Services.Messages
{
    /// <summary>
    /// Client request to log in.
    /// </summary>
    public class UserLogInRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the user PIN for this instance.
        /// </summary>
        public string UserPin { get; set; }
    }

    /// <summary>
    /// Server response with user profile information.
    /// </summary>
    public class UserProfileResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the user profile for this instance.
        /// </summary>
        public UserProfileInfo UserProfile { get; set; }
    }

    /// <summary>
    /// Server response with user information.
    /// </summary>
    public class UserResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the user for this instance.
        /// </summary>
        public UserInfo User { get; set; }
    }

    /// <summary>
    /// Represents user profile information.
    /// </summary>
    public class UserProfileInfo
    {
        /// <summary>
        /// Gets or sets the user ID for this instance.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the full name for this instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the title for this instance.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the department for this instance.
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Gets or sets the image for this instance.
        /// </summary>
        /// <value>
        /// The user's image if available; otherwise, <c>null</c>.
        /// </value>
        public byte[] Image { get; set; }

        /// <summary>
        /// Gets or sets the security roles for this instance.
        /// </summary>
        public List<string> SecurityRoles { get; set; }
    }

    /// <summary>
    /// Represents user summary information.
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// Gets or sets the user ID for this instance.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the name for this instance.
        /// </summary>
        public string Name { get; set; }
    }
}
