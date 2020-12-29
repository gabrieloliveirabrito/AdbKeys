using System;
using System.Collections.Generic;
using System.Text;

namespace AdbKeys.Dependencies
{
    public abstract class IAppSettings : BaseDependency<IAppSettings>
    {
        public abstract void OpenAppSettings();
    }
}
