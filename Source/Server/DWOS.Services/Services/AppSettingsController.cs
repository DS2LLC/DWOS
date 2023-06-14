using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Services.Messages;
using DWOS.Shared;
using System.Linq;
using System.Web.Http;

namespace DWOS.Services
{
    public class AppSettingsController : ApiController
    {
        #region Methods

        /// <summary>
        /// Retrieves application settings.
        /// </summary>
        /// <remarks>
        /// This receives POST requests to ensure backwards and forwards
        /// compatibility with all DWOS Mobile versions.
        /// </remarks>
        /// <returns>
        /// An <see cref="ApplicationSettingsResponse"/> instance.
        /// </returns>
        [HttpPost]
        [ServiceExceptionFilter("Error getting settings info.")]
        public ApplicationSettingsResponse AppSettings()
        {
            return new ApplicationSettingsResponse { Success = true, ErrorMessage = null, ApplicationSettings = RetrieveSettings() };
        }

        private static ApplicationSettingsInfo RetrieveSettings()
        {
            var settings = new ApplicationSettingsInfo()
            {
                ServerVersion = About.ApplicationVersion,
                ServerApiVersion = ApplicationSettingsInfo.CURRENT_API_VERSION,
                CompanyName = ApplicationSettings.Current.CompanyName,
                WorkStatusChangingDepartment = ApplicationSettings.Current.WorkStatusChangingDepartment,
                WorkStatusInProcess = ApplicationSettings.Current.WorkStatusInProcess,
                WorkStatusPendingInspection = ApplicationSettings.Current.WorkStatusPendingQI,
                UsingManualScheduling = ApplicationSettings.Current.UsingManualScheduling,
                UsingMultipleLines = ApplicationSettings.Current.MultipleLinesEnabled,
                ClientUpdateIntervalSeconds = ApplicationSettings.Current.ClientUpdateIntervalSeconds,
                UsingTimeTracking = ApplicationSettings.Current.TimeTrackingEnabled
            };

            using (var ta = new Data.Datasets.OrderStatusDataSetTableAdapters.d_DepartmentTableAdapter())
            {
                var dt = new OrderStatusDataSet.d_DepartmentDataTable();
                ta.Fill(dt);

                if (dt.Count > 0)
                {
                    settings.Departments = dt.Where(d => d.Active).Select(dr => dr.DepartmentID).Distinct().ToList();
                }
            }

            using (var ta = new Data.Datasets.OrderStatusDataSetTableAdapters.ProcessingLineTableAdapter())
            {
                var processingLines = ta.GetData();
                settings.ProcessingLines = processingLines.Select(l => l.Name).ToList();
            }

            return settings;
        }

        #endregion
    }
}
