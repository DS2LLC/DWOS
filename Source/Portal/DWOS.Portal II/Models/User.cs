using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DWOS.Portal.Models
{
    /// <summary>
    /// Represents a user.
    /// </summary>
    public class User
    {
        #region Properties

        public int CustomerId { get; set; }

        public string CompanyName { get; set; }

        public int ContactId { get; internal set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public DateTime LastLogin { get; set; }

        /// <summary>
        /// Gets or sets the list of secondary (additional) customers for
        /// this instance.
        /// </summary>
        public List<int> SecondaryCustomerIds { get; set; }

        [IgnoreDataMember]
        public List<int> AllCustomerIds => GetAllCustomerIds();

        #endregion

        #region Methods

        private List<int> GetAllCustomerIds()
        {
            if (SecondaryCustomerIds == null)
            {
                return new List<int> { CustomerId };
            }

            return SecondaryCustomerIds
                .Concat(new[] { CustomerId })
                .ToList();
        }

        #endregion
    }
}