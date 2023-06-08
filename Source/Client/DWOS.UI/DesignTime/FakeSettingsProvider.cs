using DWOS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWOS.UI.DesignTime
{
    internal class FakeSettingsProvider : IDwosApplicationSettingsProvider
    {
        #region IDwosApplicationSettingsProvider Members

        public ApplicationSettings Settings =>
            new ApplicationSettings(new FakeSettingsPersistence(), new FakePathProvider());

        #endregion
    }
}
