using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using BasicCard.BasicCards;
using BasicCard.Terminals;
using Ionic.Zip;
using NLog;

namespace DWOS.SmartCardManager
{
    public class SmartCardManager
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
		public const string APPLICATION_NAME = "DWOS";

        private static bool _unzippedFiles;

        public enum SmartCardVersion { ZC312_A, ZC312_B, ZC312_C, ZC313_A, ZC313_B, ZC313_C, ZC314_D, ZC39, Unknown }

        public event EventHandler<ActionTextEventArgs> ActionOccured;

        public void WriteUserID(string userID)
        {
            UnZipSmartCardFiles();

            string programPath = Application.StartupPath + "\\SmartCard";

            if (System.IO.Directory.Exists(programPath))
            {
                //set working directory
                System.Environment.CurrentDirectory = programPath;

                //get the DWOS program for the smart card
                string programText = System.IO.File.ReadAllText(Path.Combine(programPath, "DWOS.BAS")).Replace("<USERID>", userID);

                AddOutput("Creating custom user program.");

                //write out new program for this user id
                string programUserPath = Path.Combine(programPath, userID + "_DWOS.BAS");
                System.IO.File.WriteAllText(programUserPath, programText);

                ICardReader reader = GetCardReader();
                if(reader != null)
                {
                    SmartCardVersion cardVersion = GetCardVersion(reader);
                    AddOutput("Found card version: " + cardVersion.ToString());

                    //compile Program NOTE: ZC39 does not require version number, but ZC312 does
                    var compileOptions  = userID + "_DWOS " + (cardVersion == SmartCardVersion.ZC39 ? "-C3.9" : ("-CF" + cardVersion.ToString() + ".zcf")) + " -OI";
                    var compileSI       = new ProcessStartInfo(Path.Combine(programPath, "ZCMBasic.exe"), compileOptions) { UseShellExecute = false, ErrorDialog = false, RedirectStandardError = true, RedirectStandardOutput = true };
                    var compileProcess  = new Process {StartInfo = compileSI};

                    compileProcess.Start();

                    AddOutput("Compiling user program.");

                    StreamReader compileOut = compileProcess.StandardOutput;
                    StreamReader compileError = compileProcess.StandardError;
                    compileProcess.WaitForExit();

                    if(compileProcess.ExitCode == 0)
                    {
                        AddOutput(compileOut.ReadToEnd());

                        //Load Program on card
                        var downloadSI = new ProcessStartInfo(Path.Combine(programPath, "BCLOAD.EXE"), userID + "_DWOS -P100 -STest") { UseShellExecute = false, ErrorDialog = false, RedirectStandardError = true, RedirectStandardOutput = true};
                        var downloadProc = new Process {StartInfo = downloadSI};

                        downloadProc.Start();

                        AddOutput("Loading compiled program on smart card.");

                        var loadError = downloadProc.StandardError;
                        downloadProc.WaitForExit();

                        if(downloadProc.ExitCode == 0)
                            AddOutput("Successfully wrote user id: " + userID + "!");
                        else
                            AddOutput("Error writing application to smart card.", loadError.ReadToEnd());
                    }
                    else
                        AddOutput("Error compiling program.", compileError.ReadToEnd());
                }
                else
                    AddOutput("Unable to find a smart card reader.", "No smart card reader found.");
            }
            else
                AddOutput("Unable to find program file: " + programPath, "File not found.");
        }

        public string ReadCurrentUserID()
        {
            try
            {
				_log.Info("SmartCardManager.ReadCurrentUserID");
                AddOutput("Checking smart card user...");

                ICardReader reader = GetCardReader();

                if (reader != null)
                {
                    AddOutput("Reader found: " + reader.Name);
                        
                    if (reader.Status.CardInReader)
                    {
                        AddOutput("Card found in reader.");

                        SmartCardVersion cardVersion = GetCardVersion(reader);
                        AddOutput("Found card version: " + cardVersion.ToString());
						_log.Info("SmartCardManager.ReadCurrentUserID Found card version: " + cardVersion.ToString());

                        using (ICardConnection cardConn = reader.Connect(true))
                        {
							using(DWOSSmartCardService dwosCard = new DWOSSmartCardService())
                            {
                            	dwosCard.Init(cardConn);
                           
								var cardApplicationName = dwosCard.CmdGetApplicationID();
                                
								if(cardApplicationName != null && cardApplicationName.ToString(BasicCardString.CharsetIdAscii) == APPLICATION_NAME)
								{
									AddOutput("Smart card application found: " + cardApplicationName);

									var id = dwosCard.ReadUserLogOnID();
									
									AddOutput("Found id: " + id);
                            		_log.Info("SmartCardManager.ReadCurrentUserID; Found card ID " + id); 
									return id;
								}
								else
									AddOutput("Smartcard found does not have correct application loaded on it.");
							}
                        }
                    }
                    else
                        AddOutput("No smartcard found in the reader.");
                }
                else
                    AddOutput("No smart card reader detected.");

                return null;
            }
            catch (Exception exc)
            {
				const string MSG = "Error getting user id from smart card.";
				_log.Warn(exc, MSG);
				AddOutput(MSG);
                return null;
            }
        }

        private ICardReader GetCardReader()
        {
            BasicCard.Terminals.Pcsc.PcscReaderFactory rf = new BasicCard.Terminals.Pcsc.PcscReaderFactory();

            if (rf.Readers.Length > 0)
                return rf.Readers[0] as BasicCard.Terminals.Pcsc.PcscReader;

            return null;
        }

        private SmartCardVersion GetCardVersion(ICardReader reader)
        {
            if (reader.Atr != null)
            {
                System.Text.ASCIIEncoding ascii = new ASCIIEncoding();
                string cardName = ascii.GetString(reader.Atr.GetBytes());

                if(cardName.Contains("ZC3.12 REV A"))
                    return SmartCardVersion.ZC312_A;
                if(cardName.Contains("ZC3.12 REV B"))
                    return SmartCardVersion.ZC312_B;
                if(cardName.Contains("ZC3.12 REV C"))
                    return SmartCardVersion.ZC312_C;

                if (cardName.Contains("ZC3.13 REV A"))
                    return SmartCardVersion.ZC313_A;
                if (cardName.Contains("ZC3.13 REV B"))
                    return SmartCardVersion.ZC313_B;
                if (cardName.Contains("ZC3.13 REV C"))
                    return SmartCardVersion.ZC313_C;
               
                if (cardName.Contains("ZC3.14"))
                    return SmartCardVersion.ZC314_D;

                if(cardName.Contains("ZC3.9"))
                    return SmartCardVersion.ZC39;
            }

            return SmartCardVersion.Unknown;
        }

        private void AddOutput(string action)
        {
            if (ActionOccured != null)
                ActionOccured(this, new ActionTextEventArgs() {ActionText = action});
        }

        private void AddOutput(string action, string errorDesc)
        {
            _log.Warn("", action, errorDesc);

            if (ActionOccured != null)
                ActionOccured(this, new ActionTextEventArgs() { ActionText = action, ErrorText = errorDesc });
        }

        private static void UnZipSmartCardFiles()
        {
            if (!_unzippedFiles)
            {
                try
                {
                    string programPath = Application.StartupPath + "\\SmartCard";

                    //if file is not there then unpackage contents
                    if (!File.Exists(Path.Combine(programPath, "ZCMBasic.exe")))
                    {
                        var zipPath = Path.Combine(programPath, "SmartCard.zip");

                        var zipFile = Ionic.Zip.ZipFile.Read(zipPath);

                        foreach (ZipEntry zipEntry in zipFile)
                            zipEntry.Extract(programPath, ExtractExistingFileAction.OverwriteSilently);
                    }

                    _unzippedFiles = true;
                }
                catch (Exception exc)
                {
                    string errorMsg = "Error unzipping smartcard package.";
                    _log.Error(exc, errorMsg);
                }
            }
        }
    }

	public class DWOSSmartCardService: AbstractBasicCardService
	{
		public DWOSSmartCardService()
			: base(SmartCardManager.APPLICATION_NAME)
		{
			doesNeedExclusiveConnection = false;
		}

		public string ReadUserLogOnID()
		{
			try
			{
				var bsExpression = new BasicCardString("", BasicCardString.CharsetIdAscii);
				var cmd = new BasicCardCommand(0x20, 0x01);
				cmd.AppendBasicString(bsExpression);

				BasicCardResponse rsp = DoCommandAndResponse(cmd);
				rsp.CheckSW1SW2();

				bsExpression = rsp.FetchBasicString();

				return bsExpression.ToString(BasicCardString.CharsetIdAscii);
			}
			catch(Exception exc)
			{
				NLog.LogManager.GetCurrentClassLogger().Warn(exc, "Error on reading user id from smart card.");
				return null;
			}
		}

		public new BasicCard.BasicCards.BasicCardString CmdGetApplicationID()
		{
			return base.CmdGetApplicationID();
		}
	}

    public class ActionTextEventArgs: EventArgs
    {
        public string ActionText { get; set; }
        public string ErrorText { get; set; }
    }
}
