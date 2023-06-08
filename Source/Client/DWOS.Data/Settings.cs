using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DWOS.Shared;
using DWOS.Shared.Settings;
using DWOS.Shared.Data;
using System.Threading;
using DWOS.Data.Datasets;
using DWOS.Shared.Utilities;

namespace DWOS.Data
{
	[Serializable] // Don't Obfuscate Me
	public class ApplicationSettings: SettingsBase
	{
		#region Fields

		private static ApplicationSettings _appSettings = null;
		private static ManualResetEvent _mre = null;
		private static object _lock = new object();
		
		private string _companyLogoFile;
		private string _qualitySignatureFile;
		private string _shippingTemplateFile;
		private TimeSpan? _shippingRolloverTime;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the default instance of the application settings class.
		/// </summary>
		/// <value>The default.</value>
		public static ApplicationSettings Current
		{
			get
			{
				//if null
				if(_appSettings == null)
				{
					lock(_lock)
					{
						//start loading if not already started to load
						if(_appSettings == null && _mre == null)
							BeginLoadSettings();
					}

					//ask to wait till other thread is completed
					if(_mre != null)
						_mre.WaitOne();
				}

				return _appSettings;
			}
		}

		public bool UseFileStreaming { get; set; }

		[DataColumn("ShippingNotification")]
		internal string ShippingNotificationAsString { get; set; }

		/// <summary>
		/// Gets the Shipping Notification file path.
		/// </summary>
		public string ShippingNotificationFilePath
		{
			get
			{
				if(_shippingTemplateFile == null)
				{
					_shippingTemplateFile = String.Empty;

					if(!String.IsNullOrEmpty(ShippingNotificationAsString))
					{
						_shippingTemplateFile = Path.Combine(FileSystem.UserAppDataPathVersion(), "ShippingNotificationTemplate.htm");

						if(File.Exists(_shippingTemplateFile))
							File.Delete(_shippingTemplateFile);

						var doc = new HtmlAgilityPack.HtmlDocument();
						doc.LoadHtml(ShippingNotificationAsString);
						doc.Save(_shippingTemplateFile);
					}
				}

				return _shippingTemplateFile; 
			}
		}

		[DataColumn("COCWarranty")]
		public string COCWarranty { get; internal set; }

		[DataColumn("COCSignatureName")]
		public string COCSignatureName { get; set; }

		[DataColumn("COCSignatureTitle")]
		public string COCSignatureTitle { get; set; }
		
		[DataColumn("COCSignature")]
		internal string COCSignatureAsString { get; set; }

		/// <summary>
		/// Gets the COC signature file path.
		/// </summary>
		public string COCSignatureImagePath
		{
			get
			{
				if(_qualitySignatureFile == null)
					_qualitySignatureFile = SaveImageFromString(COCSignatureAsString, "QualitySignature.png");

				return _qualitySignatureFile;
			}
		}

		[DataColumn("Company Address1")]
		public string CompanyAddress1 { get; internal set; }

		[DataColumn("Company City")]
		public string CompanyCity { get; internal set; }

		[DataColumn("Company Fax")]
		public string CompanyFax { get; internal set; }

		[DataColumn("CompanyLogo")]
		internal string CompanyLogoAsString { get; set; }

		/// <summary>
		/// Gets or sets the company logo file path.
		/// </summary>
		/// <value>
		/// The company logo.
		/// </value>
		public string CompanyLogoImagePath
		{
			get
			{
				if(_companyLogoFile == null)
					_companyLogoFile = SaveImageFromString(CompanyLogoAsString, "CompanyLogo_" + About.ApplicationVersion + ".png");

				return _companyLogoFile;
			}
		}

		[DataColumn("Company Name")]
		public string CompanyName { get; internal set; }

		[DataColumn("Company Phone")]
		public string CompanyPhone { get; internal set; }

		[DataColumn("Company State")]
		public string CompanyState { get; internal set; }

		[DataColumn("Company Zip")]
		public string CompanyZip { get; internal set; }
		
		[DataColumn("QuoteWarranty")]
		public string QuoteWarranty { get; internal set; }
		
		[DataColumn("WebSyncURL")]
		public string WebSyncURL { get; internal set; }
		
		[DataColumn("EmailPort")]
		public string EmailPort { get; internal set; }
		
		[DataColumn("EmailUserName")]
		public string EmailUserName { get; internal set; }
		
		[DataColumn("EmailPassword")]
		public string EmailPassword { get; internal set; }
		
		[DataColumn("EmailFromAddress")]
		public string EmailFromAddress { get; internal set; }

		[DataColumn("EmailSMTPServer")]
		public string EmailSMTPServer { get; internal set; }

		public TimeSpan ShipppingRolloverTime
		{
			get
			{
				if(!_shippingRolloverTime.HasValue && !string.IsNullOrEmpty(ShipppingRolloverTimeInt))
				{
					var times = ShipppingRolloverTimeInt.Split(':');
					int hour  = 0;
					int min   = 0;
					int sec   = 0;

					if(times.Length >= 1)
						int.TryParse(times[0], out hour);
					if(times.Length >= 2)
						int.TryParse(times[1], out min);
					if(times.Length >= 3)
						int.TryParse(times[2], out sec);

					_shippingRolloverTime = new TimeSpan(hour, min, sec);
				}

				return _shippingRolloverTime.GetValueOrDefault(TimeSpan.FromHours(16));
			}
		}

		[DataColumn("ShipppingRolloverTime")]
		internal string ShipppingRolloverTimeInt { get; set; }

		[DataColumn("CustomerPortalEmail")]
		public string CustomerPortalEmail { get; internal set; }

		[DataColumn("SurfaceAreaRequired")]
		public bool SurfaceAreaRequired { get; set; }

		/// <summary>
		/// Get the validation key for DWOS itself. Should be universal and not typically every change.
		/// </summary>
		[DataColumn("CryptoValidationKey")]
		public string CryptoValidationKey { get; set; }

		/// <summary>
		/// Get the license key for this installation.
		/// </summary>
		[DataColumn("CryptoLicenseKey")]
		public string CryptoLicenseKey { get; set; }

		/// <summary>
		/// Get the location of the license manager.
		/// </summary>
		[DataColumn("CryptoLicenseServerUrl")]
		public string CryptoLicenseServerUrl { get; set; }
		
		/// <summary>
		/// Gets or sets the type of login to use.
		/// </summary>
		/// <value>
		/// The type of the login.
		/// </value>
		public LoginType LoginType { get; set; }

		[DataColumn("LoginType")]
		internal string LoginTypeInternal
		{
			get { return this.LoginType.ToString(); }
			set
			{
				LoginType lt = Data.LoginType.Pin;
				
				if(Enum.TryParse<LoginType>(value, out lt))
					this.LoginType = lt;
			}
		}

		[DataColumn("MultiSite")]
		public bool MultiSite { get; set;}

		[DataColumn("SchedulingEnabled")]
		public bool SchedulingEnabled { get; set; }

		[DataColumn("OrderLeadTime")]
		public int OrderLeadTime { get; set; }

		[DataColumn("DepartmentShipping")]
		public string DepartmentShipping { get; set; }

		[DataColumn("DepartmentPartMarking")]
		public string DepartmentPartMarking { get; set; }

		[DataColumn("DepartmentQA")]
		public string DepartmentQA { get; set; }

		[DataColumn("DepartmentSales")]
		public string DepartmentSales { get; set; }

		#endregion

		#region Methods

		internal ApplicationSettings()
		{
			this.OrderLeadTime = 10;

			this.DepartmentShipping = "Shipping";
			this.DepartmentPartMarking = "Part Marking";
			this.DepartmentQA = "QA";
			this.DepartmentSales = "Sales";

			//this.LoginType = Data.LoginType.Smartcard;

			//this.CryptoValidationKey	= "AMAAMADS9Of+flACbUb7TPpEkLFPAJsw7Vobk3yVPZhvLznvBeIf5mS4Pas7ZItN7NAo25kDAAEAAQ==";
			//this.CryptoLicenseServerUrl = "http://localhost/LicenseService/Service.asmx";

			////Floating 3, w/ HeartBeat, Express
			////this.CryptoLicenseKey = "thGAIEebS4UCPs0B5b+dqYdVzQEDAD4AQ3VzdG9tZXI9aHR0cDovL2xvY2FsaG9zdC98RGF0ZSBQdXJjaGFzZWQ9NS8yOS8yMDEyIDY6NDQ6MzcgUE0BAQoAAABAD+TcG6ES7TGihYwuuccAUm57fdIaC+MyLotXIVGirNfZts+V+twke8LpDE3WNgU=";
			////30-Day Evaluation
			//this.CryptoLicenseKey = "thKEIFWAQKQNPs0BVQClnKBVzQEeAD4AQ3VzdG9tZXI9aHR0cDovL2xvY2FsaG9zdC98RGF0ZSBQdXJjaGFzZWQ9NS8yOS8yMDEyIDY6NDQ6MzcgUE0BAQoAAAClvxUxIkHYisr0pf5mXCZJzSiNcUjTMp8FmA3JEOMiyUBKaEgQ5DXuCZQeEKrOmYo=";

			//this.SurfaceAreaRequired = true;
		}

		/// <summary>
		/// Begins to load settings of application.
		/// </summary>
		public static void BeginLoadSettings()
		{
			lock(_lock)
			{
				try
				{
                    using (var ta = new DWOS.Data.Datasets.ApplicationSettingsDataSetTableAdapters.ApplicationSettingsTableAdapter())
					{
						using(var dtAS = ta.GetData())
						{
							List<SettingInfo> settingInfos = new List<SettingInfo>();

							foreach (ApplicationSettingsDataSet.ApplicationSettingsRow dr in dtAS)
								settingInfos.Add(new SettingInfo(){SettingName = dr.SettingName, SettingValue = dr.Value});

							ApplicationSettings settings = new ApplicationSettings();
							settings.Load(settingInfos);
							_appSettings = settings;
						}
					}
				}
				catch(Exception exc)
				{
					System.Diagnostics.Debug.WriteLine(exc.ToString());
				}
			}
		}

		private string SaveImageFromString(string imgString, string fileName)
		{
			try
			{
				if(imgString != null && imgString.Length > 8)
				{

					string directory = null;

					//Change to common data path for web to hopefully prevent permission issues
					if(FileSystem.IsWebApplication())
						directory = FileSystem.CommonAppDataPathVersion();
					else
						directory = FileSystem.UserAppDataPathVersion();

					string path = Path.Combine(directory, fileName);

					if(!File.Exists(path))
					{
						var bmp = FileSystem.StringToImage(imgString);
						bmp.Save(path);
					}

					return path;
				}

				return String.Empty;
			}
			catch(Exception exc)
			{
				const string errorMsg = "Error saving file.";
				_log.ErrorException(errorMsg, exc);
				return null;
			}
		}

		#endregion
	}

	public enum LoginType
	{
		Pin = 0,
		Smartcard = 1,
		PinSmartcard = 2
	}
}

namespace DWOS.Data.Properties
{
	public sealed partial class Settings
	{

	}
}