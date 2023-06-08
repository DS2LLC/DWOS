using DWOS.Data;
using DWOS.Shared.Utilities;
using NLog;
using System.Linq;

namespace DWOS.Reports
{
    /// <summary>
    /// Signature data
    /// </summary>
    internal class SignatureData
    {
        /// <summary>
        /// Name to use on the signature
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Title to use on the signature
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Image to use on the signature
        /// </summary>
        public Infragistics.Documents.Reports.Graphics.Image Image { get; private set; }

        /// <summary>
        /// Generates signature data for the given user if it's allowed
        /// </summary>
        /// <param name="qaUser">QA user used to construct signature data.</param>
        /// <returns>data object if user can sign COCs; otherwise, null</returns>
        public static SignatureData From(Data.Datasets.COCDataset.UsersRow qaUser)
        {
            if (qaUser == null)
            {
                return null;
            }

            Infragistics.Documents.Reports.Graphics.Image signatureImage = null;

            bool hasCOCSignPermission = false;

            // COCSign permission check
            using (var tableAdapter = new Data.Datasets.SecurityDataSetTableAdapters.User_SecurityRolesTableAdapter())
            {
                using (var userSecurityRolesTable = new Data.Datasets.SecurityDataSet.User_SecurityRolesDataTable())
                {
                    tableAdapter.FillAllByUser(userSecurityRolesTable, qaUser.UserID);
                    hasCOCSignPermission = userSecurityRolesTable.FindByUserIDSecurityRoleID(qaUser.UserID, "COCSign") != null;
                }
            }

            if (hasCOCSignPermission && !qaUser.IsSignCOCNull() && qaUser.SignCOC)
            {
                if (!qaUser.IsSignatureMediaIDNull())
                {
                    // Try to load the user's signature
                    if (qaUser.MediaRow == null)
                    {
                        string errorMsg = "Could not load signature image." +
                            "Using default signature name, title, and image.";

                        LogManager.GetCurrentClassLogger().Info(errorMsg);
                    }
                    else
                    {
                        if (qaUser.MediaRow.Media == null)
                        {
                            using (var taMedia = new Data.Datasets.OrdersDataSetTableAdapters.MediaTableAdapter())
                            {
                                qaUser.MediaRow.Media = taMedia.GetMediaStream(qaUser.SignatureMediaID);
                                qaUser.MediaRow.AcceptChanges(); //don't take this as a change to the row
                            }
                        }

                        if (!qaUser.MediaRow.IsMediaNull() && qaUser.MediaRow.Media.Length > 0)
                        {
                            var mediaImage = MediaUtilities.GetImage(qaUser.MediaRow.Media);
                            signatureImage = new Infragistics.Documents.Reports.Graphics.Image(mediaImage);
                        }
                        else
                        {
                            string errorMsg = "Could not load signature image." +
                                "Using default signature name, title, and image.";

                            LogManager.GetCurrentClassLogger().Info(errorMsg);
                        }
                    }
                }
                else
                {
                    string errorMsg = "QA user has no signature image." +
                        "Using default signature name, title, and image.";

                    LogManager.GetCurrentClassLogger().Info(errorMsg);
                }
            }

            if (signatureImage != null)
            {
                string title = string.Empty;

                if (!qaUser.IsTitleNull())
                {
                    title = qaUser.Title;
                }

                return new SignatureData()
                {
                    Name = qaUser.Name,
                    Title = title,
                    Image = signatureImage
                };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Generates signature data for the given user if it's allowed
        /// </summary>
        /// <param name="qaUser">User to construct signature data for.</param>
        /// <returns>data object if user can sign COCs; otherwise, null</returns>
        public static SignatureData From(ISecurityUserInfo qaUser)
        {
            if (qaUser == null)
            {
                return null;
            }

            using (var dsCoc = new Data.Datasets.COCDataset())
            {
                Data.Datasets.COCDataset.UsersRow userRow;
                using (new UsingDataSetLoad(dsCoc))
                {
                    using (var taUsers = new Data.Datasets.COCDatasetTableAdapters.UsersTableAdapter())
                    {
                        taUsers.FillByUser(dsCoc.Users, qaUser.UserID);
                    }

                    userRow = dsCoc.Users.FirstOrDefault();

                    if (userRow != null && !userRow.IsSignatureMediaIDNull())
                    {
                        using (var taMedia = new Data.Datasets.COCDatasetTableAdapters.MediaTableAdapter())
                        {
                            taMedia.FillByMedia(dsCoc.Media, userRow.SignatureMediaID);
                        }
                    }
                }

                return From(userRow);

            }
        }

        /// <summary>
        /// Returns a default instance of <see cref="SignatureData"/>.
        /// </summary>
        /// <returns>default instance from ApplicationSettings</returns>
        public static SignatureData Default()
        {
            Infragistics.Documents.Reports.Graphics.Image image = null;

            var signaturePath = ApplicationSettings.Current.COCSignatureImagePath;
            if (!string.IsNullOrEmpty(signaturePath))
            {
                image = new Infragistics.Documents.Reports.Graphics.Image(signaturePath);
            }

            return new SignatureData()
            {
                Name = ApplicationSettings.Current.COCSignatureName ?? string.Empty,
                Title = ApplicationSettings.Current.COCSignatureTitle ?? string.Empty,
                Image = image
            };
        }
    }
}
