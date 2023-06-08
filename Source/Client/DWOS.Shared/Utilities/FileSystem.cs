using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using NLog;

namespace DWOS.Shared.Utilities
{
    /// <summary>
    /// Provides general utility methods for accessing files.
    /// </summary>
    public static class FileSystem
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Serialization

        /// <summary>
        ///     Serialize an object into an XML format
        /// </summary>
        /// <param name="fullPath">filepath/name object will be serialized to</param>
        /// <param name="obj">object to serialize</param>
        /// <returns>success</returns>
        public static void SerializeObjectXml(string fullPath, object obj)
        {
            var pSerializer = new XmlSerializer(obj.GetType());
            StreamWriter pWriter = null;

            try
            {
                ResetFileAttributes(fullPath); //reset file attributes if already on file to allow overwrite

                pWriter = new StreamWriter(fullPath);
                pSerializer.Serialize(pWriter, obj);
            }
            finally
            {
                pWriter?.Close();
            }
        }

        /// <summary>
        ///     DeSerialze an object from an XML format
        /// </summary>
        /// <param name="fullPath">filepath/name object will be deserialized from</param>
        /// <param name="obj">object to deserialize</param>
        /// <returns>success</returns>
        public static void DeserializeObjectXml(string fullPath, ref object obj)
        {
            FileStream pFileStream = null;
            ResolveEventHandler handler = null;

            try
            {
                handler = CurrentDomain_AssemblyResolve;
                AppDomain.CurrentDomain.AssemblyResolve += handler;

                if (string.IsNullOrEmpty(fullPath))
                {
                    throw new ArgumentNullException(nameof(fullPath));
                }

                //ensure file exists
                if (File.Exists(fullPath))
                {
                    // Create file stream with read access only
                    // ReadOnly incase user does not have write permissions
                    pFileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                }
                else
                {
                    throw new FileNotFoundException("Unable to locate file: " + fullPath);
                }

                var pSerializer = new XmlSerializer(obj.GetType());
                obj = pSerializer.Deserialize(pFileStream);
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= handler;

                pFileStream?.Close();
            }
        }

        /// <summary>
        ///     Serialize the object into binary.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static MemoryStream SerializeBinary(object o)
        {
            var binaryFormat = new BinaryFormatter();
            var memStream = new MemoryStream();
            binaryFormat.Serialize(memStream, o);
            return memStream;
        }

        /// <summary>
        ///     DeSeralize binary byte[].
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        public static object DeserializeBinary(byte[] bytes)
        {
            var binaryFormat = new BinaryFormatter();
            return binaryFormat.Deserialize(new MemoryStream(bytes));
        }

        /// <summary>
        /// Converts an image to a base64 string.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ImageToString(Image image, ImageFormat format)
        {
            using(var ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        /// <summary>
        /// Converts a base64 string to an image.
        /// </summary>
        /// <remarks>
        /// This is not guaranteed to be an exact duplicate of the image.
        /// </remarks>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static Image StringToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);

            // Convert byte[] to Image
            Image image = Image.FromStream(ms, true);
            return image;
        }

        /// <summary>
        /// Serializes an object to a JSON file.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="obj"></param>
        /// <param name="encodeContents"></param>
        public static void SerializeJson(string fullPath, object obj, bool encodeContents = false)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.Auto});

            if (encodeContents)
                json = Encode(json);

            File.WriteAllText(fullPath, json);
        }

        /// <summary>
        /// Deserializes a JSON file to an object.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="obj"></param>
        /// <param name="isEncoded"></param>
        public static void DeserializeJson(string fullPath, ref object obj, bool isEncoded = false)
        {
            var text = File.ReadAllText(fullPath);

            if (isEncoded)
                text = Decode(text);

            obj = JsonConvert.DeserializeObject(text, obj.GetType(), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
        }

        public static T DeserializeJsonFile<T>(string fullPath, bool isEncoded = false)
        {
            var text = File.ReadAllText(fullPath);

            if (isEncoded)
            {
                text = Decode(text);
            }

            return JsonConvert.DeserializeObject<T>(text, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
        }

        /// <summary>
        /// Encodes text to Base64.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Encode(string input)
        {
            return Convert.ToBase64String(Encoding.Unicode.GetBytes(input));
        }

        /// <summary>
        /// Decodes Base64 text.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Decode(string input)
        {
            return Encoding.Unicode.GetString(Convert.FromBase64String(input));
        }

        #endregion

        #region Data Paths

        /// <summary>
        /// Gets the common application data directory for the application.
        /// </summary>
        /// <exception cref="System.Exception">Unable to get user application data directory.</exception>
        public static string CommonAppDataPath()
        {
            try
            {
                //must use special folder to work with both with exe's and dll's
                string stringDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), About.ApplicationCompany, About.ApplicationName);

                //if directory does not exist then create 
                if(!Directory.Exists(stringDir))
                    Directory.CreateDirectory(stringDir);

                return stringDir;
            }
            catch(Exception exc)
            {
                throw new Exception("Unable to get user application data directory.", exc);
            }
        }

        /// <summary>
        /// Gets the common app data path for the application.
        /// </summary>
        /// <returns></returns>
        public static string CommonAppDataPathVersion()
        {
            try
            {
                string stringDir = Path.Combine(CommonAppDataPath(), About.ApplicationVersion);

                //if directory does not exist then create 
                if(!Directory.Exists(stringDir))
                    Directory.CreateDirectory(stringDir);

                return stringDir;
            }
            catch(Exception exc)
            {
                throw new Exception("Unable to get user application data version directory.", exc);
            }
        }

        /// <summary>
        /// Gets the user data path for the application.
        /// </summary>
        /// <returns>User Data Path</returns>
        public static string UserAppDataPath()
        {
            try
            {
                //NOTE: Do not use the method below, as it may give the owning processes data path, not
                //		this applications data path, Mainly a concern with ArcMap extensions.
                //					string stringDir = null;
                //					
                //					stringDir = Directory.GetParent(System.Windows.Forms.Application.UserAppDataPath).FullName;
                //					stringDir += "\\" + About.ApplicationCompany;
                //					stringDir += "\\" + About.ApplicationName;

                //must use special folder to work with both with exe's and dll's
                string stringDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), About.ApplicationCompany, About.ApplicationName);

                //if directory does not exist then create 
                if(!Directory.Exists(stringDir))
                    Directory.CreateDirectory(stringDir);

                return stringDir;
            }
            catch(Exception exc)
            {
                throw new Exception("Unable to get user application data directory.", exc);
            }
        }

        /// <summary>
        /// Gets the user data path for the application and application version.
        /// </summary>
        /// <returns></returns>
        public static string UserAppDataPathVersion()
        {
            try
            {
                string stringDir = UserAppDataPath() + "\\" + About.ApplicationVersion;

                //if directory does not exist then create 
                if(!Directory.Exists(stringDir))
                    Directory.CreateDirectory(stringDir);
                return stringDir;
            }
            catch(Exception exc)
            {
                throw new Exception("Unable to get user application data version directory.", exc);
            }
        }

        /// <summary>
        /// Get the user's temp directory.
        /// </summary>
        /// <returns></returns>
        public static string GetTempDirectory() { return Path.GetTempPath().TrimEnd('\\'); }

        /// <summary>
        /// Gets a path to a user directory for application documents.
        /// </summary>
        /// <returns>Path String</returns>
        public static string UserDocumentPath()
        {
            try
            {
                var stringDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                    About.ApplicationName);

                //if directory does not exist then create 
                if(!Directory.Exists(stringDir))
                    Directory.CreateDirectory(stringDir);

                return stringDir;
            }
            catch(Exception exc)
            {
                throw new Exception("Error getting user document directory.", exc);
            }
        }

        /// <summary>
        /// Gets the Application path where the executable exists.
        /// </summary>
        /// <returns>Returns application path string</returns>
        public static string ApplicationPath()
        {
            try
            {
                string stringDir = null;

                stringDir = Path.GetDirectoryName(About.CurrentAssembly.CodeBase);
                stringDir = stringDir.Replace(@"file:\", "");

                //if directory does not exist then throw error
                if(!Directory.Exists(stringDir))
                    throw new Exception("App directory cannot be found.");

                return stringDir;
            }
            catch(Exception exc)
            {
                throw new Exception("Error getting application directory.", exc);
            }
        }

        /// <summary>
        /// Gets the specified folder.
        /// </summary>
        /// <param name="folderType"></param>
        /// <param name="createIfNotExist"></param>
        /// <returns>Path to the specified folder if found; otherwise, null.</returns>
        public static string GetFolder(enumFolderType folderType, bool createIfNotExist)
        {
            try
            {
                string directory = null;

                switch(folderType)
                {
                    case enumFolderType.MediaCache:
                        directory = Path.Combine(UserDocumentPath(), "MediaCache");
                        break;
                    case enumFolderType.Reports:
                        directory = Path.Combine(UserDocumentPath(), "Reports");
                        break;
                    case enumFolderType.Documents:
                        directory = Path.Combine(UserDocumentPath(), "Documents");
                        break;
                    case enumFolderType.PartTemplates:
                        directory = Path.Combine(UserDocumentPath(), "Templates\\Parts");
                        break;
                    default:
                        break;
                }

                if(directory != null && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                return directory;
            }
            catch(Exception exc)
            {
                throw new Exception("Error getting application common data directory.", exc);
            }
        }

        #endregion

        #region File Security

        /// <summary>
        /// Represents the account type to use for a file system permission.
        /// </summary>
        public enum AccountType
        {
            AuthenticatedUsers,
            Everyone
        }

        /// <summary>
        /// Adds a permission to a file.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="rights"></param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        public static bool AddFilePermission(string file, FileSystemRights rights, AccountType accountType)
        {
            try
            {
                _log.Debug("Setting file permission on file {0} for user {1} to {2}.", file, accountType.ToString(), rights.ToString());
                var fileInfo = new FileInfo(file);
                FileSecurity fileSecurity = fileInfo.GetAccessControl();
                AuthorizationRuleCollection rules = fileSecurity.GetAccessRules(true, true, typeof(NTAccount));
                NTAccount account = GetAccount(accountType);

                foreach(FileSystemAccessRule rule in rules)
                {
                    if(rule.IdentityReference.Value.Contains(false, account.Value))
                        return true; //if already has this permission then just keep it
                }

                fileSecurity.AddAccessRule(new FileSystemAccessRule(account, rights, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                fileInfo.SetAccessControl(fileSecurity);

                return true;
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error("Error setting file permission to file {0}.", exc, file);
                return false;
            }
        }

        /// <summary>
        /// Adds a permission to a folder.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="rights"></param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        public static bool AddDirectoryPermission(string folder, FileSystemRights rights, AccountType accountType)
        {
            try
            {
                _log.Debug("Setting directory permission on directory {0} for user {1} to {2}.", folder, accountType.ToString(), rights.ToString());
                var dirInfo = new DirectoryInfo(folder);
                DirectorySecurity dirSecurity = dirInfo.GetAccessControl();
                AuthorizationRuleCollection rules = dirSecurity.GetAccessRules(true, true, typeof(NTAccount));
                NTAccount account = GetAccount(accountType);

                //if access rule already exists then remove
                foreach(FileSystemAccessRule rule in rules)
                {
                    if(rule.IdentityReference.Value.Contains(false, account.Value))
                    {
                        //dirSecurity.RemoveAccessRule(rule);
                        dirSecurity.RemoveAccessRuleSpecific(rule);
                    }
                }

                dirSecurity.AddAccessRule(new FileSystemAccessRule(account, rights, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                dirInfo.SetAccessControl(dirSecurity);

                return true;
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error("Error setting directory permission to folder {0}.", exc, folder);
                return false;
            }
        }

        /// <summary>
        /// Gets the account associated with an account type.
        /// </summary>
        /// <param name="accountType"></param>
        /// <returns></returns>
        public static NTAccount GetAccount(AccountType accountType)
        {
            if(accountType == AccountType.AuthenticatedUsers)
                return new NTAccount(@"NT AUTHORITY\Authenticated Users");

            return new NTAccount(@"NT AUTHORITY\Everyone");
        }

        #endregion

        #region Misc

        /// <summary>
        /// Set the file attributes of the file if it exists
        /// </summary>
        /// <param name="file">file</param>
        /// <param name="attributes">new file attributes</param>
        public static void SetFileAttributes(string file, FileAttributes attributes)
        {
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
            {
                return;
            }

            if (attributes == FileAttributes.Normal)
            {
                // Normal is a special case; it disables all attributes
                // and must be used by itself.
                File.SetAttributes(file, FileAttributes.Normal);
            }
            else
            {
                File.SetAttributes(file, File.GetAttributes(file) | attributes);
            }
        }

        /// <summary>
        ///  Reset the file attributes of the file if it exists to Normal
        /// </summary>
        /// <param name="file"></param>
        public static void ResetFileAttributes(string file)
        {
            try
            {
                if(file != null && File.Exists(file))
                    File.SetAttributes(file, FileAttributes.Normal);
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error changing file attributes [" + file + "].");
            }
        }

        /// <summary>
        /// Converts the number of bytes to megabytes.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        /// <summary>
        /// Convert the file size in bytes to a human readable string.
        /// </summary>
        /// <param name="byteCount"></param>
        /// <param name="decimalPlaces"></param>
        /// <returns></returns>
        public static string ConvertBytesToString(long byteCount, int decimalPlaces = 2)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            
            if (byteCount == 0)
                return "0 " + suf[0];
            
            long bytes = Math.Abs(byteCount);
            int place  = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), decimalPlaces);
            
            return (Math.Sign(byteCount) * num).ToString() + " " + suf[place];
        }

        /// <summary>
        ///  Determines whether the current app is hosted in a web application.
        /// </summary>
        /// <returns>
        /// <c>true</c> if [is web application]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsWebApplication()
        {
            string fileName = Process.GetCurrentProcess().MainModule.FileName;
            return fileName.Contains("w3wp.exe") || fileName.Contains("aspnet_wp.exe");
        }

        /// <summary>
        /// Generates a MD5 hash from a file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetMD5HashFromFile(string fileName)
        {
            using(var file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using(var md5 = MD5.Create())
                {
                    byte[] retVal = md5.ComputeHash(file);
                    return BitConverter.ToString(retVal);
                }
            }
        }

        /// <summary>
        /// Saves a memory stream to a file.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        public static void SaveToFile(this MemoryStream stream, string fileName)
        {
            using (var file = new FileStream(fileName, FileMode.Create, System.IO.FileAccess.Write))
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
                file.Write(bytes, 0, bytes.Length);
                stream.Close();
            }    
        }

        #endregion

        #region Events

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //hide un-needed assembly ref lookups from the logging
            if(args.Name != null && args.Name.Contains("InfragisticsWPF4.Themes") || args.Name.Contains("Neodynamic.Windows.ThermalLabelEditor.resources") ||  args.Name.Contains("XmlSerializers"))
                return null;

            _log?.Warn("Error resolving assembly: " + args.Name);

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if(assembly.GetName().Name == args.Name)
                {
                    _log?.Warn("Resolved assembly as: " + assembly.FullName);

                    return assembly;
                }
            }

            return null;
        }

        #endregion

        #region enumFolderType

        /// <summary>
        /// Defines DWOS data folders.
        /// </summary>
        public enum enumFolderType
        {
            MediaCache,
            Reports,
            Documents,
            PartTemplates
        }

        #endregion
    }
}