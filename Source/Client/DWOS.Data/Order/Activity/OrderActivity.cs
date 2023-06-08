using System;

namespace DWOS.Data.Order.Activity
{
    /// <summary>
    /// Base class that represents an activity that can occur on an order.
    /// </summary>
    public abstract class OrderActivity
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Order ID for this instance.
        /// </summary>
        public int OrderID { get; set; }

        #endregion

        #region Methods
        
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderActivity"/>
        /// class.
        /// </summary>
        /// <param name="orderId"></param>
        protected OrderActivity(int orderId)
        {
            OrderID = orderId;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Completes the activity.
        /// </summary>
        /// <returns></returns>
        public abstract ActivityResults Complete();

        /// <summary>
        /// Retrieves a user name.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>
        /// A user's name if that user was found; otherwise, returns a
        /// placeholder name.
        /// </returns>
        protected string GetUserName(int userId)
        {
            if(userId > 0)
            {
                using(var ta = new Data.Datasets.SecurityDataSetTableAdapters.UsersTableAdapter())
                {
                    var userName = ta.GetUserName(userId);

                    if(!String.IsNullOrWhiteSpace(userName))
                        return userName;
                }
            }
            
            if(userId == -99)
                return "System";

            return "UnKnown";
        }

        #endregion
    }


}
