using System;
using System.Collections.Generic;
using System.Text;

namespace AdbKeys.Dependencies
{
    public abstract class ISnackBar : BaseDependency<ISnackBar>
    {
        public abstract void ShowSnackbar(string msg);
    }
}
