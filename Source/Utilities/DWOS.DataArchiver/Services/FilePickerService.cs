using System.Windows.Forms;

namespace DWOS.DataArchiver.Services
{
    public class FilePickerService : IFilePickerService
    {
        #region IFilePickerService Members

        public string GetDirectory()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.SelectedPath;
                }
            }

            return null;
        }

        #endregion
    }
}
