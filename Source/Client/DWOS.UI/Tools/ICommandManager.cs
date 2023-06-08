using System;
using System.Windows.Forms;

namespace DWOS.UI.Tools
{
    public interface ICommandManager : IDisposable
    {
        ICommandBase AddCommand(string key, ICommandBase command);
        ICommandBase FindCommand<T>();
        ICommandBase FindCommandByKeyMapping(Keys key);
        void RefreshAll();
    }
}