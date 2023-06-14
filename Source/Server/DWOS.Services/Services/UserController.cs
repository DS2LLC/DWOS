using System.Linq;
using DWOS.Data.Datasets;
using DWOS.Services.Messages;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace DWOS.Services
{
    public class UserController : ApiController
    {
        #region Methods

        [HttpPost]
        [ServiceExceptionFilter("Error logging in")]
        public ResponseBase Login(UserLogInRequest request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidData();
            }

            var userProfile = CreateUserProfileByPin(request.UserPin);

            //add log in histry to dwos db
            if (userProfile != null && userProfile.UserId > 0)
                UserLogging.AddLogInHistory(userProfile.UserId, "DWOS.Services");
            else
                UserLogging.AddFailedLogInHistory("DWOS.Services");

            return new UserProfileResponse
            {
                Success = true,
                ErrorMessage = null,
                UserProfile = userProfile
            };
        }

        [HttpGet]
        [ServiceExceptionFilter("Error getting user info.")]
        public ResponseBase Get(int userId)
        {
            return new UserResponse
            {
                Success = true,
                ErrorMessage = null,
                User = CreateUserInfo(userId)
            };
        }

        #endregion

        #region Factories

        private static UserProfileInfo CreateUserProfileByPin(string userPin)
        {
            var userProfile = new UserProfileInfo();
           
            try
            {
                using(var taOrders = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.UsersTableAdapter())
                {
                    var userId = taOrders.GetUserIdByUserLoginPin(userPin.ToString());
                    var usersTable = new DWOS.Data.Datasets.SecurityDataSet.UsersDataTable();

                    if (!userId.HasValue)
                        return null;

                    taOrders.FillByUserID(usersTable, userId.Value);
                    var userRow = usersTable.FirstOrDefault();

                    if(userRow == null)
                        return userProfile;

                    userProfile.UserId        = userRow.UserID;
                    userProfile.Name          = userRow.Name;
                    userProfile.Title         = userRow.IsTitleNull() ? null : userRow.Title;
                    userProfile.Department    = userRow.IsDepartmentNull() ? null : userRow.Department;
                    userProfile.SecurityRoles = new List <string>();

                    //fill user pic
                    if(!userRow.IsMediaIDNull())
                    {
                        using(var taMedia = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.MediaTableAdapter())
                            using(var mediaTable = new DWOS.Data.Datasets.SecurityDataSet.MediaDataTable())
                            {

                                taMedia.FillByIdWOMedia(mediaTable, userRow.MediaID);
                                
                                if(mediaTable.Count > 0)
                                {
                                    using(var taMedia2 = new DWOS.Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter())
                                        userProfile.Image = taMedia2.GetMediaStream(userRow.MediaID);
                                }
                            }
                    }

                    //fill all security roles
                    using(var taUserRoles = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.User_SecurityRolesTableAdapter())
                    {
                        var userSecurityRoles = new Data.Datasets.SecurityDataSet.User_SecurityRolesDataTable();
                        taUserRoles.FillAllByUser(userSecurityRoles, userId.Value);

                        foreach(var securityRole in userSecurityRoles)
                            userProfile.SecurityRoles.Add(securityRole.SecurityRoleID);
                    }
                    
                    return userProfile;
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error getting user profile.");
                return null;
            }
        }

        private static UserInfo CreateUserInfo(int userId)
        {
            var user = new UserInfo();

            try
            {
                using (var taOrders = new DWOS.Data.Datasets.SecurityDataSetTableAdapters.UsersTableAdapter())
                {
                    var usersTable = new DWOS.Data.Datasets.SecurityDataSet.UsersDataTable();
                    taOrders.FillByUserID(usersTable, userId);
                    
                    var userRow = usersTable.FirstOrDefault();

                    if (userRow == null)
                        return user;

                    user.UserId = userRow.UserID;
                    user.Name = userRow.Name;

                    return user;
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error getting user info.");
                return null;
            }
        }

        #endregion
    }
}