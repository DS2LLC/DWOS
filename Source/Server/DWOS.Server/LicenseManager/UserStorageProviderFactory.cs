namespace DWOS.LicenseManager
{
    internal static class UserStorageProviderFactory
    {
        public static IUserStorageProvider NewInstance()
        {
            if (Server.Properties.Settings.Default.UseInMemoryLicenseProvider)
            {
                return new MemoryUserStorageProvider();
            }

            return new BinaryFileUserStorageProvider();
        }
    }
}
